namespace Zebble.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Media.SpeechSynthesis;

    partial class SpeechLanguage
    {
        public VoiceGender Gender { get; set; }

        static IEnumerable<SpeechLanguage> FindInstalledLanguages()
        {
            return SpeechSynthesizer.AllVoices
                            .OrderBy(a => a.Language)
                            .Select(a => new SpeechLanguage
                            {
                                Id = a.Language,
                                Name = a.DisplayName,
                                Gender = a.Gender
                            })
                            .GroupBy(c => c.ToString())
                            .Select(g => g.First());
        }
    }
}
