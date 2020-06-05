using Maintel.Icon.Portal.Domain;
using Maintel.Icon.Portal.Domain.Dtos;
using Maintel.Icon.Portal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService.Highlight.Interfaces
{
    public interface IEmailHelper
    {
        bool ConnectToEmailServer();
        Task<FullEmailListForRetrieveDto> RetrieveEmailBatch(int batchSize);
        bool DeleteBatch(IEnumerable<string> emailIds);
        bool MoveEmail(string id, CommunicationLocationEnum communicationLocationEnum);

    }
}
