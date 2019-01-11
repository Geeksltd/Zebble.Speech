namespace Zebble.Device
{
    using AVFoundation;
    using Foundation;
    using global::Speech;
    using System;
    using System.Threading.Tasks;

    public partial class Speech
    {
        partial class Recognizer
        {
            static AVAudioEngine AudioEngine;
            static SFSpeechRecognizer SpeechRecognizer;
            static SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest;
            static SFSpeechRecognitionTask RecognitionTask;

            static System.Timers.Timer Timer;

            static Task DoStart()
            {
                if (Device.OS.IsBeforeiOS(10))
                    throw new Exception("This feature is not supported in this device. Please upgrade your iOS.");

                if (SpeechRecognizer == null)
                {
                    SpeechRecognizer = new SFSpeechRecognizer();
                    LiveSpeechRequest = new SFSpeechAudioBufferRecognitionRequest();
                }

                SFSpeechRecognizer.RequestAuthorization(status =>
                {
                    if (status == SFSpeechRecognizerAuthorizationStatus.Authorized) StartRecording();
                    else
                    {
                        Stop();
                        throw new Exception("Speech recognition authorization request was denied.");
                    }
                });

                return Task.CompletedTask;
            }

            static void StartRecording()
            {
                var audioSession = AVAudioSession.SharedInstance();

                audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                audioSession.SetMode(AVAudioSession.ModeMeasurement, out NSError error);
                audioSession.SetActive(true);

                if (error != null)
                {
                    Log.Error(error);
                    return;
                }

                AudioEngine = new AVAudioEngine();
                var node = AudioEngine.InputNode;

                LiveSpeechRequest.ShouldReportPartialResults = true;

                RecognitionTask = SpeechRecognizer.GetRecognitionTask(LiveSpeechRequest, (SFSpeechRecognitionResult result, NSError err) =>
                {
                    if (err != null)
                    {
                        Stop();
                        Log.Error(err);
                        return;
                    }

                    var currentText = result.BestTranscription.FormattedString;

                    if (result.Final)
                    {
                        Listeners?.Invoke(currentText);
                    }

                    Timer = new System.Timers.Timer(20000) { Enabled = true };
                    Timer.Elapsed += (s, ev) =>
                    {

                        AudioEngine?.InputNode?.RemoveTapOnBus(0);
                        AudioEngine?.Stop();
                        AudioEngine?.Dispose();
                        AudioEngine = null;

                        LiveSpeechRequest?.EndAudio();
                        LiveSpeechRequest?.Dispose();
                        LiveSpeechRequest = null;

                        SpeechRecognizer?.Dispose();
                        SpeechRecognizer = null;

                        Timer.Stop();
                        Timer = null;

                        StartRecording();
                    };

                    Timer.Start();
                });

                var recordingFormat = node.GetBusOutputFormat(0);
                node.InstallTapOnBus(0, 1024, recordingFormat, (AVAudioPcmBuffer buffer, AVAudioTime when) =>
                {
                    LiveSpeechRequest.Append(buffer);
                });

                AudioEngine.Prepare();
                AudioEngine.StartAndReturnError(out error);

                if (error != null)
                {
                    Log.Error(error);
                    return;
                }
            }

            public static Task Stop()
            {
                Listeners = null;

                AudioEngine?.InputNode?.RemoveTapOnBus(0);
                AudioEngine?.Stop();
                AudioEngine?.Dispose();
                AudioEngine = null;

                LiveSpeechRequest?.EndAudio();
                LiveSpeechRequest?.Dispose();
                LiveSpeechRequest = null;

                SpeechRecognizer?.Dispose();
                SpeechRecognizer = null;

                Timer?.Dispose();
                Timer = null;

                return Task.CompletedTask;
            }
        }
    }
}