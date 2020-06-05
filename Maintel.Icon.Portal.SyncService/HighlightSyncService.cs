using Maintel.Icon.Portal.API.Dal;
using Maintel.Icon.Portal.Domain;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.Domain.Enums;
using Maintel.Icon.Portal.Domain.Interfaces;
using Maintel.Icon.Portal.SyncService.Highlight.Dtos;
using Maintel.Icon.Portal.SyncService.Highlight.Enums;
using Maintel.Icon.Portal.SyncService.Highlight.Helpers;
using Maintel.Icon.Portal.SyncService.Highlight.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService
{
    public class HighlightSyncService
    {
        private readonly int _batchSize = 50;
        private readonly string _acceptedEmailAddress;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private IEmailHelper _emailHelper;
        private EmailAlertsDal _emailAlertsDal;
        private readonly string _communicationApi;

        public HighlightSyncService(ILogger logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            int batchSize;
            if (int.TryParse(_configuration["email:batchsize"], out batchSize))
            {
                _batchSize = batchSize;
            }
            _acceptedEmailAddress = _configuration["email:acceptedEmailAddress"];
            _communicationApi = _configuration["email:communicationApi"];
        }

        public void RefreshAssociations()
        {
            _logger.Information("Attempting to link sites and folders");
            SiteDal siteDal = new SiteDal(_configuration, _logger);
            IEnumerable<SiteForExtendedTicketDto> sites = siteDal.GetSiteForExtendedTicket("red alert").GetAwaiter().GetResult();
            _logger.Information($"Retreived {sites.Count()} new link sites");
            Dictionary<string, int> siteAssociation = new AssociationHelper(sites, _logger).SiteAssociation;

            foreach (KeyValuePair<string, int> site in siteAssociation)
            {
                CRUDEnums.ActionResult result = siteDal.Post(site.Value, site.Key).GetAwaiter().GetResult();
                if (result!= CRUDEnums.ActionResult.Success)
                {
                    _logger.Error($"Error updating site {site.Value} with external identifier {site.Key} Error {result.ToString()}");
                }

            }
            _logger.Information("Completed link sites and folders");
        }

        public void Sync()
        {
            _logger.Information("Attempting Highlight Sync");
            //Retrieve Emails from mailbox
            _emailHelper = new EmailConnectionHelper(_communicationApi);
            if (!_emailHelper.ConnectToEmailServer())
            {
                _logger.Error("Could not connect to email server");
                return;
            }

            FullEmailListForRetrieveDto emails = _emailHelper.RetrieveEmailBatch(_batchSize).GetAwaiter().GetResult();
            _logger.Information($"{emails.Emails.Count()} Retrieved");
            if (emails.Emails.Count() == 0)
            {
                return;
            }

            _emailAlertsDal = new EmailAlertsDal(_configuration, _logger);

            List<FullEmailModel> validEmails = saveAndValidateExtendedEmails(emails.Emails).ToList();
            if (validEmails.Count == 0)
            {
                _logger.Information("No valid emails to parse");
                return;
            }

            List<EmailForExtendedDto> validatedExtended = validateExtension(validEmails).ToList();

            if (validatedExtended.Count() == 0)
            {
                _logger.Information("All emails failed parse");
                return;
            }

            if (!createRatingMarkers(validatedExtended))
            {
                _logger.Information("Failed to create rating markers");
                return;
            }

            _logger.Information("Completed Highlight Sync");

        }



        protected IEnumerable<FullEmailModel> saveAndValidateExtendedEmails(IEnumerable<FullEmailModel> emails)
        {
            //Any emails to be saved to database

            List<FullEmailModel> validEmails = new List<FullEmailModel>();

            foreach (FullEmailModel email in emails)
            {
                if (email.From.ToLower() != _acceptedEmailAddress)
                {
                    if (!_emailHelper.MoveEmail(email.UniqueIdentifier, CommunicationLocationEnum.InvalidSender))
                    {
                        _logger.Error($"Could not send Email {email.UniqueIdentifier} to Invalid Sender Folder");
                    }
                    continue;
                }

                CRUDEnums.ActionResult actionResult = _emailAlertsDal.Put(email).GetAwaiter().GetResult();

                if (actionResult != CRUDEnums.ActionResult.Success)
                {
                    if (actionResult == CRUDEnums.ActionResult.Duplicate)
                    {
                        _logger.Information($"Full email {email.UniqueIdentifier} did not save due to duplication");
                        if (!_emailHelper.MoveEmail(email.UniqueIdentifier, CommunicationLocationEnum.Duplicate))
                        {
                            _logger.Error($"Could not send Email {email.UniqueIdentifier} to Duplicated Folder");
                        }
                    }
                    else
                    {
                        _logger.Error($"An error occured while saving full email {email.UniqueIdentifier} to the database Error Type {actionResult.ToString()}");
                        if (!_emailHelper.MoveEmail(email.UniqueIdentifier, CommunicationLocationEnum.Unparsed))
                        {
                            _logger.Error($"Could not send Email {email.UniqueIdentifier} to Unparsed Folder");
                        }
                    }
                    

                    continue;
                }
                validEmails.Add(email);
                if (!_emailHelper.DeleteBatch(new string[] { email.UniqueIdentifier }))
                {
                    _logger.Error($"Could not remove emails {email.UniqueIdentifier} from mailbox");
                }
            }
            return validEmails;
        }

        protected IEnumerable<EmailForExtendedDto> validateExtension(List<FullEmailModel> emails)
        {
            List<EmailForExtendedDto> emailForExtendedDtos = ExtendedInformationHelper.ExtendEmailInformation(emails).ToList();
            List<EmailForExtendedDto> validatedList = new List<EmailForExtendedDto>();
            _logger.Information($"{emailForExtendedDtos.Count()} Extended");
            foreach (EmailForExtendedDto emailForExtendedDto in emailForExtendedDtos)
            {
                if (emailForExtendedDto.DateStamp == null ||
                    string.IsNullOrWhiteSpace(emailForExtendedDto.Folder))
                {
                    _logger.Warning($"Invalid Data format for email {emailForExtendedDto.UniqueIdentifier}");
                    if (_emailAlertsDal.Post(emailForExtendedDto.UniqueIdentifier, (int)EmailParseErrorEnum.ParseError).GetAwaiter().GetResult() != CRUDEnums.ActionResult.Success)
                    {
                        _logger.Error($"Could not update email {emailForExtendedDto.UniqueIdentifier}");
                    }
                    continue;
                }
                validatedList.Add(emailForExtendedDto);
            }
            return validatedList;

        }

        protected bool createRatingMarkers(IEnumerable<EmailForExtendedDto> emails)
        {
            try
            {
                SiteDal siteDal = new SiteDal(_configuration, _logger);
                RatingMarkerDal ratingMarkerDal = new RatingMarkerDal(_configuration, _logger);
                int added = 0;
                int updated = 0;
                foreach (EmailForExtendedDto emailForExtendedDto in emails)
                {
                    int siteId = siteDal.GetSiteIdByExternalIdentifier(emailForExtendedDto.Folder).GetAwaiter().GetResult();
                    if (siteId > -1)
                    {
                        int ratingMarkerId = ratingMarkerDal.Put(RatingMarkerHelper.EmailForExtendedToRatingMarker(emailForExtendedDto, siteId)).GetAwaiter().GetResult();
                        if (ratingMarkerId < 0)
                        {
                            _logger.Error($"An error occured while saving rating marker to the database");
                            continue;
                        }
                        added++;
                        if (_emailAlertsDal.Post(emailForExtendedDto.UniqueIdentifier, ratingMarkerId).GetAwaiter().GetResult() != CRUDEnums.ActionResult.Success)
                        {
                            _logger.Error($"An error occured while updating Email Alerts in the database");
                            continue;
                        }
                        updated++;
                    }
                    else
                    {
                        _logger.Information($"Error linking Highlight Folder - {emailForExtendedDto.Folder}");
                    }
                }
                _logger.Information($"RatingMarkers added to the database {added}");
                _logger.Information($"EmailAlerts updated in the database {updated}");
                if (added != emails.Count())
                {
                    _logger.Warning($"RatingMarkers not added to the database {(emails.Count() - added)}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Saving RatingMarkers", ex);
                return false;
            }
            return true;

        }
    }
}
