using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TheThirdAge
{
    /// <summary>
    ///     Filtered backstories file already added.
    ///     string[] filteredWords = {"world", "planet",
    ///     "universe", "research", "space", "galaxy",
    ///     "genetic", "communications", "gun", "ceti", "tech", "machine",
    ///     "addiction"};
    /// </summary>
    public static class BackstoryHandler
    {
        public static void RemoveIncompatibleBackstories(StringBuilder DebugString)
        {
            if (!ModStuff.Settings.LimitTechnology)
            {
                return;
            }

            DebugString.AppendLine("BackstoryDef Removal List");
            //StringBuilder listOfBackstoriesToRemove = new StringBuilder();
            var tempBackstoryKeys = BackstoryDatabase.allBackstories.Keys;
            foreach (var badId in GetIncompatibleBackstories())
            {
                //listOfBackstoriesToRemove.AppendLine(badId);
                foreach (var existingId in tempBackstoryKeys)
                {
                    var properId = RemoveNumbers(existingId);
                    //listOfBackstoriesToRemove.AppendLine(":: " + properId);
                    if (properId != RemoveNumbers(badId))
                    {
                        continue;
                    }

                    BackstoryDatabase.allBackstories.Remove(existingId);
                    //listOfBackstoriesToRemove.AppendLine("::::::::::::: ");
                    //listOfBackstoriesToRemove.AppendLine(":: REMOVED :: ");
                    //listOfBackstoriesToRemove.AppendLine(existingId);
                    //listOfBackstoriesToRemove.AppendLine("::::::::::::: ");
                    break;
                }
            }

            //Log.Message(listOfBackstoriesToRemove.ToString());
            //var cacheKeyType = AccessTools.TypeByName("CacheKey");
            //Log.Message(cacheKeyType.ToString());
            //dynamic shuffle = AccessTools.Field(typeof(BackstoryDatabase), "shuffleableBackstoryList").GetValue(null);
            //shuffle.Clear();
        }

        public static void ListIncompatibleBackstories()
        {
            var listOfBackstoriesToRemove = new StringBuilder();
            foreach (var bsy in BackstoryDatabase.allBackstories)
            {
                var bs = bsy.Value;
                var bsTitle = bs.title.ToLowerInvariant();
                var bsDesc = bs.baseDesc.ToLowerInvariant();
                string[] filteredWords =
                {
                    "world", "planet", "vat", "robot", "organ",
                    "universe", "research", "midworld", "space", "galaxy", "star system",
                    "genetic", "communications", "gun", "ceti", "tech", "machine",
                    "addiction", "starship", "pilot", "coma", "napalm", "imperial"
                };
                foreach (var subString in filteredWords)
                {
                    if (!(bsTitle + " " + bsDesc).Contains(subString))
                    {
                        continue;
                    }

                    listOfBackstoriesToRemove.AppendLine(bsy.Key);
                    break;
                }
            }

            Log.Message(listOfBackstoriesToRemove.ToString());
        }


        private static IEnumerable<string> GetIncompatibleBackstories()
        {
            if (!Translator.TryGetTranslatedStringsForFile("Static/IncompatibleBackstories", out var list))
            {
                yield break;
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }

        public static string GetIdentifier(string s)
        {
            return RemoveNumbers(s);
        }

        private static string RemoveNumbers(string s)
        {
            var result = new StringBuilder();
            foreach (var value in s)
            {
                if (char.IsLetter(value))
                {
                    result.Append(value);
                }
            }

            return result.ToString();
        }
    }
}