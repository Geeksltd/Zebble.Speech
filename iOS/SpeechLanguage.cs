namespace Zebble.Device
{
    using AVFoundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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