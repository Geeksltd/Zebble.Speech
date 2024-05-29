namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using AVFoundation;

    public partial class Speech
    {
        static readonly AVSpeechSynthesizer SpeechSynthesizer = new();

        static async Task DoSpeak(string text, Settings settings)
        {
            var utterance = new AVSpeechUtterance(text)
            {
                Rate = GetNormalizedSpeed(settings.Speed),
                Voice = settings.GetVoiceForLocaleLanguage(),
                Volume = settings.Volume,
                PitchMultiplier = settings.Pitch
            };

            EventHandler<AVSpeechSynthesizerUteranceEventArgs> handler = null;
            handler = (sender, args) =>
            {
                SpeechSynthesizer.DidFinishSpeechUtterance -= handler;
                SpeechInProgress?.TrySetResult(false);
            };

            SpeechSynthesizer.DidFinishSpeechUtterance += handler;

            SpeechSynthesizer.SpeakUtterance(utterance);
            await SpeechInProgress.Task;
        }

        static float GetNormalizedSpeed(float speed) => speed switch
        {
            1 => 0.5F,
            < 1 => speed / 2,
            _ => 0.5F + speed / 20
        };

        static void DoStop() => SpeechSynthesizer.StopSpeaking(AVSpeechBoundary.Word);
    }
}