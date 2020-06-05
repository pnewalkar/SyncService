using Maintel.Icon.Portal.SyncService.Highlight.Dtos;
using Maintel.Icon.Portal.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Maintel.Icon.Portal.Domain;

namespace Maintel.Icon.Portal.SyncService.Highlight.Helpers
{
    public static class ExtendedInformationHelper
    {

        public static IEnumerable<EmailForExtendedDto> ExtendEmailInformation(IEnumerable<FullEmailModel> fullEmailModels)
        {
            List<EmailForExtendedDto> emailForExtendedDtos = new List<EmailForExtendedDto>();
            foreach(FullEmailModel fullEmailModel in fullEmailModels)
            {
                emailForExtendedDtos.Add(ExtendEmailInformation(fullEmailModel));
            }
            return emailForExtendedDtos;
        }
        public static EmailForExtendedDto ExtendEmailInformation(FullEmailModel fullEmailModel)
        {
            EmailForExtendedDto returnValue = new EmailForExtendedDto()
            {
                DateSent = fullEmailModel.DateSent,
                Title = fullEmailModel.Title,
                UniqueIdentifier = fullEmailModel.UniqueIdentifier
            };

            returnValue.DateStamp = StripText(fullEmailModel.Body, "time : ").ConvertStringToKnownDateTime();
            returnValue.Folder = StripText(fullEmailModel.Body, "folder : ");
            returnValue.Location = StripText(fullEmailModel.Body, "location : ");
            returnValue.Problem = StripText(fullEmailModel.Body, "problem : ");
            returnValue.Report = StripText(fullEmailModel.Body, "report : ");
            returnValue.Name = StripText(fullEmailModel.Body, "name : ");
            returnValue.Tag = StripText(fullEmailModel.Body, "tagtext : ");

            return returnValue;

        }

        public static string StripText(string body, string text)
        {
            try
            {
                if (!body.ToLower().Contains(text.ToLower()))
                {
                    return string.Empty;
                }

                int start = (body.ToLower().IndexOf(text.ToLower()) + text.Length);
                int end = body.ToLower().IndexOf(Environment.NewLine, start);
                if (end == -1)
                {
                    end = body.ToLower().IndexOf("\n", start);
                }
                if (end == -1)
                {
                    return string.Empty;
                }
                return body.Substring(start, end - start).Trim();
            }
            catch
            {
                return string.Empty;
            }


        }
    }
}
