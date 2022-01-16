namespace Zebble.Device
{
    using Java.Util;
    using System.Linq;

    partial class Speech
    {
        partial class Settings
        {
            internal Locale GetLocale()
            {
                if (Language == null) return Locale.Default;
                var langs = Locale.GetAvailableLocales().ToList();
                return langs.FirstOrDefault(x => x.Language.Replace("_","-").StartsWith(Language?.Id.ToLower())) ?? Locale.Default;
            }
        }
    }
}