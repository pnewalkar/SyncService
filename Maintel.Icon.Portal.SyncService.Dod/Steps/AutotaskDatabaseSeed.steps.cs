using Maintel.Icon.Portal.SyncService.Autotask.Helpers;
using Maintel.Icon.Portal.SyncService.Autotask.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TechTalk.SpecFlow;

namespace Maintel.Icon.Portal.SyncService.Dod.Steps
{
    [Binding]
    public class AutotaskDatabaseSeedSteps
    {
        private IConfigurationRoot _configuration;
        
        [Before]
        public void StartUp()
        {
            _configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: true)
             .Build();
            Helpers.StaticVariables.ConnectionString = _configuration["Data:RemoteAutotaskDatabaseConnectionString"];
        }

        [Given(@"I have a valid connection to the Autotask database")]
        public void GivenIHaveAValidConnectionToTheAutotaskDatabase()
        {
            SqlConnection sql = new SqlConnection(Helpers.StaticVariables.ConnectionString);
            try
            {
                sql.Open();
            }
            catch
            {
            }

            Assert.AreEqual(sql.State, System.Data.ConnectionState.Open, "Connection could not be made");
        }
        
        [When(@"I request an initialisation of ""(.*)""")]
        public void WhenIRequestAnInitialisationOf(string objectType)
        {
            //No code required
        }
        
        [When(@"I query based on the ""(.*)""")]
        public  void WhenIQueryBasedOnThe(string timeframe)
        {
            switch (timeframe)
            {
                case "far past":
                    {
                        Helpers.StaticVariables.DateStatic = DateTime.UtcNow.AddYears(-5);
                        break;
                    }
                case "future":
                    {
                        Helpers.StaticVariables.DateStatic = DateTime.UtcNow.AddDays(10);
                        break;
                    }
                default:
                    {
                        Helpers.StaticVariables.DateStatic = null;
                        break;
                    }
            }
        }

        [Then(@"An empty collection of ""(.*)"" are returned")]
        public void ThenAnEmptyCollectionOfAreReturned(string objectType)
        {
            Assert.AreEqual(0, retrieveItems(objectType), "Invalid Number of results returned");
        }


        [Then(@"A collection of ""(.*)"" are returned")]
        public void ThenACollectionOfAreReturned(string objectType)
        {
            Assert.AreEqual(2, retrieveItems(objectType), "Invalid Number of results returned");
        }

        private int retrieveItems(string objectType)
        { 
            int taskTypeId;
            int accountTypeId;

            DatabaseHelper databaseHelper = new DatabaseHelper(_configuration["Data:RemoteAutotaskDatabaseConnectionString"], true);

            switch (objectType)
            {
                case "sites":
                    {
                        int.TryParse(_configuration["AutotaskVariables:AccountTypeId"], out accountTypeId);
                        Assert.IsTrue(accountTypeId > 0, "Invalid account type");
                        if (Helpers.StaticVariables.DateStatic == null)
                        {
                            var sites = databaseHelper.GetAllSites(accountTypeId);
                            Assert.IsTrue(sites.GetType() == typeof(List<SitePOCO>), "Invalid Site Type Returned");
                            return sites.ToList().Count();
                        }
                        else
                        {
                            var sites = databaseHelper.GetNewSites((DateTime)Helpers.StaticVariables.DateStatic, accountTypeId);
                            Assert.IsTrue(sites.GetType() == typeof(List<SitePOCO>), "Invalid Site Type Returned");
                            return sites.ToList().Count();
                        }
                    }
                case "tickets":
                    {
                        int.TryParse(_configuration["AutotaskVariables:TaskTypeId"], out taskTypeId);
                        Assert.IsTrue(taskTypeId > 0, "Invalid task type");
                        if (Helpers.StaticVariables.DateStatic == null)
                        {
                            var tickets = databaseHelper.GetAllTickets(taskTypeId);
                            Assert.IsTrue(tickets.GetType() == typeof(List<TicketPOCO>), "Invalid Ticket Type Returned");
                            return tickets.ToList().Count();
                        }
                        else
                        {
                            var tickets = databaseHelper.GetUpdatedTickets((DateTime)Helpers.StaticVariables.DateStatic, taskTypeId);
                            Assert.IsTrue(tickets.GetType() == typeof(List<TicketPOCO>), "Invalid Ticket Type Returned");
                            return tickets.ToList().Count();
                        }
                    }
                case "statustype":
                    {
                       
                        var statusType = databaseHelper.GetTaskStatus();
                        Assert.IsTrue(statusType.GetType() == typeof(List<TaskStatusPOCO>), "Invalid Task Status Type Returned");
                        return statusType.ToList().Count();
                    }
                case "priority":
                    {
                        var priorityType = databaseHelper.GetTaskStatus();
                        Assert.IsTrue(priorityType.GetType() == typeof(List<TaskStatusPOCO>), "Invalid Task Status Type Returned");
                        return priorityType.ToList().Count();
                    }
                case "taskstatus":
                    {
                        var taskType = databaseHelper.GetTaskStatus();
                        Assert.IsTrue(taskType.GetType() == typeof(List<TaskStatusPOCO>), "Invalid Task Status Type Returned");
                        return taskType.ToList().Count();
                    }
            }
            return -1;
        }
    }
}
