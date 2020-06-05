using Maintel.Icon.Portal.SyncService.Dod.Helpers;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;
using Maintel.Icon.Portal.SyncService.Autotask.Helpers;
using Maintel.Icon.Portal.SyncService.Autotask.Models;
using Maintel.Icon.Portal.Domain.Models;
using Maintel.Icon.Portal.Domain.Extensions;
using System.Linq;

namespace Maintel.Icon.Portal.SyncService.Dod.Steps
{
    [Binding]
    public class AutotaskAPIFeedSteps
    {
        private IConfigurationRoot _configuration;
        private string _autotaskApiUsername;
        private string _autotaskApiPassword;
        private string _autotaskApiIntegrationCode;
        private string _autotaskApiUrl;
        
        [Before]
        public void StartUp()
        {
            _configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: true)
             .Build();
            _autotaskApiUsername = _configuration["AutotaskAPI:Username"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_autotaskApiUsername), "Invalid configuration AutotaskAPI:Username");
            _autotaskApiPassword = _configuration["AutotaskAPI:Password"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_autotaskApiPassword), "Invalid configuration AutotaskAPI:Password");
            _autotaskApiIntegrationCode = _configuration["AutotaskAPI:IntegrationCode"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_autotaskApiIntegrationCode), "Invalid configuration AutotaskAPI:IntegrationCode");
            _autotaskApiUrl = _configuration["AutotaskAPI:Url"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_autotaskApiUrl), "Invalid configuration AutotaskAPI:Url");

        }

        [Given(@"I have a connection to the autotask API")]
        public void GivenIHaveAConnectionToTheAutotaskAPI()
        {
            //No implementation required
        }
        
        [When(@"I request details on a site known to exist")]
        public void WhenIRequestDetailsOnASiteKnownToExist()
        {
            int tempVal = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdExists"], out tempVal);
            StaticVariables.AccountId = tempVal;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdExists");
            StaticVariables.Site = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetrieveSiteInformation(StaticVariables.AccountId.ToString());

        }

        [When(@"I request details on a site known not to exist")]
        public void WhenIRequestDetailsOnASiteKnownNotToExist()
        {
            int tempVal = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdNotExists"], out tempVal);
            StaticVariables.AccountId = tempVal;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdNotExists");
            StaticVariables.Site = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetrieveSiteInformation(StaticVariables.AccountId.ToString());
        }

        [When(@"I request details on new sites")]
        public void WhenIRequestDetailsOnNewSites()
        {
            DateTime? date = _configuration["AutotaskVariables:DateWithNewAccountsAfter"].ConvertStringToKnownDateTime();

            Assert.IsNotNull(date, "Invalid configuration AutotaskVariables:DateWithNewAccountsAfter");

            StaticVariables.Sites = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetrieveNewSites((DateTime)date);

        }


        [When(@"I request details on a site known to have children")]
        public void WhenIRequestDetailsOnASiteKnownToHaveChildren()
        {
            int tempval = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdWithChildren"], out tempval);
            StaticVariables.AccountId = tempval;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdWithChildren");
            StaticVariables.Sites = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetrieveChildSites(StaticVariables.AccountId.ToString());
        }

        [When(@"I request details on a site known to not have children")]
        public void WhenIRequestDetailsOnASiteKnownToNotHaveChildren()
        {
            int tempVal = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdWithNoChildren"], out tempVal);
            StaticVariables.AccountId = tempVal;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdWithNoChildren");
            StaticVariables.Sites = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetrieveChildSites(StaticVariables.AccountId.ToString());
        }


        [When(@"I request details on a site known to have tickets")]
        public void WhenIRequestDetailsOnASiteKnownToHaveTickets()
        {
            int tempVal = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdWithTickets"], out tempVal);
            StaticVariables.AccountId = tempVal;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdWithTickets");
            StaticVariables.Tickets = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetriveTicketsByAccount(StaticVariables.AccountId.ToString());
        }
        
        [When(@"I request details on a site known not to have tickets")]
        public void WhenIRequestDetailsOnASiteKnownNotToHaveTickets()
        {
            int tempVal = -1;
            int.TryParse(_configuration["AutotaskVariables:AccountIdWithNoTickets"], out tempVal);
            StaticVariables.AccountId = tempVal;
            Assert.AreNotEqual(-1, StaticVariables.AccountId, "Invalid configuration AutotaskVariables:AccountIdWithNoTickets");
            StaticVariables.Tickets = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, StaticVariables.Logger.Object).RetriveTicketsByAccount(StaticVariables.AccountId.ToString());
        }
        
        [Then(@"A ""(.*)"" should have some results")]
        public void ThenAShouldHaveSomeResults(string objectType)
        {
            switch (objectType.ToLower())
            {
                case "sitemodel":
                    {
                        Assert.AreNotEqual(StaticVariables.Site.Id, 0, "No Site Returned");
                        break;
                    }
                case "ticketmodel":
                    {
                        break;
                    }
                case "list<sitemodel>":
                    {
                        Assert.AreNotEqual(StaticVariables.Sites.ToList().Count, 0, "No Sites Returned");
                        break;
                    }
                case "list<ticketmodel>":
                    {
                        Assert.AreNotEqual(StaticVariables.Sites.ToList().Count, 0, "No Tickets Returned");
                        break;
                    }
            }
        }
        
        [Then(@"A ""(.*)"" should have no results")]
        public void ThenAShouldHaveNoResults(string objectType)
        {
            switch (objectType.ToLower())
            {
                case "sitemodel":
                    {
                        Assert.AreEqual(StaticVariables.Site.Id == 0, "Site Returned");
                        break;
                    }
                case "ticketmodel":
                    {
                        break;
                    }
                case "list<sitemodel>":
                    {
                        Assert.AreEqual(StaticVariables.Sites.ToList().Count, 0, "Some Sites Returned");
                        break;
                    }
                case "list<ticketmodel>":
                    {
                        Assert.AreEqual(StaticVariables.Sites.ToList().Count, 0, "Some Tickets Returned");
                        break;
                    }
            }
        }
    }
}
