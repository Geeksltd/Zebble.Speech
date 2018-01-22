namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;
    using Windows.Media.SpeechRecognition;

    partial class SpeechRecognizer
    {
        static Windows.Media.SpeechRecognition.SpeechRecognizer Recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

        static async Task DoStart()
        {
            if (Recognizer == null)
            {
                Recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
                var result = await Recognizer.CompileConstraintsAsync().AsTask();
                if (result.Status != SpeechRecognitionResultStatus.Success)
                    throw new Exception("Failed to start speech recognizer: " + result.Status);
            }

            Recognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;

            try
            {
                await Recognizer.ContinuousRecognitionSession.StartAsync();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Recognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                Recognizer = null;
                await Device.OS.OpenSettings("privacy-speechtyping");
                throw new Exception(ex.Message);
            }
        }

        static void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            var confidence = args.Result.Confidence;
            if (confidence != SpeechRecognitionConfidence.Medium && confidence != SpeechRecognitionConfidence.High) return;

            var text = args.Result.Text;
            Thread.Pool.RunAction(() => Listeners?.Invoke(text));
        }

        public static async Task Stop()
        {
            var recognizer = Recognizer;
            if (recognizer == null) return;
            Listeners = null;

            await Thread.UI.Run(async () =>
            {
                try
                {
                    Listeners = null;

                    await recognizer.ContinuousRecognitionSession.StopAsync();
                    recognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
                }
                finally
                {
                    recognizer?.Dispose();
                    Recognizer = null;
                }
            });
        }
    }
}