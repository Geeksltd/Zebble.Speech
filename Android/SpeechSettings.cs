namespace Zebble.Device
{
    using System.Linq;
    using Java.Util;

    partial class SpeechSettings
    {
        internal Locale GetLocale()
        {
            if (Language == null) return Locale.Default;
            return Locale.GetAvailableLocales().FirstOrDefault(x => x.Language == Language?.Id) ?? Locale.Default;
        }
    }
}