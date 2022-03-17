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
                var selection= langs.FirstOrDefault(x => x.Language.Replace("_","-").StartsWith(Language?.Id,false)) ?? langs.FirstOrDefault(x => x.Language.Replace("_", "-").StartsWith(Language?.Id.Split("-").FirstOrDefault(), false)) ?? Locale.Default;
                return selection;
            }
        }
    }
}