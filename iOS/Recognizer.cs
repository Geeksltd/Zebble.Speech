namespace Zebble.Device
{
    using AVFoundation;
    using Foundation;
    using global::Speech;
    using System;
    using System.Threading.Tasks;
    using Olive;

    public partial class Speech
    {
        partial class Recognizer
        {
            static AVAudioEngine AudioEngine;
            static SFSpeechRecognizer SpeechRecognizer;
            static SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest;
            static SFSpeechRecognitionTask RecognitionTask;

            static System.Timers.Timer Timer;
            static readonly object Lock = new object();

            static Task DoStart()
            {
                if (Device.OS.IsBeforeiOS(10))
                    throw new Exception("This feature is not supported in this device. Please upgrade your iOS.");

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
                lock (Lock)
                {
                    if (SpeechRecognizer == null)
                    {
                        SpeechRecognizer = new SFSpeechRecognizer();
                        LiveSpeechRequest = new SFSpeechAudioBufferRecognitionRequest();
                    }

                    var audioSession = AVAudioSession.SharedInstance();

                    audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                    audioSession.SetMode(AVAudioSession.ModeDefault, out NSError error);
                    audioSession.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out NSError speakerError);
                    audioSession.SetActive(true);

                    if (LogErrorAndStop(error) || LogErrorAndStop(speakerError))
                        return;

                    AudioEngine = new AVAudioEngine();
                    var node = AudioEngine.InputNode;

                    LiveSpeechRequest.ShouldReportPartialResults = true;

                    RecognitionTask = SpeechRecognizer.GetRecognitionTask(LiveSpeechRequest, (SFSpeechRecognitionResult result, NSError err) =>
                    {
                        if (LogErrorAndStop(err))
                            return;

                        var currentText = result.BestTranscription.FormattedString;

                        if (currentText.HasValue())
                        {
                            Listeners?.Invoke(currentText, result.Final);
                        }

                        if (IsContinuous)
                        {
                            Timer = new System.Timers.Timer(20000) { Enabled = true };
                            Timer.Elapsed += (s, ev) =>
                            {
                                StopInstances();
                                StartRecording();
                            };

                            Timer.Start();
                        }
                    });

                    var recordingFormat = node.GetBusOutputFormat(0);
                    node.InstallTapOnBus(0, 1024, recordingFormat, (AVAudioPcmBuffer buffer, AVAudioTime when) =>
                    {
                        LiveSpeechRequest.Append(buffer);
                    });

                    if (AudioEngine == null)
                    {
                        Stop();
                        return;
                    }

                    AudioEngine?.Prepare();
                    AudioEngine?.StartAndReturnError(out error);

                    if (LogErrorAndStop(error))
                        return;
                }
            }

            public static Task Stop()
            {
                Listeners = null;

                StopInstances();

                Stopped?.Invoke();

                return Task.CompletedTask;
            }

            static void StopInstances()
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

                //Timer?.Dispose();
                //Timer = null;
            }

            static bool LogErrorAndStop(NSError error)
            {
                if (error != null)
                {
                    Stop();
                    Log.For(typeof(Recognizer)).Error(null, error.ToString());
                    return true;
                }
                return false;
            }
        }
    }
}