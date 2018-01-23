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
            const int SPEECH_TIMEOUT = 1500;

            static Android.Speech.SpeechRecognizer recognizer;
            static RecognitionListener StandardListener;
            static Intent VoiceIntent;

            static Task DoStart()
            {
                if (recognizer == null)
                {
                    StandardListener = new RecognitionListener();
                    recognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(UIRuntime.NativeRootScreen as AndroidOS.BaseActivity);
                    recognizer.SetRecognitionListener(StandardListener);
                    recognizer.StartListening(CreateIntent());
                }

                return Task.CompletedTask;
            }

            static Intent CreateIntent()
            {
                VoiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak Now :)");
                VoiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, SPEECH_TIMEOUT);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, SPEECH_TIMEOUT);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, SPEECH_TIMEOUT * 3);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 10);
                VoiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
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
                });
            }

            class RecognitionListener : Activity, IRecognitionListener
            {
                public RecognitionListener() { }

                public RecognitionListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

                public void OnResults(Bundle results)
                {
                    var result = results.GetStringArrayList(Android.Speech.SpeechRecognizer.ResultsRecognition);
                    if (result.Count > 0)
                    {
                        var text = result[0].Trim();
                        Listeners?.Invoke(text);
                    }
                }

                public new IntPtr Handle { get; }

                void IRecognitionListener.OnRmsChanged(float rmsdB) { }

                void IRecognitionListener.OnBeginningOfSpeech() { }

                void IRecognitionListener.OnBufferReceived(byte[] buffer) { }

                void IRecognitionListener.OnEndOfSpeech() { }

                void IRecognitionListener.OnError([GeneratedEnum] SpeechRecognizerError error) { }

                void IRecognitionListener.OnEvent(int eventType, Bundle @params) { }

                void IRecognitionListener.OnPartialResults(Bundle partialResults) { }

                void IRecognitionListener.OnReadyForSpeech(Bundle @params) { }

                protected override void Dispose(bool disposing)
                {
                    Listeners = null;
                    base.Dispose(disposing);
                }
            }
        }
    }
}