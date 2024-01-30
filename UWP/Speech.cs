namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Media.Playback;
    using Windows.Media.SpeechSynthesis;

    public partial class Speech
    {
        static SpeechSynthesizer Synthesizer = new SpeechSynthesizer();
        static MediaPlayer Player = new MediaPlayer();

        static async Task DoSpeak(string text, Settings settings)
        {
            Synthesizer.Voice = settings.SelectVoice();

            Synthesizer.Options.SpeakingRate = GetNormalizedSpeed(settings.Speed);

            var handler = new TypedEventHandler<MediaPlayer, object>((sender, args) => SpeechInProgress?.TrySetResult(true));
            Player.MediaEnded += handler;

            var stream = (await Synthesizer.SynthesizeTextToStreamAsync(text));
            Player.SetStreamSource(stream);

            Player.Play();

            await SpeechInProgress.Task;
            Player.MediaEnded -= handler;
        }

        /// <summary>
        /// It should be between 0.5 and 6 and it`s defalt is 1;
        /// </summary>
        static float GetNormalizedSpeed(float speed)
        {
            if (speed == 1) return 1;
            else if (speed < 1) return 0.5F + speed / 2;
            else return speed * 0.6F;
        }

        static void DoStop()
        {
            if (Player.PlaybackSession?.CanPause == true)
                Player.Pause();
        }
    }
}