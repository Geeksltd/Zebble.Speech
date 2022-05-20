namespace Zebble.Device
{
    using Java.Util;
    using System.Linq;
    using Olive;
    partial class Speech
    {
        partial class Settings
        {
            internal Locale GetLocale()
            {
                if (Language == null) return Locale.Default;
                var langs = Locale.GetAvailableLocales().ToList();
                var all = langs.OrderByDescending(x=>x.Language.Equals(Language.LanguageCode,false))
                    .ThenByDescending(x=>x.Country.Equals(Language.CountryCode,false))
                    .ThenByDescending(x=>x==Locale.Default)
                    .ToList();


                var selection = all.FirstOrDefault();
                return selection;
            }

            
        }
    }
}