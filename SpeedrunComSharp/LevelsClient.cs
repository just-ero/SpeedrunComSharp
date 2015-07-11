﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

namespace SpeedrunComSharp
{
    public class LevelsClient
    {
        public const string Name = "levels";

        private SpeedrunComClient baseClient;

        public LevelsClient(SpeedrunComClient baseClient)
        {
            this.baseClient = baseClient;
        }

        public static Uri GetLevelsUri(string subUri)
        {
            return SpeedrunComClient.GetAPIUri(string.Format("{0}{1}", Name, subUri));
        }

        public Level GetLevelFromSiteUri(string siteUri, LevelEmbeds embeds = default(LevelEmbeds))
        {
            var id = GetLevelIDFromSiteUri(siteUri);

            if (string.IsNullOrEmpty(id))
                return null;

            return GetLevel(id, embeds);
        }

        public string GetLevelIDFromSiteUri(string siteUri)
        {
            var elementDescription = SpeedrunComClient.GetElementDescriptionFromSiteUri(siteUri);

            if (elementDescription == null
                || elementDescription.Type != ElementType.Level)
                return null;

            return elementDescription.ID;
        }

        public Level GetLevel(string levelId, 
            LevelEmbeds embeds = default(LevelEmbeds))
        {
            var parameters = new List<string>() { embeds.ToString() };

            var uri = GetLevelsUri(string.Format("/{0}{1}",
                Uri.EscapeDataString(levelId), 
                parameters.ToParameters()));

            var result = baseClient.DoRequest(uri);

            return Level.Parse(baseClient, result.data);
        }

        public ReadOnlyCollection<Category> GetCategories(
            string levelId, bool miscellaneous = true,
            CategoryEmbeds embeds = default(CategoryEmbeds),
            CategoriesOrdering orderBy = default(CategoriesOrdering))
        {
            var parameters = new List<string>() { embeds.ToString() };

            parameters.AddRange(orderBy.ToParameters());

            if (!miscellaneous)
                parameters.Add("miscellaneous=no");

            var uri = GetLevelsUri(string.Format("/{0}/categories{1}", 
                Uri.EscapeDataString(levelId), 
                parameters.ToParameters()));

            return baseClient.DoDataCollectionRequest<Category>(uri,
                x => Category.Parse(baseClient, x));
        }

        public ReadOnlyCollection<Variable> GetVariables(string levelId,
            VariablesOrdering orderBy = default(VariablesOrdering))
        {
            var parameters = new List<string>(orderBy.ToParameters());

            var uri = GetLevelsUri(string.Format("/{0}/variables{1}", 
                Uri.EscapeDataString(levelId),
                parameters.ToParameters()));

            return baseClient.DoDataCollectionRequest<Variable>(uri,
                x => Variable.Parse(baseClient, x));
        }
    }
}
