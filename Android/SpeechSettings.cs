namespace Zebble.Device
{
    using Java.Util;
    using System.Linq;

    partial class SpeechSettings
    {
        internal Locale GetLocale()
        {
            if (Language == null) return Locale.Default;
            return Locale.GetAvailableLocales().FirstOrDefault(x => x.Language == Language?.Id) ?? Locale.Default;
        }
    }
}