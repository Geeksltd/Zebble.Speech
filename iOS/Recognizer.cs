﻿namespace Zebble.Device
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
            static SFSpeechRecognizer SpeechRecognizer;
            static AVAudioEngine AudioEngine;
            static SFSpeechAudioBufferRecognitionRequest LiveSpeechRequest;
            static SFSpeechRecognitionTask RecognitionTask;

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
                StopInstances();

                SpeechRecognizer = new SFSpeechRecognizer();

                if (!SpeechRecognizer.Available)
                    return;

                if (SFSpeechRecognizer.AuthorizationStatus != SFSpeechRecognizerAuthorizationStatus.Authorized)
                    return;

                var audioSession = AVAudioSession.SharedInstance();

                audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                audioSession.SetMode(AVAudioSession.ModeDefault, out NSError error);
                audioSession.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out NSError speakerError);
                audioSession.SetActive(true);

                if (LogErrorAndStop(error) || LogErrorAndStop(speakerError))
                    return;

                AudioEngine = new AVAudioEngine();
                LiveSpeechRequest = new SFSpeechAudioBufferRecognitionRequest();

                var node = AudioEngine.InputNode;
                var recordingFormat = node.GetBusOutputFormat(0);

                node.InstallTapOnBus(0, 1024, recordingFormat, (buffer, when) =>
                {
                    LiveSpeechRequest.Append(buffer);
                });

                LiveSpeechRequest.ShouldReportPartialResults = true;

                var currentIndex = 0;
                RecognitionTask = SpeechRecognizer.GetRecognitionTask(LiveSpeechRequest, (result, err) =>
                {
                    if (LogErrorAndStop(err))
                        return;

                    var currentText = result.BestTranscription.FormattedString;

                    for (var i = currentIndex; i < result.BestTranscription.Segments.Length; i++)
                    {
                        var s = result.BestTranscription.Segments[i].Substring;
                        currentIndex++;
                        Listeners?.Invoke(s, result.Final);
                    }
                });

                AudioEngine.Prepare();
                AudioEngine.StartAndReturnError(out error);

                if (LogErrorAndStop(error))
                    return;
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

                RecognitionTask?.Cancel();
                RecognitionTask?.Finish();
                RecognitionTask?.Dispose();
                RecognitionTask = null;
            }

            static bool LogErrorAndStop(NSError error)
            {
                if (error != null)
                {
                    Stop();
                    Log.For(typeof(Recognizer)).Error(error.ToString());
                    return true;
                }
                return false;
            }
        }
    }
}