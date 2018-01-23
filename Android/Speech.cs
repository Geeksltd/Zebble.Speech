namespace Zebble.Device
{
    using Android.App;
    using Android.Runtime;
    using Android.Speech.Tts;
    using System;
    using System.Threading.Tasks;

    public partial class Speech
    {
        static SpeechListener Listener = new SpeechListener();
        static TextToSpeech TextToSpeech;
        static TaskCompletionSource<bool> InitializationAwaiter = new TaskCompletionSource<bool>();

        static Speech()
        {
            TextToSpeech = new TextToSpeech(Application.Context, Listener);
        }

        static async Task DoSpeak(string text, Settings settings)
        {
            if (text.Length > TextToSpeech.MaxSpeechInputLength)
            {
                Device.Log.Error("Text-to-Speech text length exceeds the maximum supported by this device.");
                text = text.Summarize(TextToSpeech.MaxSpeechInputLength);
            }

            if (!Listener.IsInitialized) await InitializationAwaiter.Task;

            TextToSpeech.SetLanguage(settings.GetLocale());

            TextToSpeech.SetPitch(settings.Pitch);
            TextToSpeech.SetSpeechRate(settings.Speed);
            var result = TextToSpeech.Speak(text, QueueMode.Flush, null);

            if (result == OperationResult.Error)
                Device.Log.Error(new ArgumentException("Error in text-to-speech engine when listening to progress."));
        }

        public static void Stop() => TextToSpeech.Stop();

        class SpeechListener : Java.Lang.Object, TextToSpeech.IOnInitListener
        {
            public bool IsInitialized;

            public void OnInit([GeneratedEnum] OperationResult status)
            {
                if (status.Equals(OperationResult.Success))
                {
                    IsInitialized = true;
                    InitializationAwaiter.TrySetResult(result: true);
                }
                else
                {
                    InitializationAwaiter.TrySetException(new ArgumentException("Failed to initialize the text to speech engine."));
                }
            }
        }
    }
}