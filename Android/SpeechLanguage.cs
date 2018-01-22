﻿namespace Zebble.Device
{
    using Java.Util;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SpeechLanguage
    {
        static IEnumerable<SpeechLanguage> FindInstalledLanguages()
        {
            return Locale.GetAvailableLocales()
              .Where(a => a.Language.HasValue() && a.DisplayName.HasValue())
              .Select(a => new SpeechLanguage { Id = a.Language, Name = a.DisplayName })
              .GroupBy(c => c.Id)
              .Select(g => g.First());
        }
    }
}