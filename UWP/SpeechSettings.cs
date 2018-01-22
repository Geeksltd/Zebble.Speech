namespace Zebble.Device
{
    using System.Linq;
    using Windows.Media.SpeechSynthesis;

    partial class SpeechSettings
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
            return SpeechSynthesizer.AllVoices.FirstOrDefault(x => x.Language == Language?.Id)
                 ?? SpeechSynthesizer.DefaultVoice;
        }
    }
}
