
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Maintel.Icon.Portal.Domain.Dtos;

namespace Maintel.Icon.Portal.SyncService.Highlight.Helpers
{

    public class AssociationHelper
    {
        public Dictionary<string,int> SiteAssociation;
        private readonly ILogger _logger;
        public AssociationHelper(IEnumerable<SiteForExtendedTicketDto> sites, ILogger logger)
        {
            _logger = logger;
            SiteAssociation = new Dictionary<string, int>();
            foreach (SiteForExtendedTicketDto site in sites)
            {
                string folder = ExtendedInformationHelper.StripText(site.Description, "folder :");
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    if (SiteAssociation.ContainsKey(folder.ToLower()))
                    {
                        _logger.Warning($"Folder : {folder} is already associated with siteId {SiteAssociation[folder.ToLower()]} and cannot be associated with {site.Id}");
                        continue;

                    }
                    SiteAssociation.Add(folder.ToLower(), site.Id);
                }
            }
        }
        public int GetAssociatedSite(string ticketBody)
        {
            string folder = ExtendedInformationHelper.StripText(ticketBody, "folder :");
            if (!string.IsNullOrWhiteSpace(folder) && 
                            !SiteAssociation.ContainsKey(folder.ToLower())){
                return SiteAssociation[folder.ToLower()];
            }
            return -1;
        }


    }
}
