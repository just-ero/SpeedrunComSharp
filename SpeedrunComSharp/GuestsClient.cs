﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SpeedrunComSharp
{
    public class GuestsClient
    {
        public const string Name = "guests";

        private SpeedrunComClient baseClient;

        public GuestsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetGuestsUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("{0}{1}", Name, subUri));
        }

        public Guest GetGuestFromSiteUri(string siteUri)
        {
            var id = GetGuestIDFromSiteUri(siteUri);

            if (string.IsNullOrEmpty(id))
                return null;

            return GetGuest(id);
        }

        public string GetGuestIDFromSiteUri(string siteUri)
        {
            var elementDescription = SpeedrunComClient.GetElementDescriptionFromSiteUri(siteUri);

            if (elementDescription == null
                || elementDescription.Type != ElementType.Guest)
                return null;

            return elementDescription.ID;
        }

        public Guest GetGuest(string guestName)
        {
            var uri = GetGuestsUri(string.Format("/{0}", Uri.EscapeDataString(guestName)));
            var result = baseClient.DoRequest(uri);

            return Guest.Parse(baseClient, result.data);
        }
    }
}
