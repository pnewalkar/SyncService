using Maintel.Icon.Portal.API.Dal;
using Maintel.Icon.Portal.Domain;
using Maintel.Icon.Portal.Domain.Models;
using Maintel.Icon.Portal.SyncService.Highlight.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService.Dod.Helpers
{
    public static class StaticVariables
    {
        
        public static string ConnectionString { get; set; }
        public static DateTime? DateStatic { get; set; }
        public static int AccountId { get; set; }
        public static SiteModel Site { get; set; }
        public static IEnumerable<SiteModel> Sites { get; set; }
        public static IEnumerable<TicketModel> Tickets { get; set; }
        public static Mock<IConfiguration> Configuration { get; set; }
        public static Mock<ILogger> Logger { get; set; }
        public static EmailHelper StaticEmailHelper { get; set; }
        public static EmailAlertsDal StaticEmailAlertDal { get; set; }
        public static bool EmailServerConnected { get; set; }
        public static IEnumerable<FullEmailModel> StaticFullEmailModel { get; set; }
        public static EmailConnectionHelper EmailConnectionHelper { get; set; }


    }
}
