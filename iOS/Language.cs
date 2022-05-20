namespace Zebble.Device
{
    using AVFoundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Speech
    {
        partial class Language
        {
            static IEnumerable<Language> FindInstalledLanguages()
            {
                return AVSpeechSynthesisVoice.GetSpeechVoices()
                  .OrderBy(a => a.Language)
                  .Select(a => new Language (a.Language){Name = a.Language });
            }
        }
    }
}