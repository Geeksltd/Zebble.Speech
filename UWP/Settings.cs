namespace Zebble.Device
{
    using System.Linq;
    using Olive;
    using Windows.Media.SpeechSynthesis;

    partial class Speech
    {
        partial class Settings
        {
            internal string GetPitchProsody()
            {
                if (Pitch == 1) return "default";

                if (Pitch >= 1.6f) return "x-high";
                else if (Pitch >= 1.1f) return "high";
                else if (Pitch >= .9f) return "medium";
                else if (Pitch >= .4f) return "low";
                else return "x-low";
            }

            internal VoiceInformation SelectVoice()
            {
                return SpeechSynthesizer.AllVoices.OrderByDescending(x => x.Language.StartsWith(Language?.LanguageCode, false))
                    .ThenByDescending(x => x.Language.Contains(Language.CountryCode, false))
                    .ThenByDescending(x => x == SpeechSynthesizer.DefaultVoice)
                    .FirstOrDefault()
                     ?? SpeechSynthesizer.DefaultVoice;
            }
        }
    }
}