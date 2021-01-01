namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Olive;

    public static partial class Speech
    {
        static TaskCompletionSource<bool> SpeechInProgress;

        /// <summary>
        /// Speaks a specified text using the device's operating system.
        /// </summary>
        public static async Task Speak(string text, Settings settings = null, OnError errorAction = OnError.Toast)
        {
            if (text.IsEmpty()) return;
            settings ??= new Settings();
            settings.Volume = settings.Volume.LimitWithin(0, 1);

            SpeechInProgress?.TrySetResult(false);

            SpeechInProgress = new TaskCompletionSource<bool>();

            try
            {
                await DoSpeak(text, settings);
                await SpeechInProgress.Task;
            }
            catch (Exception ex)
            {
                await errorAction.Apply(ex, "Failed to run Text to Speech.");
            }
        }

        public static void Stop()
        {
            DoStop();

            SpeechInProgress?.TrySetResult(false);
        }
    }
}
