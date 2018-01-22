namespace Zebble.Device
{
    public partial class SpeechSettings
    {
        /// <summary>If not specified, the device's default language will be used.</summary>
        public SpeechLanguage Language { get; set; }

        /// <summary>Normal pitch (default) is 1.</summary>
        public float Pitch { get; set; } = 1;

        /// <summary>Normal speed (default) is 1 and It could be a value between 0 to 10.</summary>
        public float Speed { get; set; } = 1;

        /// <summary>Anything from 0 to 1.</summary>
        public float Volume { get; set; } = 1;
    }
}
