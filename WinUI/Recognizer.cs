namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Windows.Media.SpeechRecognition;

    public partial class Speech
    {
        partial class Recognizer
        {
            static Windows.Media.SpeechRecognition.SpeechRecognizer recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            static async Task DoStart()
            {
                if (recognizer == null)
                {
                    recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer(new Windows.Globalization.Language(Accent));
                    var result = await recognizer.CompileConstraintsAsync().AsTask();

                    if (result.Status != SpeechRecognitionResultStatus.Success)
                        throw new Exception("Failed to start speech recognizer: " + result.Status);
                }

                recognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

                try
                {
                    await recognizer.ContinuousRecognitionSession.StartAsync();
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    recognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                    recognizer = null;
                    await Device.OS.OpenSettings("privacy-speechtyping");
                    throw new Exception(ex.Message);
                }
            }

            static void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
            {
                if (args.Result.Confidence == SpeechRecognitionConfidence.Rejected) return;
                Thread.Pool.RunAction(() => Detected?.Invoke(args.Result.Text));
            }

            static async Task DoStop()
            {
                var recognizer = Recognizer.recognizer;
                if (recognizer == null) return;
                Detected = null;

                try
                {
                    await recognizer.ContinuousRecognitionSession.StopAsync();
                    recognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                }
                finally
                {
                    recognizer?.Dispose();
                    Recognizer.recognizer = null;
                    Stopped?.Invoke();
                }
            }
        }
    }
}