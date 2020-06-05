using Maintel.Icon.Portal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintel.Icon.Portal.SyncService.Dod.Helpers
{
    public static class EmailConversionHelper
    {
        public static IEnumerable<FullEmailModel>GenerateFakeEmails(int numOfEmails, string from){

            List<FullEmailModel> returnModels = new List<FullEmailModel>();
            for (int i=0; i< numOfEmails; i++)
            {
                returnModels.Add(new FullEmailModel()
                {
                    Body = createBody(DateTime.UtcNow.AddMinutes(-5).AddHours(-1)),
                    DateRecieved = DateTime.UtcNow,
                    DateSent = DateTime.UtcNow.AddHours(-1),
                    From = from,
                    Title = $"Fake Email Title {i}",
                    UniqueIdentifier = new Guid().ToString()
                });
            }
            return returnModels;
        }

        private static string createBody(DateTime dateGenerated)
        {
            string returnString = $"Time: {dateGenerated.ToString("HH:mm on MMMM dd yyyy")}{Environment.NewLine}";
            returnString += $"Folder: JD Sports » JD Stores -All » Sports Fashion Stores » JD - JD Sports » 0228 Beckton - JD{Environment.NewLine}";
            returnString += $"Location: 0228 Beckton - JD{Environment.NewLine}";
            returnString += $"Report: Primary | BT | DSL | HEX(02075409313){Environment.NewLine}";
            returnString += $"Problem: Outbound - Traffic - Volume - Red alert cleared{Environment.NewLine}";
            returnString += $"Name: 02075409313{Environment.NewLine}";
            returnString += $"TagText: 785702{Environment.NewLine}";
            return returnString;

        }
    }
}
