using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maintel.Icon.Portal.Domain;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.Domain.Enums;
using Maintel.Icon.Portal.SyncService.Highlight.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Maintel.Icon.Portal.SyncService.Highlight.Helpers
{
    public class EmailHelper :IEmailHelper
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _unparsedFolderName = "Unparsed Emails";
        private readonly string _invalidSenderFolderName = "Invalid Sender";
        private readonly string _duplicateEmailFolderName = "Duplicate Email";
        private FolderId _unparsedFolderId;
        private FolderId _invalidSenderFolderId;
        private FolderId _duplicateEmailFolderId;
        private readonly bool _deleteToDeletedItems;
        private readonly ILogger _logger;
        private ExchangeService _exchangeService;

        public EmailHelper(IConfiguration configuration,
                                ILogger logger)
        {
            _username = configuration["email:username"];
            _password = configuration["email:password"];
            _deleteToDeletedItems = (!string.IsNullOrWhiteSpace(configuration["email:deleteToDeletedItems"]) &&
                        configuration["email:deleteToDeletedItems"].ToLower() == "true");
            _logger = logger;
        }

        public bool ConnectToEmailServer()
        {

            _logger.Information($"Connecting to mailbox {_username}");
            try
            {
                _exchangeService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                _exchangeService.Credentials = new WebCredentials(_username, _password);
                _exchangeService.AutodiscoverUrl(_username, sSLRedirectionCallback);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Cannot connect to Email Server");
                return false;
            }

            _invalidSenderFolderId = getOrCreateFolder(_invalidSenderFolderName);
            if (_invalidSenderFolderId == null)
            {
                _logger.Error($"{_invalidSenderFolderName} could not be found or created");
                return false;
            }
            _unparsedFolderId= getOrCreateFolder(_unparsedFolderName);
            if (_unparsedFolderId == null)
            {
                _logger.Error($"{_unparsedFolderName} could not be found or created");
                return false;
            }
            _duplicateEmailFolderId = getOrCreateFolder(_duplicateEmailFolderName);
            if (_duplicateEmailFolderName == null)
            {
                _logger.Error($"{_duplicateEmailFolderName} could not be found or created");
                return false;
            }
            return true;
        }


        private FolderId getOrCreateFolder(string folderName)
        {
            FolderId folderId = checkFolderExists(folderName);
            if (folderId != null)
            {
                return folderId;
            }
            return createFolder(folderName);
        }

        private FolderId checkFolderExists(string folderName)
        {
            _logger.Debug($"Checking for existance of unparsed Folder ({folderName}) in mailbox");

            FindFoldersResults folderResults = _exchangeService.FindFolders(WellKnownFolderName.Inbox,
                                                                        new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName),
                                                                        new FolderView(1));

            if (folderResults.Folders.Count > 0)
            {
                _logger.Debug($"Found Folder ({folderName}) in mailbox");
                return folderResults.Folders[0].Id;
            }
            return null;
        }

        private FolderId createFolder(string folderName)
        {
           
            try
            {
                _logger.Warning($"{folderName} does not exist in mailbox. Attempting to create");
                // Create a custom folder.
                Folder folder = new Folder(_exchangeService);
                folder.DisplayName = folderName;
                folder.FolderClass = "IPF.Note";
                // Save the folder as a child folder of the Inbox.
                folder.Save(WellKnownFolderName.Inbox);
                _logger.Information($"Created {folderName} in mailbox");
                return  folder.Id;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating folder {folderName} for mailbox {_username}",ex);
                return null;
            }
            

        }

        private bool sSLRedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }

        private bool sendToInvalidSender(string uniqueIdentifier)
        {
            return sendToOtherFolder(uniqueIdentifier, _invalidSenderFolderName, _invalidSenderFolderId);
        }

        private bool sendToUnparsedEmail(string uniqueIdentifier)
        {
            return sendToOtherFolder(uniqueIdentifier, _unparsedFolderName, _unparsedFolderId);
        }

        private bool sendToDuplicated(string uniqueIdentifier)
        {
            return sendToOtherFolder(uniqueIdentifier, _duplicateEmailFolderName, _duplicateEmailFolderId);
        }
        private bool sendToOtherFolder(string uniqueIdentifier, string folderName, FolderId folderId)
        {
            try
            {
                PropertySet propSet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.ParentFolderId);
                EmailMessage beforeMessage = EmailMessage.Bind(_exchangeService, new ItemId(uniqueIdentifier), propSet);
                Item item = beforeMessage.Move(folderId);
                EmailMessage movedMessage = EmailMessage.Bind(_exchangeService, item.Id, propSet);
            }
            catch(Exception ex)
            {
                _logger.Error($"Could not move Email {uniqueIdentifier} to folder {folderName}",ex);
                return false;
            }
            
            return true;
        }

        public async Task<FullEmailListForRetrieveDto> RetrieveEmailBatch(int batchSize)
        {


            return await System.Threading.Tasks.Task.Run(() =>
            {
                List<FullEmailModel> fullEmails = new List<FullEmailModel>();
                FindItemsResults<Item> findResults = _exchangeService.FindItems(
                  WellKnownFolderName.Inbox,
                  new ItemView(batchSize)
               );
                if (findResults.Items.Count == 0)
                {
                    return null;
                }
                //When loading emails using EWS you need to load the items from the emnail schema you require
                _exchangeService.LoadPropertiesForItems(findResults.Items, new PropertySet(BasePropertySet.IdOnly, ItemSchema.TextBody, ItemSchema.DateTimeReceived, ItemSchema.DateTimeSent, ItemSchema.Subject, EmailMessageSchema.Sender));

                foreach (EmailMessage item in findResults.Items)
                {
                    fullEmails.Add(new FullEmailModel()
                    {
                        UniqueIdentifier = item.Id.UniqueId,
                        Body = item.TextBody,
                        DateRecieved = item.DateTimeReceived.ToUniversalTime(),
                        DateSent = item.DateTimeSent.ToUniversalTime(),
                        Title = item.Subject,
                        From = item.Sender.Address
                    });
                }

                return new FullEmailListForRetrieveDto() { Emails = fullEmails, TotalRetrieved = fullEmails.Count() };
            }
            );
            
        }

        public bool Delete(string emailId)
        {
            return DeleteBatch(new List<string>() { emailId });
        }
        public bool DeleteBatch(IEnumerable<string> emailIds)
        {

            List<ItemId> itemIds = new List<ItemId>();

            foreach (string emailId in emailIds)
            {
                itemIds.Add(new ItemId(emailId));
            }
            try
            {
                if (_deleteToDeletedItems)
                {
                    _exchangeService.DeleteItems(itemIds, DeleteMode.MoveToDeletedItems, SendCancellationsMode.SendToNone, AffectedTaskOccurrence.AllOccurrences);
                }
                else
                {
                    _exchangeService.DeleteItems(itemIds, DeleteMode.HardDelete, SendCancellationsMode.SendToNone, AffectedTaskOccurrence.AllOccurrences);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("An Error occured while deleting email batch", ex);
                return false;
            }

        }
        public bool MoveEmail(string id, CommunicationLocationEnum communicationLocationEnum)
        {
            switch (communicationLocationEnum)
            {
                case CommunicationLocationEnum.Duplicate:
                    {
                        return sendToDuplicated(id);
                    }
                case CommunicationLocationEnum.InvalidSender:
                    {
                        return sendToInvalidSender(id);
                    }
                case CommunicationLocationEnum.Unparsed:
                    {
                        return sendToUnparsedEmail(id);
                    }
            }
            _logger.Warning($"Could not move email ({id}) as invalid communications location specified ({communicationLocationEnum})");
            return false;
        }
    }
}
