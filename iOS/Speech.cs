namespace Zebble.Device
{
    using AVFoundation;
    using System.Threading.Tasks;
    using Olive;

    public partial class Speech
    {
        static readonly AVSpeechSynthesizer SpeechSynthesizer = new AVSpeechSynthesizer();

        static async Task DoSpeak(string text, Settings settings)
        {
            var utterance = new AVSpeechUtterance(text)
            {
                Rate = GetNormalizedSpeed(settings.Speed).LimitWithin(AVSpeechUtterance.MinimumSpeechRate, AVSpeechUtterance.MaximumSpeechRate),
                Voice = settings.GetVoiceForLocaleLanguage(),
                Volume = settings.Volume,
                PitchMultiplier = settings.Pitch
            };

            SpeechSynthesizer.DidFinishSpeechUtterance += OnFinishedSpeechUtterance;

            try
            {
                SpeechSynthesizer.SpeakUtterance(utterance);
                await SpeechInProgress.Task;
            }
            finally
            {
                SpeechSynthesizer.DidFinishSpeechUtterance -= OnFinishedSpeechUtterance;
            }
        }

        /// <summary>
        /// It should be between 0 and 1 and it`s defalt is 0.5;
        /// </summary>
        static float GetNormalizedSpeed(float speed)
        {
            if (speed == 1)
                return 0.5F;

            else if (speed < 1)
                return speed / 2;

            else
                return 0.5F + speed / 20;
        }

        static void OnFinishedSpeechUtterance(object sender, AVSpeechSynthesizerUteranceEventArgs args)
        {
            SpeechInProgress?.TrySetResult(false);
        }

        static void DoStop()
        {
            SpeechSynthesizer.StopSpeaking(AVSpeechBoundary.Word);
        }
    }
}