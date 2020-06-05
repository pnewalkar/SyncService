using Dapper;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.Domain.Enums;
using Maintel.Icon.Portal.Domain.Interfaces;
using Maintel.Icon.Portal.Domain.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.Permissions.Dal
{
    public class AreaDal : ISiteDal
    {
        private const string cnSites_SELECT = "Maintel.Sites_SELECT";
        private const string cnSites_UPSERT = "Maintel.upsertArea";

        private readonly ILogger _logger;
        private readonly string _connectionString;

        public AreaDal(IConfiguration configuration,
                           ILogger logger)
        {
            _logger = logger;
            _connectionString = configuration["Data:PermissionsDatabaseConnectionString"];
        }
        /// <summary>
        /// Centralised Connection Property to reduce code complexity.
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                var sql = new SqlConnection(_connectionString);
                return sql;
            }
        }

        public async Task<SiteModel> Get(int id)
        {
            SiteModel site = new SiteModel();
            return site;
        }

        public async Task<IEnumerable<SiteForViewDto>> Get(string username)
        {
            //Not used by the permissions database
            IEnumerable<SiteForViewDto> sites = new List<SiteForViewDto>();
            return sites;
        }

        public async Task<SiteModel> Get(string requestingUsername, int id)
        {
            //Not used by the permissions database
            SiteModel site = new SiteModel();
            return site;
        }

        public async Task<CRUDEnums.ActionResult> Post(SiteModel siteModel)
        {
            //if (await Get(siteModel.Id) != null)
            //{
            //    return await insertSite(siteModel.Id);
            //};
            return CRUDEnums.ActionResult.DoesntExist;
        }

        public async Task<CRUDEnums.ActionResult> Post(int siteId, string externalIdentifier)
        {
            //if (await Get(siteId) != null)
            //{
            //    return await updateSite(siteId, externalIdentifier);
            //};
            return CRUDEnums.ActionResult.DoesntExist;
        }

        public async Task<CRUDEnums.ActionResult> Put(SiteModel siteModel)
        {
            //if (await Get(siteModel.Id) != null)
            //{
            //    return CRUDEnums.ActionResult.Duplicate;
            //}
            return await UpsertSite(siteModel);
        }

        public async Task<CRUDEnums.ActionResult> PutOrPost(IEnumerable<SiteModel> siteModels)
        {
            CRUDEnums.ActionResult actionResult = CRUDEnums.ActionResult.UnknownError;
            foreach (SiteModel site in siteModels)
            {
                var tempSite = await Get(site.Id);
                //if (tempSite != null && tempSite.Id == site.Id)
                //{
                //    actionResult = await updateSite(site);
                //}
                //else
                //{
                //    actionResult = await insertSite(site);
                //}
                if (actionResult != CRUDEnums.ActionResult.Success)
                {
                    return actionResult;
                }
            }

            return CRUDEnums.ActionResult.Success;
        }

        public DateTime GetLastAdded()
        {
            DateTime? lastUpdate = null;
            return (DateTime)lastUpdate;
        }

        public CRUDEnums.ActionResult BulkUpload(IEnumerable<SiteModel> siteModels)
        {
            foreach (SiteModel site in siteModels)
            {
                var rtn = UpsertSite(site);
            }
            return CRUDEnums.ActionResult.Success;
        }

        private async Task<CRUDEnums.ActionResult> UpsertSite(SiteModel site)
        {
            var dbArgs = new DynamicParameters();
            dbArgs.Add("@Id", site.Id);
            dbArgs.Add("@Parent", site.ParentAccountId);
            dbArgs.Add("@Name", site.AccountName);
            using (IDbConnection conn = Connection)
            {
                try
                {
                    conn.Open();
                    var result = await conn.QueryAsync<SiteModel>(cnSites_UPSERT, dbArgs, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    return CRUDEnums.ActionResult.UnknownError;
                }
                return CRUDEnums.ActionResult.Success;
            }
        }
    }
}
