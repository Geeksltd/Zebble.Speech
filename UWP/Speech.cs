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

        static async Task DoSpeak(string text, Settings settings)
        {
            Synthesizer.Voice = settings.SelectVoice();

            Synthesizer.Options.SpeakingRate = GetNormalizedSpeed(settings.Speed);

            var tcs = new TaskCompletionSource<object>();
            var handler = new TypedEventHandler<MediaPlayer, object>((sender, args) => tcs.TrySetResult(null));

            var player = BackgroundMediaPlayer.Current;
            player.MediaEnded += handler;

            var stream = await Synthesizer.SynthesizeTextToStreamAsync(text);
            player.SetStreamSource(stream);

            player.Play();

            await tcs.Task;
            player.MediaEnded -= handler;
        }

        /// <summary>
        /// It should be between 0.5 and 6 and it`s defalt is 1;
        /// </summary>
        static float GetNormalizedSpeed(float speed)
        {
            if (speed == 1)
                return 1;

            else if (speed < 1)
                return 0.5F + speed / 2;

            else
                return speed * 0.6F;
        }

        public static void Stop() => BackgroundMediaPlayer.Current.Pause();
    }
}