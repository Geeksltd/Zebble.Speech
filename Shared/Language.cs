namespace Zebble.Device
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
            }

            public string Id { get; internal set; }
            public string Name { get; internal set; }

            public static IEnumerable<Language> GetInstalledLanguages()
            {
                if (InstalledLanguages != null) return InstalledLanguages;

                return InstalledLanguages = FindInstalledLanguages().ToList();
            }
        }
    }
}
