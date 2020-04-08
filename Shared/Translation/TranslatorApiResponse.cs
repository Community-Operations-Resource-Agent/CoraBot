using System.Collections.Generic;

namespace Shared.Translation
{
    public class TranslatorApiResponse
    {
        public TranslatorDetectedLanguage DetectedLanguage { get; set; }
        public List<TranslatorTranslation> Translations { get; set; }
    }

    public class TranslatorDetectedLanguage
    {
        public string Language { get; set; }
    }

    public class TranslatorTranslation
    {
        public string Text { get; set; }

        public string To { get; set; }
    }
}