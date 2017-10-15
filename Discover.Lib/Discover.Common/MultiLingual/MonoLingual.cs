using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Discover.MultiLingual
{
    public class MonoLingual : IMultiLingual
    {
        const string RegExFindLangReference = "(<mlang[ ]*mlid=\\\"(?<mlid>[\\d@!]*)\\\"[ ]*((>(?<text>[^<]*)</mlang>)|(/>)))|(<mlang[ ]*mlid=(?<mlid>[\\d@!]*)[ ]*((>(?<text>[^<]*)</mlang>)|(/>)))|(@mlang\\[(?<mlid>[\\d@!]+)\\]\\[(?<text>[^]]*)\\])";

        public Translation AddTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string createdBy)
        {
            throw new NotImplementedException();
        }

        public void DeletePhrase(int phraseId)
        {
            throw new NotImplementedException();
        }

        public void DeletePhraseReference(int referenceId)
        {
            throw new NotImplementedException();
        }

        public List<Phrase> FindPhrases(string languageCode, string searchText, string source, int status, int pageNo, int pageSize, ref int? totalPhraseCount)
        {
            throw new NotImplementedException();
        }

        public List<Language> GetAllLanguages()
        {
            return new List<Language>() { GetEnglishLanguage() };
        }

        public List<Translation> GetAllTranslationsByLanguage(string languageCode)
        {
            throw new NotImplementedException();
        }

        public Language GetLanguageByCulture(string cultureCode)
        {
            throw new NotImplementedException();
        }

        public string GetLanguageCodeByCulture(string cultureCode)
        {
            throw new NotImplementedException();
        }

        public string GetLanguageCodeByName(string name)
        {
            throw new NotImplementedException();
        }

        public Language GetLanguageByName(string name)
        {
            throw new NotImplementedException();
        }

        public Language GetLanguage(string code)
        {
            throw new NotImplementedException();
        }

        public Language GetDefaultLanguage()
        {
            return GetEnglishLanguage();
        }

        public string GetDefaultLanguageCode()
        {
            return Language.LANG_DefaultCode;
        }

        public Phrase GetPhrase(int phraseId)
        {
            throw new NotImplementedException();
        }

        public Phrase GetPhraseByReferenceId(int referenceId)
        {
            throw new NotImplementedException();
        }

        public string GetPhraseTextByReferenceId(int referenceId, string languageCode)
        {
            throw new NotImplementedException();
        }

        public Phrase GetPhraseByText(string text)
        {
            throw new NotImplementedException();
        }

        public PhraseReference GetPhraseReference(int phraseReferenceId)
        {
            throw new NotImplementedException();
        }

        public Translation GetTranslation(int translationId)
        {
            throw new NotImplementedException();
        }

        public Translation GetTranslation(int phraseId, string languageCode)
        {
            throw new NotImplementedException();
        }

        public bool IsCultureSupported(string cultureCode)
        {
            return (cultureCode == Language.LANG_DefaultCode);
        }

        public string LocaliseText(string source, string language)
        {
            StringBuilder sBuilder = new StringBuilder(source);
            LocaliseText(sBuilder, language);
            return sBuilder.ToString();
        }

        public void LocaliseText(StringBuilder source, string language)
        {
            Regex findLangRegex = new Regex(RegExFindLangReference, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //Regular Expression Replacements
            MatchCollection matches = findLangRegex.Matches(source.ToString());

            //Iterate through all the matches
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                //Pull text name from the group
                string mlid = matches[i].Groups["mlid"].Value;
                string replacementValue = matches[i].Groups["text"].Value;

                Match m = matches[i];
                //Make replacements
                source.Remove(m.Index, m.Length);
                source.Insert(m.Index, replacementValue);
            }
        }

        public string RegisterAndWrapPhraseText(string text, string referenceName, string createdBy, MLangReferenceFormat format)
        {
            throw new NotImplementedException();
        }

        public int RegisterPhraseText(string text, string referenceName, string createdBy)
        {
            throw new NotImplementedException();
        }

        public void RegisterTranslation(int phraseId, string translationText, string languageCode, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDefaultTranslation(int referenceId, string text, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public Phrase UpdatePhraseText(int phraseId, string phraseText, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public Translation UpdateTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public string WrapPhrase(int referenceId, MLangReferenceFormat format)
        {
            throw new NotImplementedException();
        }

        public string WrapPhrase(string mlid, string text, MLangReferenceFormat format)
        {
            throw new NotImplementedException();
        }

        private Language GetEnglishLanguage()
        {
            return new Language(Language.LANG_DefaultCode, Language.LANG_DefaultName, true, 0);
        }


        public List<Region> GetAllRegions()
        {
            return new List<Region>() { new Region() };
        }

        public Region GetRegion(string regionCode)
        {
            return new Region();
        }
    }
}
