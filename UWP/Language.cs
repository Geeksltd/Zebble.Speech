namespace Zebble.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Media.SpeechSynthesis;

    partial class Speech
    {
        partial class Language
        {
            public VoiceGender Gender { get; set; }

            static IEnumerable<Language> FindInstalledLanguages()
            {
                return SpeechSynthesizer.AllVoices
                                .OrderBy(a => a.Language)
                                .Select(a => new Language(a.Language)
                                {
                                    Name = a.DisplayName,
                                    Gender = a.Gender
                                })
                                .GroupBy(c => c.ToString())
                                .Select(g => g.First());
            }
        }
    }
}
