using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public interface IMultiLingual
    {
        Translation AddTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string createdBy);
        void DeletePhrase(int phraseId);
        void DeletePhraseReference(int referenceId);
        List<Phrase> FindPhrases(string languageCode, string searchText, string source, int status, int pageNo, int pageSize, ref int? totalPhraseCount);
        List<Language> GetAllLanguages();
        List<Region> GetAllRegions();
        List<Translation> GetAllTranslationsByLanguage(string languageCode);
        Language GetLanguageByCulture(string cultureCode);
        string GetLanguageCodeByCulture(string cultureCode);
        string GetLanguageCodeByName(string name);
        Language GetLanguageByName(string name);
        Language GetLanguage(string code);
        Language GetDefaultLanguage();
        string GetDefaultLanguageCode();
        Phrase GetPhrase(int phraseId);
        Phrase GetPhraseByReferenceId(int referenceId);
        string GetPhraseTextByReferenceId(int referenceId, string languageCode);
        Phrase GetPhraseByText(string text);
        PhraseReference GetPhraseReference(int phraseReferenceId);
        Region GetRegion(string regionCode);
        Translation GetTranslation(int translationId);
        Translation GetTranslation(int phraseId, string languageCode);
        bool IsCultureSupported(string cultureCode);
        string LocaliseText(string source, string language);
        void LocaliseText(StringBuilder source, string language);
        string RegisterAndWrapPhraseText(string text, string referenceName, string createdBy, MLangReferenceFormat format);
        int RegisterPhraseText(string text, string referenceName, string createdBy);
        void RegisterTranslation(int phraseId, string translationText, string languageCode, string updatedBy);
        bool UpdateDefaultTranslation(int referenceId, string text, string updatedBy);
        Phrase UpdatePhraseText(int phraseId, string phraseText, string updatedBy);
        Translation UpdateTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string updatedBy);
        string WrapPhrase(int referenceId, MLangReferenceFormat format);
        string WrapPhrase(string mlid, string text, MLangReferenceFormat format);
    }
}
