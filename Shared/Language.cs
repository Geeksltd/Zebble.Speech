﻿namespace Zebble.Device
{
    using System.Collections.Generic;
    using System.Linq;
    public partial class Speech
    {
        public partial class Language
        {
            static List<Language> InstalledLanguages;

            public Language(string languageId = null)
            {
                Id = languageId;
                var parts = Id.Split('.',' ','-','_');

                if (parts.Length>0)
                {
                    LanguageCode = parts.FirstOrDefault().ToLower();
                }
                if (parts.Length==2)
                {
                    CountryCode = parts.LastOrDefault().ToLower();
                }
            }

            public string Id { get; internal set; }
            public string Name { get; internal set; }

            public string LanguageCode { get; internal set; }
            public string CountryCode { get; internal set; }

            public static IEnumerable<Language> GetInstalledLanguages()
            {
                if (InstalledLanguages != null) return InstalledLanguages;

                return InstalledLanguages = FindInstalledLanguages().ToList();
            }
        }
    }
}
