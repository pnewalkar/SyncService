using Maintel.Icon.Portal.API.Dal;
using Maintel.Icon.Portal.Domain;
using Maintel.Icon.Portal.Domain.Enums;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.SyncService.Dod.Helpers;
using Maintel.Icon.Portal.SyncService.Highlight.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace Maintel.Icon.Portal.SyncService.Dod.Steps
{
    [Binding]
    public class HighlightEmailFeedSteps
    {

        private IConfigurationRoot _testConfiguration;
        
        private string _username;
        private string _password;
        private string _invalidUsername;
        private string _invalidPassword;
        private string _validFromEmailAddress;
        private string _invalidFromEmailAddress;
        private string _testValidEmailAccount;
        private string _testValidEmailPassword;
        private string _testInvalidEmailAccount;
        private string _testInvalidEmailPassword;
        private string _highlightDatabaseConnection;
        private string _emailConnectionHelperUrl;

        [Before]
        public void StartUp()
        {
            _testConfiguration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: true)
             .Build();
            _username = _testConfiguration["HighlightMailbox:Username"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_username), "Invalid configuration Highlight:Username");
            _password = _testConfiguration["HighlightMailbox:Password"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_password), "Invalid configuration HighlightMailbox:Password");
            _invalidUsername = _testConfiguration["HighlightMailbox:InvalidUsername"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_invalidUsername), "Invalid configuration Highlight:InvalidUsername");
            _invalidPassword= _testConfiguration["HighlightMailbox:InvalidPassword"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_invalidPassword), "Invalid configuration HighlightMailbox:InvalidPassword");
            _validFromEmailAddress = _testConfiguration["HighlightMailbox:ValidFromEmailAddress"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_validFromEmailAddress), "Invalid configuration HighlightMailbox:ValidFromEmailAddress");
            _invalidFromEmailAddress = _testConfiguration["HighlightMailbox:InvalidFromEmailAddress"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_invalidFromEmailAddress), "Invalid configuration HighlightMailbox:InvalidFromEmailAddress");


            _testValidEmailAccount = _testConfiguration["HighlightMailbox:TestValidEmailAccount"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_testValidEmailAccount), "Invalid configuration Highlight:TestValidEmailAccount");
            _testValidEmailPassword = _testConfiguration["HighlightMailbox:TestValidEmailPassword"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_testValidEmailPassword), "Invalid configuration Highlight:TestValidEmailPassword");

            _testInvalidEmailAccount = _testConfiguration["HighlightMailbox:TestInvalidEmailAccount"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_testInvalidEmailAccount), "Invalid configuration Highlight:TestValidEmailAccount");
            _testInvalidEmailPassword = _testConfiguration["HighlightMailbox:TestInvalidEmailPassword"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_testInvalidEmailPassword), "Invalid configuration Highlight:TestInvalidEmailPassword");

            _highlightDatabaseConnection = _testConfiguration["Data:HighlightDatabaseConnectionString"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_highlightDatabaseConnection), "Invalid configuration Data:HighlightDatabaseConnectionString");

            _emailConnectionHelperUrl = _testConfiguration["HighlightMailbox:communicationApi"];
            Assert.IsFalse(string.IsNullOrWhiteSpace(_emailConnectionHelperUrl), "Invalid configuration HighlightMailbox:communicationApi");


            StaticVariables.Logger = new Mock<ILogger>();
            StaticVariables.Configuration = new Mock<IConfiguration>();
            EmailGenerationHelper.ClearOutMailbox(_username,_password);
            new DatabaseHelper(_highlightDatabaseConnection).ClearTable("Highlight.EmailAlerts");

        }
        [Given(@"I have a ""(.*)"" username and ""(.*)"" password")]
        public void GivenIHaveAUsernameAndPassword(string validUsername, string validPassword)
        {

            StaticVariables.Configuration.Setup(a => a["email:username"]).Returns(validUsername.ToLower() == "valid" ? _username : _invalidUsername);
            StaticVariables.Configuration.Setup(a => a["email:password"]).Returns(validPassword.ToLower() == "valid" ? _password : _invalidPassword);
        }

        [Given(@"I have a valid connection to my email provider")]
        public void GivenIHaveAValidConnectionToMyEmailProvider()
        {
            StaticVariables.Configuration.Setup(a => a["email:username"]).Returns(_username);
            StaticVariables.Configuration.Setup(a => a["email:password"]).Returns(_password);
            StaticVariables.Configuration.Setup(a => a["email:deleteToDeletedItems"]).Returns("false");
            StaticVariables.Configuration.Setup(a => a["email:acceptedEmailAddress"]).Returns(_validFromEmailAddress);
            StaticVariables.StaticEmailHelper = new EmailHelper(StaticVariables.Configuration.Object, StaticVariables.Logger.Object);

        }
        [Given(@"I have a valid connection to my database")]
        public void GivenIHaveAValidConnectionToMyDatabase()
        {
            StaticVariables.Configuration.Setup(a => a["Data:HighlightDatabaseConnectionString"]).Returns(_highlightDatabaseConnection);
            StaticVariables.StaticEmailAlertDal = new EmailAlertsDal(StaticVariables.Configuration.Object, StaticVariables.Logger.Object);
        }


        [Given(@"there ""(.*)"" emails recieved from a ""(.*)"" email address")]
        public void GivenThereEmailsRecievedFromAEmailAddress(int noOfEmails, string valid)
        {
            if (valid.ToLower() == "valid")
            {
                EmailGenerationHelper.GenerateEmails(_username, _testValidEmailAccount, _testValidEmailPassword, noOfEmails);
            }
            else
            {
                EmailGenerationHelper.GenerateEmails(_username, _testInvalidEmailAccount, _testInvalidEmailPassword, noOfEmails);
            }
        }
        
        [When(@"I connect to my email provider")]
        public void WhenIConnectToMyEmailProvider()
        {
            StaticVariables.StaticEmailHelper = new EmailHelper(StaticVariables.Configuration.Object, StaticVariables.Logger.Object);
            StaticVariables.EmailServerConnected = StaticVariables.StaticEmailHelper.ConnectToEmailServer();
        }
        
        [When(@"I try to recieve emails")]
        public void WhenITryToRecieveEmails()
        {
            StaticVariables.StaticEmailHelper = new EmailHelper(StaticVariables.Configuration.Object, StaticVariables.Logger.Object);
            StaticVariables.EmailServerConnected = StaticVariables.StaticEmailHelper.ConnectToEmailServer();
            Assert.IsTrue(StaticVariables.EmailServerConnected, "Could not connect to Email Server");
        }
        
        [Then(@"a connection to the email provider ""(.*)"" made")]
        public void ThenAConnectionToTheEmailProviderMade(string isOrIsNot)
        {
            if (isOrIsNot.ToLower() == "is")
            {
                Assert.IsTrue(StaticVariables.EmailServerConnected, "Could not connect to email server");
            }
            else
            {
                Assert.IsFalse(StaticVariables.EmailServerConnected, "Invalid Connection made to email server");
            };
        }
        
        [Then(@"""(.*)"" new emails are receieved")]
        public void ThenNewEmailsAreReceieved(int numOfEmails)
        {
            FullEmailListForRetrieveDto emails = StaticVariables.StaticEmailHelper.RetrieveEmailBatch(100).GetAwaiter().GetResult();
            Assert.AreEqual(numOfEmails, emails.Emails.Count());
        }

        [Then(@"""(.*)"" new emails are receieved from my email api")]
        public void ThenNewEmailsAreReceievedFromMyEmailApi(int numOfEmails)
        {
            FullEmailListForRetrieveDto emails = StaticVariables.EmailConnectionHelper.RetrieveEmailBatch(100).GetAwaiter().GetResult();
            Assert.AreEqual(numOfEmails, emails.Emails.Count());
        }


        [When(@"I run the receive add to database process")]
        public void WhenIRunTheReceiveAddToDatabaseProcess()
        {
            Assert.IsTrue(StaticVariables.StaticEmailHelper.ConnectToEmailServer(), "Could not connect to email server");
            FullEmailListForRetrieveDto emails = StaticVariables.StaticEmailHelper.RetrieveEmailBatch(100).GetAwaiter().GetResult();
            //Any emails to be saved to database
            CRUDEnums.ActionResult actionResult = StaticVariables.StaticEmailAlertDal.Put(emails.Emails.ToList()).GetAwaiter().GetResult();
            Assert.AreEqual(CRUDEnums.ActionResult.Success, actionResult, "Error adding to database");

        }

        [Then(@"""(.*)"" new ""(.*)"" are in the database")]
        public void ThenNewAreInTheDatabase(int numberOfRecords, string objectType)
        {
            switch (objectType.ToLower())
            {
                case "emailalerts":
                    {
                        Assert.AreEqual(numberOfRecords, new DatabaseHelper(_highlightDatabaseConnection).NumberOfRecords("SELECT Count(*) FROM Highlight.EmailAlerts"), "An invalid number of records were returned");
                        break;
                    }
            }
        }

        [Given(@"I have a ""(.*)"" of ""(.*)"" objects")]
        public void GivenIHaveAOfObjects(int number, string objectType)
        {
            switch(objectType.ToLower())
            {
                case "fullemail":
                    {
                        StaticVariables.StaticFullEmailModel = EmailConversionHelper.GenerateFakeEmails(number, _validFromEmailAddress);
                        break;
                    }
            }
            

        }

        [When(@"I run the email converstion process")]
        public void WhenIRunTheEmailConverstionProcess()
        {
            
        }

        [When(@"I run the add to ratingsmarker process")]
        public void WhenIRunTheAddToRatingsmarkerProcess()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have a valid connection to my email api")]
        public void GivenIHaveAValidConnectionToMyEmailApi()
        {
            StaticVariables.EmailConnectionHelper = new EmailConnectionHelper(_emailConnectionHelperUrl);
        }

        [When(@"I try to recieve emails from my email api")]
        public void WhenITryToRecieveEmailsFromMyEmailApi()
        {
            Assert.IsTrue(StaticVariables.EmailConnectionHelper.ConnectToEmailServer(), "Could not connect");
        }


    }
}
