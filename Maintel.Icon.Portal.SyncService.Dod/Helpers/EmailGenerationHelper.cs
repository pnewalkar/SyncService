using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService.Dod.Helpers
{
    public static class EmailGenerationHelper
    {

        private static ExchangeService _exchangeService;
        public static void GenerateEmails(string to, string username, string password, int noOfEmails)
        {
            connectToExchangeService(username,password);
            for (int i = 0; i < noOfEmails; i++)
            {
                sendEmail($"Test Subject to email {to} - {i.ToString()}",
                          $"Test Body to email {to} - {i.ToString()}",
                          to);
            }

            System.Threading.Thread.Sleep(5000);
        }

        public static void ClearOutMailbox(string username, string password)
        {
            connectToExchangeService(username, password);
            List<Item> messages = RetrieveAllEmails();

            while (messages.Count > 0)
            {
                _exchangeService.DeleteItems(messages.Select(x => x.Id).ToList(),DeleteMode.HardDelete,SendCancellationsMode.SendToNone,AffectedTaskOccurrence.AllOccurrences);
                messages = RetrieveAllEmails();
            }
            System.Threading.Thread.Sleep(5000);
        }

        public static List<Item> RetrieveAllEmails()
        {
            FindItemsResults<Item> findResults = _exchangeService.FindItems(
             WellKnownFolderName.Inbox,
             new ItemView(100)
          );
            return findResults.Items.ToList();
        }

        private static void connectToExchangeService(string username, string password)
        {
            _exchangeService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            _exchangeService.Credentials = new WebCredentials(username, password);
            _exchangeService.AutodiscoverUrl(username, sSLRedirectionCallback);
        }


        private static void sendEmail(string subject, string body, string to)
        {
            EmailMessage message = new EmailMessage(_exchangeService);
            message.Subject = subject;
            message.Body = body;
            message.ToRecipients.Add(to);
            message.SendAndSaveCopy();
        }
        private static bool sSLRedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }

    }
}
