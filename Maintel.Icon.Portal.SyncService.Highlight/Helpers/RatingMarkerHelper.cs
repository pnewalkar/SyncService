using Maintel.Icon.Portal.Domain.Models;
using Maintel.Icon.Portal.SyncService.Highlight.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maintel.Icon.Portal.SyncService.Highlight.Helpers
{
    public static class RatingMarkerHelper
    {
        public static RatingMarkerModel EmailForExtendedToRatingMarker(EmailForExtendedDto emailForExtendedDto, int siteId)
        {
            RatingMarkerModel ratingMarker = new RatingMarkerModel();

            ratingMarker.SiteId = siteId;
            ratingMarker.DateAdded = DateTime.UtcNow;
            ratingMarker.DateRaised =  emailForExtendedDto.DateStamp.Value;
            ratingMarker.Raised = emailForExtendedDto.Title.ToLower().Contains(" alert raised - ");
            if (emailForExtendedDto.Title.ToLower().Contains(" - red alert "))
            {
                ratingMarker.Rating = Domain.Enums.RatingMarkerEnum.Red;
            }
            else if (emailForExtendedDto.Title.ToLower().Contains(" - amber alert "))
            {
                ratingMarker.Rating = Domain.Enums.RatingMarkerEnum.Amber;
            }
            else if (emailForExtendedDto.Title.ToLower().Contains(" - green alert "))
            {
                ratingMarker.Rating = Domain.Enums.RatingMarkerEnum.Green;
            }
            else
            {
                ratingMarker.Rating = Domain.Enums.RatingMarkerEnum.Unknown;
            }
            return ratingMarker;

        }
    }
}
