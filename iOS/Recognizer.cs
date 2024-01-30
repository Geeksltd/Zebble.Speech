namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using AVFoundation;
    using Foundation;
    using global::Speech;
    using Olive;

    public partial class Speech
    {
        partial class Recognizer
        {
            static AVAudioSession Session;
            static SFSpeechRecognizer SpeechRecognizer;
            static AVAudioEngine AudioEngine;
            static SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest;
            static SFSpeechRecognitionTask RecognitionTask;

            static Task DoStart()
            {
                if (Device.OS.IsBeforeiOS(10))
                    throw new Exception("This feature is not supported in this device. Please upgrade your iOS.");

                SFSpeechRecognizer.RequestAuthorization(HandleAuthorisationRequest);

                return Task.CompletedTask;
            }

            static void HandleAuthorisationRequest(SFSpeechRecognizerAuthorizationStatus status)
            {
                if (status == SFSpeechRecognizerAuthorizationStatus.Authorized)
                {
                    StopInstances();
                    StartRecording();
                }
                else
                {
                    Stop().GetAwaiter();
                    throw new Exception("Speech recognition authorization request was denied.");
                }
            }

            static void StartRecording()
            {
                if (!ConfigureAudioSession()) return;

                SpeechRecognizer ??= new SFSpeechRecognizer();
                if (!SpeechRecognizer.Available) return;

                AudioEngine = new AVAudioEngine();
                AudioEngine.InputNode?.InstallTapOnBus(0, 1024, AudioEngine.InputNode.GetBusOutputFormat(0), (buffer, when) => LiveSpeechRequest?.Append(buffer));

                LiveSpeechRequest = new SFSpeechAudioBufferRecognitionRequest { ShouldReportPartialResults = true };
                RecognitionTask = SpeechRecognizer.GetRecognitionTask(LiveSpeechRequest, OnRecognitionTaskResult);
                AudioEngine.Prepare();
                AudioEngine.StartAndReturnError(out var error);
                if (error != null) Log.For(typeof(Recognizer)).Error(error.ToString());
            }

            static void OnRecognitionTaskResult(SFSpeechRecognitionResult result, NSError error)
            {
                if (error is null)
                    Detected?.Invoke(result.BestTranscription.FormattedString);
                else
                    Log.For(typeof(Recognizer)).Error(error.ToString());
            }

            static bool ConfigureAudioSession()
            {
                Session = AVAudioSession.SharedInstance();
                if (Session is null) return false;

                Session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                Session.SetMode(AVAudioSession.ModeDefault, out var error);

                if (error != null)
                {
                    Log.For(typeof(Recognizer)).Error(error.ToString());
                    return false;
                }

                Session.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out error);

                if (error != null)
                {
                    Log.For(typeof(Recognizer)).Error(error.ToString());
                    return false;
                }

                Session.SetActive(true);
                return true;
            }

            static Task DoStop()
            {
                Detected = null;
                StopInstances();
                Stopped?.Invoke();

                return Task.CompletedTask;
            }

            static void StopInstances()
            {
                Session?.Dispose();
                Session = null;

                AudioEngine?.InputNode?.RemoveTapOnBus(0);
                AudioEngine?.Stop();
                AudioEngine?.Dispose();
                AudioEngine = null;

                LiveSpeechRequest?.EndAudio();
                LiveSpeechRequest?.Dispose();
                LiveSpeechRequest = null;

                SpeechRecognizer?.Dispose();
                SpeechRecognizer = null;

                RecognitionTask?.Cancel();
                RecognitionTask?.Finish();
                RecognitionTask?.Dispose();
                RecognitionTask = null;
            }
        }
    }
}