namespace Zebble.Device
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Speech;
    using System;
    using System.Threading.Tasks;

    public partial class Speech
    {
        partial class Recognizer
        {
            static SpeechRecognizer recognizer;
            static RecognitionListener StandardListener;
            static Intent VoiceIntent;
            static bool IsStopped;
            static bool IsEnded;

            static Task DoStart()
            {
                if (recognizer == null)
                {
                    StandardListener = new RecognitionListener();
                    recognizer = SpeechRecognizer.CreateSpeechRecognizer(UIRuntime.NativeRootScreen as AndroidOS.BaseActivity);
                    recognizer.SetRecognitionListener(StandardListener);
                    recognizer.StartListening(CreateIntent());

                    IsStopped = false;
                }

                return Task.CompletedTask;
            }

            static Intent CreateIntent()
            {
                VoiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, Java.Util.Locale.Default);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
                return VoiceIntent;
            }

            public static Task Stop()
            {
                return Thread.UI.Run(async () =>
                {
                    VoiceIntent?.Dispose();
                    VoiceIntent = null;

                    recognizer?.SetRecognitionListener(null);
                    recognizer?.StopListening();

                    try
                    {
                        recognizer?.Destroy();
                        recognizer?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        await Alert.Toast("Speech recognition error: " + ex.Message);
                    }

                    recognizer = null;
                    StandardListener?.Dispose();
                    StandardListener = null;

                    IsStopped = true;
                });
            }

            class RecognitionListener : Java.Lang.Object, IRecognitionListener
            {
                readonly object syncLock = new object();

                public void OnResults(Bundle results)
                {
                    var result = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
                    if (result.Count > 0)
                    {
                        var text = result[0].Trim();
                        Listeners?.Invoke(text);
                    }

                    if (IsEnded && !IsStopped)
                    {
                        RestartRecognizer();
                        IsEnded = false;
                    }
                }

                void RestartRecognizer()
                {
                    lock (syncLock)
                    {
                        recognizer.Destroy();
                        recognizer.Dispose();

                        recognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                        recognizer.SetRecognitionListener(this);
                        recognizer.StartListening(CreateIntent());
                    }
                }

                void IRecognitionListener.OnEndOfSpeech() => IsEnded = true;

                void IRecognitionListener.OnError([GeneratedEnum] SpeechRecognizerError error)
                {
                    if (IsStopped) return;
                    RestartRecognizer();
                }

                void IRecognitionListener.OnEvent(int eventType, Bundle @params) { }

                void IRecognitionListener.OnPartialResults(Bundle partialResults) { }

                void IRecognitionListener.OnReadyForSpeech(Bundle @params) { }

                void IRecognitionListener.OnRmsChanged(float rmsdB) { }

                void IRecognitionListener.OnBeginningOfSpeech() { }

                void IRecognitionListener.OnBufferReceived(byte[] buffer) { }

                protected override void Dispose(bool disposing)
                {
                    Listeners = null;
                    base.Dispose(disposing);
                }
            }
        }
    }
}