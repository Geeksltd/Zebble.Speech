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
            static SFSpeechRecognizer recognizer;
            static AVAudioEngine AudioEngine;
            static SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest;

            static Task DoStart()
            {
                if (Device.OS.IsBeforeiOS(10))
                    throw new Exception("This feature is not supported in this device. Please upgrade your iOS.");

                if (recognizer == null)
                {
                    recognizer = new SFSpeechRecognizer();
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
                AudioEngine = new AVAudioEngine();
                var node = AudioEngine.InputNode;
                var recordingFormat = node.GetBusOutputFormat(0);
                var tapBlock = new AVAudioNodeTapBlock((buffer, when) => { LiveSpeechRequest.Append(buffer); });
                node.InstallTapOnBus(0, 1024, recordingFormat, tapBlock);
                AudioEngine.Prepare();
                AudioEngine.StartAndReturnError(out var error);

                if (error == null)
                    recognizer.GetRecognitionTask(LiveSpeechRequest,
                        (SFSpeechRecognitionResult result, NSError err) =>
                        {
                            if (err == null)
                                Listeners?.Invoke(result.BestTranscription.FormattedString);
                        });
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

                recognizer?.Dispose();
                recognizer = null;

                return Task.CompletedTask;
            }
        }
    }
}