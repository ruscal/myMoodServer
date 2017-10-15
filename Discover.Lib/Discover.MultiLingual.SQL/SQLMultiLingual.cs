using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.MultiLingual.SQL.MultiLingualDataTableAdapters;
using Discover.Logging;
using System.Text.RegularExpressions;

namespace Discover.MultiLingual.SQL
{
    public class SqlMultiLingual : IMultiLingual
    {
        public SqlMultiLingual(ILogger logger)
        {
            _logger = logger;
        }

        private ILogger _logger;
        private static readonly Regex FindLangReferenceRegex = new Regex("(<mlang[ ]*mlid=\\\"(?<mlid>[\\d@!]*)\\\"[ ]*((>(?<text>[^<]*)</mlang>)|(/>)))|(<mlang[ ]*mlid=(?<mlid>[\\d@!]*)[ ]*((>(?<text>[^<]*)</mlang>)|(/>)))|(@mlang\\[(?<mlid>[\\d@!]+)\\]\\[(?<text>[^]]*)\\])", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        
        #region Translation Caching Mechanisms

        private Dictionary<string, string> _cachedTranslations = null;
        private object _lock = new object();

        private IDictionary<string, string> CachedTranslations
        {
            get
            {
                if (_cachedTranslations == null)
                {
                    lock (_lock)
                    {
                        if (_cachedTranslations == null)
                        {
                            _cachedTranslations = (System.Web.HttpRuntime.Cache != null ? System.Web.HttpRuntime.Cache["Discover.MultiLingual.SQL.SqlMultiLingual.TranslatedPhrases"] as Dictionary<string, string> : null) ?? BuildTranslationCache();
                        }
                    }
                }
                return _cachedTranslations;
            }
        }

        private Dictionary<string, string> BuildTranslationCache()
        {
            var items = new Dictionary<string, string>();

            var translations = new TranslationTableAdapter().GetAllTranslations().Select(t => MultiLingualHelper.ToTranslationDTO(t)).ToArray();
            
            foreach (var phraseReference in new PhraseReferenceTableAdapter().GetAllPhraseReferences())
            {
                foreach (var phraseTranslation in translations.Where(t => t.PhraseId == phraseReference.phraseId))
                {
                    items.Add(phraseReference.id.ToString() + phraseTranslation.LanguageCode, phraseTranslation.Text);
                }
            }

            if (System.Web.HttpRuntime.Cache != null)
            {
                int cacheMinutes;
                if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings[""], out cacheMinutes)) cacheMinutes = 15;
                System.Web.HttpRuntime.Cache.Remove("Discover.MultiLingual.SQL.SqlMultiLingual.TranslatedPhrases");
                System.Web.HttpRuntime.Cache.Add("Discover.MultiLingual.SQL.SqlMultiLingual.TranslatedPhrases", items, null, DateTime.Now.AddMinutes(cacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }

            return items;
        }

        #endregion

        public bool Enabled
        {
            get
            {
                return MultiLingualConfigSection.Current.Enabled;
            }
        }

        public string DefaultLanguageCode
        {
            get
            {
                return MultiLingualConfigSection.Current.DefaultLanguage;
            }
        }

        public Translation AddTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string createdBy)
        {
            if (!Enabled) throw new TranslationException("Cannot add translation - multilingual service is disabled.");

            Translation trans = GetTranslation(phraseId, languageCode);
            if (trans != null) throw new TranslationException(string.Format("Could not add translation - a translation for that language already exists phraseId=[{0}] langeague=[{1}]", phraseId, languageCode));

            TranslationTableAdapter adapter = new TranslationTableAdapter();
            int? id = 0;
            adapter.Insert(phraseId, languageCode, translationText, (int)status, DateTime.Now, createdBy, DateTime.Now, createdBy, ref id);

            return GetTranslation((int)id);

        }

        public void DeletePhrase(int phraseId)
        {
            if (!Enabled) throw new TranslationException("Cannot delete phrase - multilingual service is disabled.");

            PhraseTableAdapter adapter = new PhraseTableAdapter();
            adapter.Delete(phraseId);
        }

        public void DeletePhraseReference(int referenceId)
        {
            if (!Enabled) throw new TranslationException("Cannot delete phrase reference - multilingual service is disabled.");

            PhraseReferenceTableAdapter adapter = new PhraseReferenceTableAdapter();
            adapter.Delete(referenceId);
        }

        public List<Phrase> FindPhrases(string languageCode, string searchText, string source, int status, int pageNo, int pageSize, ref int? totalPhraseCount)
        {
            throw new NotImplementedException();
        }

        public List<Language> GetAllLanguages()
        {
            List<Language> langs = new List<Language>();
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetAllLanguages();
            foreach (MultiLingualData.LanguageRow row in table)
            {
                langs.Add(MultiLingualHelper.ToLanguageDTO(row));
            }
            return langs;
        }

        public List<Region> GetAllRegions()
        {
            List<Region> regions = new List<Region>();
            RegionTableAdapter adapter = new RegionTableAdapter();
            MultiLingualData.RegionDataTable table = adapter.GetAllRegions();
            foreach (MultiLingualData.RegionRow row in table)
            {
                regions.Add(MultiLingualHelper.ToRegionDTO(row));
            }
            return regions;
        }

        public List<Translation> GetAllTranslationsByLanguage(string languageCode)
        {
            List<Translation> translations = new List<Translation>();
            TranslationTableAdapter adapter = new TranslationTableAdapter();
            MultiLingualData.TranslationDataTable table = adapter.GetAllTranslationsByLanguage(languageCode);
            foreach (MultiLingualData.TranslationRow row in table)
            {
                translations.Add(MultiLingualHelper.ToTranslationDTO(row));
            }
            return translations;
        }

        public Language GetDefaultLanguage()
        {
            Language lang = GetLanguage(DefaultLanguageCode);
            return lang;
        }

        public string GetDefaultLanguageCode()
        {
            return DefaultLanguageCode;
        }

        public Language GetLanguage(string code)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguage(code);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToLanguageDTO(table[0]);
        }

        public Language GetLanguageByCulture(string cultureCode)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguageByCulture(cultureCode);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToLanguageDTO(table[0]);
        }

        public string GetLanguageCodeByCulture(string cultureCode)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguageByCulture(cultureCode);
            if (table.Count == 0) return null;
            return table[0].code;
        }

        public string GetLanguageCodeByName(string name)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguageByName(name);
            if (table.Count == 0) return null;
            return table[0].code;
        }

        public Language GetLanguageByName(string name)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguageByName(name);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToLanguageDTO(table[0]);
        }

        public Phrase GetPhrase(int phraseId)
        {
            PhraseTableAdapter adapter = new PhraseTableAdapter();
            MultiLingualData.PhraseDataTable table = adapter.GetPhrase(phraseId);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToPhraseDTO(table[0], DefaultLanguageCode);
        }

        public Phrase GetPhraseByReferenceId(int referenceId)
        {
            PhraseTableAdapter adapter = new PhraseTableAdapter();
            MultiLingualData.PhraseDataTable table =  adapter.GetPhraseByReferenceId(referenceId);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToPhraseDTO(table[0], DefaultLanguageCode);
        }

        public Phrase GetPhraseByText(string text)
        {
            PhraseTableAdapter adapter = new PhraseTableAdapter();
            MultiLingualData.PhraseDataTable table = adapter.GetPhraseByText(text);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToPhraseDTO(table[0], DefaultLanguageCode);
        }

        public string GetPhraseTextByReferenceId(int referenceId, string languageCode)
        {
            TranslationTableAdapter adapter = new TranslationTableAdapter();
            MultiLingualData.TranslationDataTable table = adapter.GetTranslationByReferenceId(referenceId, languageCode);
            if (table.Count > 0)
            {
                Translation tran = MultiLingualHelper.ToTranslationDTO(table[0]);
                return tran.Text;
            }
            _logger.Error(this.GetType(), "MultiLingual error - could not find translation.  referenceId=[{0}] language=[{1}]", referenceId, languageCode);
            //get phrase and default text
            Phrase phrase = GetPhraseByReferenceId(referenceId);
            if (phrase != null) return phrase.DefaultPhrase;
            return "";
        }

        public PhraseReference GetPhraseReference(int phraseReferenceId)
        {
            if (phraseReferenceId <= 0) return null;
            PhraseReferenceTableAdapter adapter = new PhraseReferenceTableAdapter();
            MultiLingualData.PhraseReferenceDataTable table = adapter.GetPhraseReference(phraseReferenceId);
            if (table.Count > 0)
            {
                return new PhraseReference(phraseReferenceId, GetPhrase(table[0].phraseId));
            }
            return null;
        }

        public Region GetRegion(string regionCode)
        {
            if (string.IsNullOrEmpty(regionCode)) return new Region();
            RegionTableAdapter adapter = new RegionTableAdapter();
            MultiLingualData.RegionDataTable table = adapter.GetRegion(regionCode);
            if (table.Count == 0) return new Region();
            return MultiLingualHelper.ToRegionDTO(table[0]);
        }

        public Translation GetTranslation(int translationId)
        {
            TranslationTableAdapter adapter = new TranslationTableAdapter();
            MultiLingualData.TranslationDataTable table = adapter.GetTranslation(translationId);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToTranslationDTO(table[0]);
        }

        public Translation GetTranslation(int phraseId, string languageCode)
        {
            TranslationTableAdapter adapter = new TranslationTableAdapter();
            MultiLingualData.TranslationDataTable table = adapter.GetTranslationByPhraseId(phraseId, languageCode);
            if (table.Count == 0) return null;
            return MultiLingualHelper.ToTranslationDTO(table[0]);
        }

        public bool IsCultureSupported(string cultureCode)
        {
            LanguageTableAdapter adapter = new LanguageTableAdapter();
            MultiLingualData.LanguageDataTable table = adapter.GetLanguageByCulture(cultureCode);
            if (table.Count == 0) return false;
            return table[0].enabled;
        }

        public string LocaliseText(string source, string language)
        {
            if(string.IsNullOrWhiteSpace(source)) return "";
            return FindLangReferenceRegex.Replace(source, new MatchEvaluator(m =>
                {
                    string mlid = m.Groups["mlid"].Value;
                    string defaultText = m.Groups["text"].Value;

                    return CachedTranslations.ContainsKey(mlid + language) ? CachedTranslations[mlid + language] : defaultText;
                }));
        }

        public void LocaliseText(StringBuilder source, string language)
        {
            if (source == null) return;
            var text = source.ToString();
            source.Clear();
            source.Append(LocaliseText(text, language));
        }

        public string RegisterAndWrapPhraseText(string text, string referenceName, string createdBy, MLangReferenceFormat format)
        {
            if (text != "")
            {
                string wrapper = "";
                switch (format)
                {
                    case MLangReferenceFormat.Html:
                        wrapper = MLangHelper.MLangHtmlWrapperFormat;
                        text = text.Replace("<", "&lt;");
                        break;
                    case MLangReferenceFormat.NonHtml:
                        wrapper = MLangHelper.MLangNonHtmlWrapperFormat;
                        text = text.Replace("[", "&#91;");
                        text = text.Replace("]", "&#93;");
                        break;

                }

                int mlid = RegisterPhraseText(text, referenceName, createdBy);
                return string.Format(wrapper, (mlid == 0) ? "@" : mlid.ToString(), text);
            } return "";
        }

        public int RegisterPhraseText(string text, string referenceName, string createdBy)
        {
            if (text != "")
            {
                    Phrase p = GetPhraseByText(text);
                    // if phrase already exists just create a new reference otherwise add phrase
                    if (p == null)
                    {
                        PhraseTableAdapter adapter = new PhraseTableAdapter();
                        int? id = 0;
                        adapter.Insert(text, "", "", DateTime.Now, createdBy, DateTime.Now, createdBy, ref id);
                        
                        //add translations
                        List<Language> langs = GetAllLanguages();
                        foreach (Language lang in langs)
                        {
                            if (lang.Code == DefaultLanguageCode)
                            {
                                AddTranslation((int)id, lang.Code, text, TranslationStatus.Translated, createdBy);
                            }
                            else
                            {
                                AddTranslation((int)id, lang.Code, text, TranslationStatus.AwaitingTranslation, createdBy);
                            }
                        }
                        p = GetPhrase((int)id);
                    }

                    PhraseReferenceTableAdapter refAdapter = new PhraseReferenceTableAdapter();
                    int? refId = 0;
                    refAdapter.Insert(p.Id, referenceName, ref refId);
                    return (int)refId;
            } return 0;
        }

        public void RegisterTranslation(int phraseId, string translationText, string languageCode, string updatedBy)
        {
            Translation tran = GetTranslation(phraseId, languageCode);
            if (tran == null)
            {
                AddTranslation(phraseId, languageCode, translationText, TranslationStatus.Translated, updatedBy);
            }
            else
            {
                UpdateTranslation(phraseId, languageCode, translationText, TranslationStatus.Translated, updatedBy);
            }
        }

        public bool UpdateDefaultTranslation(int referenceId, string text, string updatedBy)
        {
            PhraseTableAdapter adapter = new PhraseTableAdapter();
            MultiLingualData.PhraseDataTable table = adapter.GetPhraseByReferenceId(referenceId);
            if (table.Count == 0) return false;
            MultiLingualData.PhraseRow row = table[0];
            row.defaultPhrase = text;
            row.lastEditedBy = updatedBy;
            adapter.Update(row);
            UpdateTranslation(row.id, DefaultLanguageCode, text, TranslationStatus.Translated, updatedBy);
            return true;
        }

        public Phrase UpdatePhraseText(int phraseId, string text, string updatedBy)
        {
            PhraseTableAdapter adapter = new PhraseTableAdapter();
            MultiLingualData.PhraseDataTable table = adapter.GetPhrase(phraseId);
            if (table.Count == 0) return null;
            MultiLingualData.PhraseRow row = table[0];
            row.defaultPhrase = text;
            row.lastEditedBy = updatedBy;
            adapter.Update(row);
            UpdateTranslation(row.id, DefaultLanguageCode, text, TranslationStatus.Translated, updatedBy);
            return MultiLingualHelper.ToPhraseDTO(row, DefaultLanguageCode);
        }

        public Translation UpdateTranslation(int phraseId, string languageCode, string translationText, TranslationStatus status, string updatedBy)
        {
            TranslationTableAdapter adapter = new TranslationTableAdapter();
            MultiLingualData.TranslationDataTable table = adapter.GetTranslationByPhraseId(phraseId, languageCode);
            if (table.Count == 0) return null;
            MultiLingualData.TranslationRow row = table[0];
            row.translationText = translationText;
            row.status = (int)status;
            row.lastEditedBy = updatedBy;
            adapter.Update(row);
            return MultiLingualHelper.ToTranslationDTO(row);
        }

        public string WrapPhrase(int referenceId, MLangReferenceFormat format)
        {
            Phrase p = GetPhraseByReferenceId(referenceId);
            string text = p==null ? "" : p.DefaultPhrase;
            return WrapPhrase(referenceId.ToString(), text, format);
        }

        public string WrapPhrase(string mlid, string text, MLangReferenceFormat format)
        {
            string wrapper = "";
            switch (format)
            {
                case MLangReferenceFormat.Html:
                    wrapper = MLangHelper.MLangHtmlWrapperFormat;
                    text = text.Replace("<", "&lt;");
                    break;
                case MLangReferenceFormat.NonHtml:
                    wrapper = MLangHelper.MLangNonHtmlWrapperFormat;
                    text = text.Replace("[", "&#91;");
                    text = text.Replace("]", "&#93;");
                    break;

            }
            return string.Format(wrapper, mlid.ToString(), text);
        }

        public void InitDatabaseSchema()
        {
            var commands = new string[0];

            using (var script = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetName().Name + ".SqlMultiLingual.sql")))
            {
                commands = script.ReadToEnd().Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            }

            using (var adapter = new LanguageTableAdapter())
            {
                adapter.Connection.Open();

                var cmd = adapter.Connection.CreateCommand();

                foreach (var command in commands)
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
