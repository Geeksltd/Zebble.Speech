namespace Zebble.Device
{
    using Android.Speech.Tts;
    using System;
    using System.Threading.Tasks;

    internal class SpeakAwaiter : UtteranceProgressListener
    {
        TaskCompletionSource<bool> Source = new TaskCompletionSource<bool>();
        public Task Task => Source.Task;
        public override void OnStart(string _) { }

        public override void OnDone(string _) => Source.TrySetResult(result: true);

        public override void OnError(string _)
        {
            Source.TrySetException(new ArgumentException("Error in text-to-speech engine when listening to progress."));
        }
    }
}