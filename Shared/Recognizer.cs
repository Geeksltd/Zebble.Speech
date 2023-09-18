﻿namespace Zebble.Device
{
    using System;
    using System.Threading.Tasks;

    public partial class Speech
    {
        public static partial class Recognizer
        {
            public static Action<string> Detected;
            public static Action Stopped;

            public static async Task<bool> Start(Action<string> listener, OnError errorAction = OnError.Alert)
            {
                try { await Stop(); } catch { /* No logging is needed. */ }

                try
                {
                    if (!await Permission.Speech.IsRequestGranted())
                        throw new Exception("Request was denied to access Speech Recognition.");

                    await Thread.UI.Run(DoStart);
                    Detected += listener;
                    return true;
                }
                catch (Exception ex)
                {
                    await errorAction.Apply(ex);
                    return false;
                }
            }

            public static async Task<bool> Stop(OnError errorAction = OnError.Alert)
            {
                try
                {
                    await Thread.UI.Run(DoStop);
                    return true;
                }
                catch (InvalidOperationException) { return false; }
                catch (Exception ex)
                {
                    await errorAction.Apply(ex);
                    return false;
                }
            }
        }
    }
}
