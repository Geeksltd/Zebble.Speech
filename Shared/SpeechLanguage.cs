namespace Zebble.Device
{
    using System.Collections.Generic;
    using System.Linq;

    public partial class SpeechLanguage
    {
        static List<SpeechLanguage> InstalledLanguages;

        public SpeechLanguage(string languageId = null)
        {
            Id = languageId;
        }

        public string Id { get; internal set; }
        public string Name { get; internal set; }

        public static IEnumerable<SpeechLanguage> GetInstalledLanguages()
        {
            if (InstalledLanguages != null) return InstalledLanguages;

            return InstalledLanguages = FindInstalledLanguages().ToList();
        }
    }
}
