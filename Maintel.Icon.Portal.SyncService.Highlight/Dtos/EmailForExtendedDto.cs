using System;
using System.Collections.Generic;
using System.Text;

namespace Maintel.Icon.Portal.SyncService.Highlight.Dtos
{
    public class EmailForExtendedDto
    {
        public DateTime DateSent{ get; set; }
        public DateTime? DateStamp { get; set; }
        public string Folder { get; set; }
        public string Location { get; set; }
        public string Report { get; set; }
        public string Problem { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string UniqueIdentifier { get; set; }
    }
}
