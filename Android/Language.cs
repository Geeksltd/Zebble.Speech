namespace Zebble.Device
{
    using Java.Util;
    using System.Collections.Generic;
    using System.Linq;
    using  Olive;
    
    partial class Speech
    {
        partial class Language
        {
            static IEnumerable<Language> FindInstalledLanguages()
            {
                return Locale.GetAvailableLocales()
                  .Where(a => a.Language.HasValue() && a.Country.HasValue() && a.DisplayName.HasValue())
                  .Select(a => new Language 
                  { 
                      Id = a.ToString(), 
                      LanguageCode= a.Language,
                      CountryCode=a.Country,
                      Name = a.DisplayName 
                  }
                  )
                  .GroupBy(c => c.Id)
                  .Select(g => g.First());
            }
        }
    }
}