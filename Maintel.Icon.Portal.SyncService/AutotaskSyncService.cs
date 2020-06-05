using Maintel.Icon.Portal.API.Dal;
using Maintel.Icon.Portal.Domain.Models;
using Maintel.Icon.Portal.SyncService.Autotask.Helpers;
using Maintel.Icon.Portal.SyncService.Autotask.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService
{
    public class AutotaskSyncService
    {

        private bool _sitesInitialised;
        private bool _ticketsInitialised;
        private bool _typesInitialised;

        private string _remoteAutoTaskDatabaseConnection = "";
        private int _taskTypeId = 2;
        private int _accountTypeId = 1;


        private string _autotaskApiUsername;
        private string _autotaskApiPassword;
        private string _autotaskApiIntegrationCode;
        private string _autotaskApiUrl;

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AutotaskSyncService(ILogger logger, IConfiguration configuration)
        {


            _configuration = configuration;
            _logger = logger;

            //Set initialisation for each object type, if set to false the database will be truncated and re-populated from the autotask source database
            _sitesInitialised = true;
            _ticketsInitialised = true;
            _typesInitialised = true;

            _remoteAutoTaskDatabaseConnection = _configuration["Data:RemoteAutotaskDatabaseConnectionString"];
            int.TryParse(_configuration["AutotaskVariables:TaskTypeId"], out _taskTypeId);
            int.TryParse(_configuration["AutotaskVariables:AccountTypeId"], out _accountTypeId);

            _autotaskApiUsername = _configuration["AutotaskAPI:Username"];
            _autotaskApiPassword = _configuration["AutotaskAPI:Password"];
            _autotaskApiIntegrationCode = _configuration["AutotaskAPI:IntegrationCode"];
            _autotaskApiUrl = _configuration["AutotaskAPI:Url"];

            _logger.Information($"Started with TaskType Filter : {_taskTypeId}, AccountType Filter : {_accountTypeId}");
        }

        public void Sync()
        {
            _logger.Information("Starting Autotask Sync");
            if (!_typesInitialised)
            {
                InitialiseTypes();
                _typesInitialised = true;
            }
            if (!_sitesInitialised)
            {
                InitialiseSites();
                _sitesInitialised = true;
            }
            else
            {
                UpdateSites();
            }

            if (!_ticketsInitialised)
            {
                InitialiseTickets();
                _ticketsInitialised = true;
            }
            else
            {
                UpdateTickets();
            }
            _logger.Information("Completed Autotask Sync");
        }

        private void InitialiseTypes()
        {
            _logger.Information("Initialising Types");
            DatabaseHelper databaseHelper = new DatabaseHelper(_remoteAutoTaskDatabaseConnection);
            Domain.Enums.CRUDEnums.ActionResult actionResult = Domain.Enums.CRUDEnums.ActionResult.Success;
            TypesDal dal = new TypesDal(_configuration);
            try
            {
                IEnumerable<PriorityTypePOCO> priorityTypePOCOs = databaseHelper.GetPriorityType();
                if (priorityTypePOCOs != null && priorityTypePOCOs.Count() > 0)
                {
                    _logger.Information($"{priorityTypePOCOs.Count()} Priority Types Found");
                    IEnumerable<PriorityTypeModel> priorityTypes = from priority in priorityTypePOCOs
                                                                   select new PriorityTypeModel()
                                                                   {
                                                                       Id = priority.Priority_Id,
                                                                       IsActive = priority.Is_Active,
                                                                       Name = priority.Priority_Name,
                                                                       SortOrder = priority.Sort_Order
                                                                   };

                    actionResult = dal.AddOrReplacePriorityTypes(priorityTypes).GetAwaiter().GetResult();
                    _logger.Information("Priority Types Inserted");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed intialising Priority Types");
            }

            try
            {
                IEnumerable<TicketTypePOCO> ticketTypePOCOs = databaseHelper.GetTicketType();

                if (ticketTypePOCOs != null && ticketTypePOCOs.Count() > 0)
                {
                    _logger.Information($"{ticketTypePOCOs.Count()} Ticket Types Found");
                    IEnumerable<TicketTypeModel> ticketTypes = from ticketTypePOCO in ticketTypePOCOs
                                                               select new TicketTypeModel()
                                                               {
                                                                   Id = ticketTypePOCO.Task_Type_Id,
                                                                   Name = ticketTypePOCO.Task_Type_Name
                                                               };
                    actionResult = dal.AddOrReplaceTicketType(ticketTypes).GetAwaiter().GetResult();
                    _logger.Information("Ticket Types Inserted");
                }

            }


            catch (Exception ex)
            {
                _logger.Error(ex, "Failed intialising Ticket Types");
            }


            try
            {
                IEnumerable<TaskStatusPOCO> taskStatusPOCOs = databaseHelper.GetTaskStatus();

                if (taskStatusPOCOs != null && taskStatusPOCOs.Count() > 0)
                {
                    _logger.Information($"{taskStatusPOCOs.Count()} Task Statuses Found");
                    IEnumerable<TaskStatusModel> taskStatuses = from taskStatusPOCO in taskStatusPOCOs
                                                                select new TaskStatusModel()
                                                                {
                                                                    Id = taskStatusPOCO.Task_Status_Id,
                                                                    IsActive = taskStatusPOCO.Is_Active,
                                                                    IsSystem = taskStatusPOCO.Is_System,
                                                                    ServiceLevelAgreementEventTypeCode = taskStatusPOCO.Service_Level_Agreement_Event_Type_Code,
                                                                    SortOrder = taskStatusPOCO.Sort_Order,
                                                                    TaskStatusName = taskStatusPOCO.Task_Status_Name
                                                                };
                    actionResult = dal.AddOrReplaceTaskStatuss(taskStatuses).GetAwaiter().GetResult();
                    _logger.Information("Task Statuses Inserted");
                }
            }

            catch (Exception ex)
            {
                _logger.Error(ex, "Failed intialising Task Statuses");
            }
            databaseHelper = null;

        }

        private void InitialiseTickets()
        {
            _logger.Information("Initialising Tickets");
            IEnumerable<TicketPOCO> tickets = new List<TicketPOCO>();

            try
            {
                tickets = new DatabaseHelper(_remoteAutoTaskDatabaseConnection).GetAllTickets(_taskTypeId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed intialising Tickets");
            }

            if (tickets.Count() > 0)
            {
                _logger.Information($"{tickets.Count()} Tickets found");
                try
                {
                    _logger.Information($"Inserting Tickets");
                    IEnumerable<TicketModel> ticketModels = from ticket in tickets
                                                            select new TicketModel()
                                                            {
                                                                AccountId = ticket.account_id,
                                                                CompleteDateTime = ticket.date_completed,
                                                                CreateDateTime = ticket.create_time,
                                                                Description = ticket.task_description,
                                                                DueDateTime = ticket.due_time,
                                                                Id = ticket.task_id,
                                                                LastActivityDateTime = ticket.last_activity_time,
                                                                PriorityId = ticket.priority_id,
                                                                StatusId = ticket.task_status_id,
                                                                TicketTypeId = ticket.ticket_type_id,
                                                                Title = ticket.task_name
                                                            };


                    TicketDal dal = new TicketDal(_configuration, _logger);
                    dal.Truncate();
                    dal.BulkUpload(ticketModels);
                    _logger.Information($"Completed Tickets Bulk insert");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed Bulk upload of tickets");
                }
            }


        }

        private void UpdateTickets()
        {
            _logger.Information("Updating Tickets");
            TicketDal dal = new TicketDal(_configuration, _logger);
            DateTime? lastUpdate = dal.GetLastUpdated();
            if (lastUpdate == null || lastUpdate.Value.Year == 0)
            {
                _logger.Information("No tickets found trying initalisation");
                InitialiseTickets();
            }
            else
            {
                IEnumerable<TicketModel> ticketModels = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, _logger)
                                                    .RetriveUpdatedTickets(lastUpdate.Value);
                if (ticketModels != null &&
                            ticketModels.Count() > 0)
                {
                    _logger.Information($"{ticketModels.Count()} updated Tickets found");
                    Domain.Enums.CRUDEnums.ActionResult actionResult = dal.PutOrPost(ticketModels).GetAwaiter().GetResult();
                }
            }

        }

        private void InitialiseSites()
        {
            _logger.Information("Initialising Sites");
            IEnumerable<SitePOCO> sites = new List<SitePOCO>();

            try
            {
                sites = new DatabaseHelper(_remoteAutoTaskDatabaseConnection).GetAllSites(_accountTypeId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed intialising Sites");
            }



            if (sites.Count() > 0)
            {
                _logger.Information($"{sites.Count()} Sites Found");
                try
                {
                    _logger.Information($"Uploading Sites");
                    IEnumerable<SiteModel> siteModels = from site in sites
                                                        select new SiteModel()
                                                        {
                                                            AccountName = site.account_name,
                                                            Address1 = site.address_1,
                                                            Address2 = site.address_2,
                                                            City = site.city,
                                                            Country = site.country,
                                                            DateCreated = site.create_time,
                                                            Id = site.account_id,
                                                            ParentAccountId = site.parent_account_id,
                                                            State = site.state,
                                                            ZipCode = site.zip_code
                                                        };
                    SiteDal dal = new SiteDal(_configuration, _logger);
                    dal.Truncate();
                    dal.BulkUpload(siteModels);
                    _logger.Information($"Completed Sites Bulk insert");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed Bulk upload of sites");
                }
            }


        }
        private void UpdateSites()
        {
            _logger.Information("Updating Sites");
            SiteDal dal = new SiteDal(_configuration, _logger);
            DateTime? lastUpdate = dal.GetLastAdded();
            if (lastUpdate == null || lastUpdate.Value.Year == 0)
            {
                InitialiseSites();
            }
            else
            {
                IEnumerable<SiteModel> siteModels = new SOAPHelper(_autotaskApiUsername, _autotaskApiPassword, _autotaskApiIntegrationCode, _autotaskApiUrl, _logger)
                                            .RetrieveNewSites(lastUpdate.Value);
                if (siteModels != null &&
                            siteModels.Count() > 0)
                    _logger.Information($"{siteModels.Count()} new Sites found");
                {
                    foreach (SiteModel site in siteModels)
                    {
                        Domain.Enums.CRUDEnums.ActionResult actionResult = dal.Put(site).GetAwaiter().GetResult();
                    }
                }
            }

        }

    }
}
