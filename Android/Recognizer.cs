﻿namespace Zebble.Device
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
            static SpeechRecognizer AndroidRecognizer;
            static RecognitionListener StandardListener;
            static Intent VoiceIntent;
            static bool IsStopped;

            static Task DoStart()
            {
                if (AndroidRecognizer == null)
                {
                    StandardListener = new RecognitionListener();
                    AndroidRecognizer = SpeechRecognizer.CreateSpeechRecognizer(UIRuntime.CurrentActivity);
                    AndroidRecognizer.SetRecognitionListener(StandardListener);
                    AndroidRecognizer.StartListening(CreateIntent());

                    IsStopped = false;
                }

                return Task.CompletedTask;
            }

            static Intent CreateIntent()
            {
                VoiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference, Java.Util.Locale.English);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Accent);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 15000);
                return VoiceIntent;
            }

            static async Task DoStop()
            {
                VoiceIntent?.Dispose();
                VoiceIntent = null;

                try
                {
                    AndroidRecognizer?.SetRecognitionListener(null);
                    AndroidRecognizer?.Destroy();
                    AndroidRecognizer?.Dispose();
                }
                catch (Exception ex)
                {
                    await Dialogs.Current.Toast("Speech recognition error: " + ex.Message);
                }

                AndroidRecognizer = null;
                StandardListener?.Dispose();
                StandardListener = null;

                IsStopped = true;

                Stopped?.Invoke();
            }

            class RecognitionListener : Java.Lang.Object, IRecognitionListener
            {
                readonly object SyncLock = new object();

                public RecognitionListener() { }

                [Preserve]
                public RecognitionListener(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle) { }

                public void OnResults(Bundle results)
                {
                    var text = GetPlainText(results);
                    if (!string.IsNullOrEmpty(text)) Detected?.Invoke(text);
                    Recognizer.DoStop().RunInParallel();
                }

                void RestartRecognizer()
                {
                    lock (SyncLock)
                    {
                        try
                        {
                            AndroidRecognizer?.SetRecognitionListener(null);
                            AndroidRecognizer?.Destroy();
                            AndroidRecognizer?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Dialogs.Current.Toast("Speech recognition error: " + ex.Message).RunInParallel();
                        }

                        AndroidRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                        AndroidRecognizer.SetRecognitionListener(this);
                        AndroidRecognizer.StartListening(CreateIntent());
                    }
                }

                void IRecognitionListener.OnEndOfSpeech() { }

                void IRecognitionListener.OnError([GeneratedEnum] SpeechRecognizerError error)
                {
                    if (IsStopped) return;
                    RestartRecognizer();
                }

                void IRecognitionListener.OnEvent(int eventType, Bundle @params) { }

                void IRecognitionListener.OnPartialResults(Bundle partialResults)
                {
                    var text = GetPlainText(partialResults);
                    if (!string.IsNullOrEmpty(text)) Detected?.Invoke(text);
                }

                string GetPlainText(Bundle results)
                {
                    var result = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
                    if (result?.Count > 0) return result[0].Trim();
                    return string.Empty;
                }

                void IRecognitionListener.OnReadyForSpeech(Bundle @params) { }

                void IRecognitionListener.OnRmsChanged(float rmsdB) { }

                void IRecognitionListener.OnBeginningOfSpeech() { }

                void IRecognitionListener.OnBufferReceived(byte[] buffer) { }

                protected override void Dispose(bool disposing)
                {
                    Detected = null;
                    IsStopped = true;

                    base.Dispose(disposing);
                }
            }
        }
    }
}