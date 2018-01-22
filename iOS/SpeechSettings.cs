namespace Zebble.Device
{
    using AVFoundation;
    using System.Linq;
    using Zebble.Device;

    partial class SpeechSettings
    {
        internal AVSpeechSynthesisVoice GetVoiceForLocaleLanguage()
        {
            var language = SpeechLanguage.GetInstalledLanguages().FirstOrDefault(x => x.Id == Language?.Id)?.Id ?? AVSpeechSynthesisVoice.CurrentLanguageCode;

            var voice = AVSpeechSynthesisVoice.FromLanguage(language);
            if (voice != null) return voice;

            Log.Error("Voice not found for language: " + language + ". Using default instead.");
            return AVSpeechSynthesisVoice.FromLanguage(AVSpeechSynthesisVoice.CurrentLanguageCode);
        }
    }
}