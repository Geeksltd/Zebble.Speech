namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;

    public static partial class SpeechRecognizer
    {
        public static Action<string> Listeners;

        public static async Task<bool> Start(Action<string> listener, OnError errorAction = OnError.Alert)
        {
            try { await Stop(); } catch { /* No logging is needed. */ }

            try
            {
                if (!await Device.Permission.Speech.IsRequestGranted())
                    throw new Exception("Request was denied to access Speech Recognition.");

                await Thread.UI.Run(DoStart);
                Listeners += listener;
                return true;
            }
            catch (Exception ex)
            {
                await errorAction.Apply(ex);
                return false;
            }
        }
    }
}
