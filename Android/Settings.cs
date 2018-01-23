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
                return Locale.GetAvailableLocales().FirstOrDefault(x => x.Language == Language?.Id) ?? Locale.Default;
            }
        }
    }
}