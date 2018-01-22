namespace Zebble.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AVFoundation;

    partial class SpeechLanguage
    {
        static IEnumerable<SpeechLanguage> FindInstalledLanguages()
        {
            return AVSpeechSynthesisVoice.GetSpeechVoices()
              .OrderBy(a => a.Language)
              .Select(a => new SpeechLanguage { Id = a.Language, Name = a.Language });
        }
    }
}