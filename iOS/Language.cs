namespace Zebble.Device
{
    using System.Collections.Generic;
    using System.Linq;
    using AVFoundation;

    partial class Speech
    {
        partial class Language
        {
            static IEnumerable<Language> FindInstalledLanguages()
            {
                return AVSpeechSynthesisVoice.GetSpeechVoices()
                  .OrderBy(a => a.Language)
                  .Select(a => new Language(a.Language) { Name = a.Language });
            }
        }
    }
}