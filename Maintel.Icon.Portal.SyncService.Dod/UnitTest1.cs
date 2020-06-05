using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maintel.Icon.Portal.SyncService.Autotask.Helpers;
using Maintel.Icon.Portal.SyncService.Autotask.Models;
using System.Collections.Generic;
using System.Linq;
using Maintel.Icon.Portal.API.Dal;
using Maintel.Icon.Portal.Domain.Models;
using Moq;
using Microsoft.Extensions.Configuration;
using Maintel.Icon.Portal.Domain.Dtos;

namespace Maintel.Icon.Portal.SyncService.Dod
{
    [TestClass]
    public class UnitTest1
    {
        //private Mock<IConfiguration> _configuration = new Mock<IConfiguration>();

        //[TestInitialize]
        //public void Initialise()
        //{
        //    _configuration.Setup(a => a["Data:AutotaskDatabaseConnectionString"]).Returns("Server=.\\ICONPORTAL;Database=PortalTestDatabase;User Id=MaintelAdmin;Password=MaintelAdmin;");
        //}
        


        //[TestMethod]
        //public void GetAllSites()
        //{
        //    List<SitePOCO> sites = DatabaseHelper.GetAllSites().ToList();

        //    Assert.IsTrue(sites.Count > 0, "Failed");
        //}


        //[TestMethod]
        //public void GetUserSites()
        //{
        //    IEnumerable<SiteForViewDto> sites = new SiteDal(_configuration.Object).Get("testuser1").GetAwaiter().GetResult();

        //    Assert.IsTrue(sites.ToList().Count > 0, "Failed");
        //}


        //[TestMethod]
        //public void GetAllTickets() 
        //{
        //    List<TicketPOCO> tickets = DatabaseHelper.GetAllTickets().ToList();

        //    Assert.IsTrue(tickets.Count > 0, "Failed");
        //}

        //[TestMethod]
        //public void GetUpdatedSitesFromDatabase()
        //{
        //    List<SitePOCO> sites = DatabaseHelper.GetNewSites(new DateTime(2019,3,1)).ToList();
        //    Assert.IsTrue(sites.Count > 0, "Failed");
        //}


        //[TestMethod]
        //public void GetUpdatedSitesFromWebservice()
        //{
        //      SiteDal dal = new SiteDal(_configuration.Object);
        //    DateTime date = dal.GetLastAdded();
        //    List<SiteModel> sites = SOAPHelper.RetrieveNewSites(date).ToList();
        //    Assert.IsTrue(dal.PutOrPost(sites).GetAwaiter().GetResult() == Domain.Enums.CRUDEnums.ActionResult.Success, "OOPS");
        //}

        
        //[TestMethod]
        //public void GetUpdatedTicketsFromService()
        //{
        //    TicketDal dal = new TicketDal(_configuration.Object);
        //    DateTime date = dal.GetLastUpdated();
        //    List<TicketModel> tickets = SOAPHelper.RetriveUpdatedTickets(date).ToList();
        //    Assert.IsTrue (dal.PutOrPost(tickets).GetAwaiter().GetResult() == Domain.Enums.CRUDEnums.ActionResult.Success,"OOPS");
        //}



        //[TestMethod]
        //public void InsertAllSites()
        //{
        
        //    try
        //    {

        //        IEnumerable<SitePOCO> sites = DatabaseHelper.GetAllSites();


        //        if (sites != null && sites.Count() > 0)
        //        {
        //            IEnumerable<SiteModel> siteModels = from site in sites
        //                                                select new SiteModel()
        //                                                {
        //                                                    AccountName = site.account_name,
        //                                                    Address1 = site.address_1,
        //                                                    Address2 = site.address_2,
        //                                                    City = site.city,
        //                                                    Country = site.country,
        //                                                    DateCreated = site.create_time,
        //                                                    Id = site.account_id,
        //                                                    ParentAccountId = site.parent_account_id,
        //                                                    State = site.state,
        //                                                    ZipCode = site.zip_code
        //                                                };
        //            SiteDal dal = new SiteDal(_configuration.Object);
        //            dal.Truncate();
        //            dal.BulkUpload(siteModels);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var exs = ex;
        //    }




        //}

        //[TestMethod]
        //public void InsertAllTypes()
        //{
        //    IEnumerable<PriorityTypePOCO> priorityTypePOCOs = DatabaseHelper.GetPriorityType();
        //    Domain.Enums.CRUDEnums.ActionResult actionResult = Domain.Enums.CRUDEnums.ActionResult.Success;
        //    TypesDal dal = new TypesDal(_configuration.Object);

        //    if (priorityTypePOCOs != null && priorityTypePOCOs.Count() > 0)
        //    {
        //        IEnumerable<PriorityTypeModel> priorityTypes = from priority in priorityTypePOCOs
        //                                                       select new PriorityTypeModel()
        //                                                       {
        //                                                           Id = priority.Priority_Id,
        //                                                           IsActive = priority.Is_Active,
        //                                                           Name = priority.Priority_Name,
        //                                                           SortOrder = priority.Sort_Order
        //                                                       };

        //        actionResult = dal.AddOrReplacePriorityTypes(priorityTypes).GetAwaiter().GetResult();
        //    }



        //    IEnumerable<TicketTypePOCO> ticketTypePOCOs = DatabaseHelper.GetTicketType();

        //    if (ticketTypePOCOs != null && ticketTypePOCOs.Count() > 0)
        //    {
        //        IEnumerable<TicketTypeModel> ticketTypes = from ticketTypePOCO in ticketTypePOCOs
        //                                                   select new TicketTypeModel()
        //                                                   {
        //                                                       Id = ticketTypePOCO.Task_Type_Id,
        //                                                       Name = ticketTypePOCO.Task_Type_Name
        //                                                   };
        //        actionResult = dal.AddOrReplaceTicketType(ticketTypes).GetAwaiter().GetResult();
        //    }

        //    IEnumerable<TaskStatusPOCO> taskStatusPOCOs = DatabaseHelper.GetTaskStatus();

        //    if (taskStatusPOCOs != null && taskStatusPOCOs.Count() > 0)
        //    {
        //        IEnumerable<TaskStatusModel> taskStatuses = from taskStatusPOCO in taskStatusPOCOs
        //                                                    select new TaskStatusModel()
        //                                                    {
        //                                                        Id = taskStatusPOCO.Task_Status_Id,
        //                                                        IsActive = taskStatusPOCO.Is_Active,
        //                                                        IsSystem = taskStatusPOCO.Is_System,
        //                                                        ServiceLevelAgreementEventTypeCode = taskStatusPOCO.Service_Level_Agreement_Event_Type_Code,
        //                                                        SortOrder = taskStatusPOCO.Sort_Order,
        //                                                        TaskStatusName = taskStatusPOCO.Task_Status_Name
        //                                                    };
        //        actionResult = dal.AddOrReplaceTaskStatuss(taskStatuses).GetAwaiter().GetResult();
        //    }


        //}


        //[TestMethod]
        //public void InsertAllTickets()
        //{

                
        //    IEnumerable<TicketPOCO> tickets = DatabaseHelper.GetAllTickets();


        //    try
        //    {
        //        if (tickets != null && tickets.Count() > 0)
        //        {
        //            IEnumerable<TicketModel> ticketModels = from ticket in tickets
        //                                                    select new TicketModel()
        //                                                    {
        //                                                        AccountId = ticket.account_id,
        //                                                        CompleteDateTime = ticket.date_completed,
        //                                                        CreateDateTime = ticket.create_time,
        //                                                        Description = ticket.task_description,
        //                                                        DueDateTime = ticket.due_time,
        //                                                        Id = ticket.task_id,
        //                                                        LastActivityDateTime = ticket.last_activity_time,
        //                                                        PriorityId = ticket.priority_id,
        //                                                        StatusId = ticket.task_status_id,
        //                                                        TicketTypeId = ticket.ticket_type_id,
        //                                                        Title = ticket.task_name
        //                                                    };
        //            TicketDal dal = new TicketDal(_configuration.Object);
        //            dal.Truncate();
        //            dal.BulkUpload(ticketModels);

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        var exs = ex;
        //    }
           
        //}
    }
}
  