using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual.SQL
{
    class MultiLingualHelper
    {
        public static Translation ToTranslationDTO(MultiLingualData.TranslationRow row)
        {
            return new Translation(row.id, row.phraseId, row.languageCode, row.translationText, (TranslationStatus)Enum.Parse(typeof(TranslationStatus), row.status.ToString()), row.createdBy, row.dateCreated, row.lastEditedBy, row.dateLastEdited);
        }

        public static Language ToLanguageDTO(MultiLingualData.LanguageRow row)
        {
            return new Language(row.code, row.name, row.enabled, row.displayIndex);
        }

        public static Language ToLanguageDTO(MultiLingualData.LanguageRow row, List<string> supportedCultures)
        {
            return new Language(row.code, row.name, row.enabled, row.displayIndex, supportedCultures);
        }

        public static Phrase ToPhraseDTO(MultiLingualData.PhraseRow row, string defaultLanguageCode)
        {
            return new Phrase(defaultLanguageCode, row.id, row.defaultPhrase, row.description, row.source, row.createdBy, row.dateCreated, row.lastEditedBy, row.dateLastEdited);
        }

        public static Region ToRegionDTO(MultiLingualData.RegionRow row)
        {
            return new Region() { Code = row.Code, DateDiff = row.DateDiff, DateFormat = row.DateFormat, DefaultLanguage = row.DefaultLanguage, TimeCode = row.TimeCode };
        }
    }
}
