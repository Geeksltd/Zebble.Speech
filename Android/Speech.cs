namespace Zebble.Device
{
    using Android.App;
    using Android.Runtime;
    using Android.Speech.Tts;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Olive;

    public partial class Speech
    {
        static SpeechListener Listener = new SpeechListener();
        static SpeechProgressListener ProgressListener = new SpeechProgressListener();
        static TextToSpeech TextToSpeech;
        static TaskCompletionSource<bool> InitializationAwaiter = new TaskCompletionSource<bool>();

        const int MAX_INPUT_LEN = 4000;

        static Speech()
        {
            TextToSpeech = new TextToSpeech(Application.Context, Listener);
            TextToSpeech.SetOnUtteranceProgressListener(ProgressListener);
        }

        static async Task DoSpeak(string text, Settings settings)
        {
            if (text.Length > MAX_INPUT_LEN)
            {
                Log.Error("Text-to-Speech text length exceeds the maximum supported by this device.");
                text = text.Summarize(TextToSpeech.MaxSpeechInputLength);
            }

            if (!Listener.IsInitialized) await InitializationAwaiter.Task;

            TextToSpeech.SetLanguage(settings.GetLocale());
            TextToSpeech.SetPitch(settings.Pitch);
            TextToSpeech.SetSpeechRate(settings.Speed);

            OperationResult result;
            var map = new Dictionary<string, string>();
            map.Add(TextToSpeech.Engine.KeyParamUtteranceId, Guid.NewGuid().ToString());

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                result = TextToSpeech.Speak(text, QueueMode.Flush, null, map[TextToSpeech.Engine.KeyParamUtteranceId]);
            else
                result = TextToSpeech.Speak(text, QueueMode.Flush, map);

            if (result == OperationResult.Error)
                Log.Error(new ArgumentException("Error in text-to-speech engine when listening to progress."));
        }

        static void DoStop() => TextToSpeech.Stop();

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

        class SpeechProgressListener : UtteranceProgressListener
        {
            public override void OnStart(string utteranceId)
            {
            }

            public override void OnDone(string utteranceId) => SpeechInProgress.TrySetResult(true);

            public override void OnError(string utteranceId)
            {
                SpeechInProgress.TrySetResult(false);
                Log.Error($"Error in text-to-speech engine when listening to progress. [{utteranceId}]");
            }
        }
    }
}