using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Data.Entity.Design.PluralizationServices;

namespace Discover.Common
{
    public static class StringHelper
    {
        private static readonly PluralizationService Pluralizer = PluralizationService.CreateService(new System.Globalization.CultureInfo("en-US"));

        private const string Ellipsis = "...";

        public static string Truncate(this string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength) return text;

            return (maxLength > Ellipsis.Length) ? text.Substring(0, maxLength - Ellipsis.Length) + Ellipsis : text.Substring(0, Math.Min(text.Length, maxLength));
        }

        public static string ToQueryString(Dictionary<string, string> dict)
        {
            if (dict == null) return "";
            return string.Join(@"&", Array.ConvertAll(dict.Keys.ToArray(), key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(dict[key]))));
        }

        public static Dictionary<string, string> ToDictionary(string queryString)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (queryString == "") return dict;
            Array sqarr = queryString.Split("&".ToCharArray());
            for (int i = 0; i < sqarr.Length; i++)
            {
                string[] pairs =
                sqarr.GetValue(i).ToString().Split("=".ToCharArray());
                dict.Add(HttpUtility.UrlDecode(pairs[0]), HttpUtility.UrlDecode(pairs[1]));
            }
            return dict;
        }

        public static string RegexReplace(this string text, string pattern, string replacement)
        {
            return text == null ? text : Regex.Replace(text, pattern, replacement);
        }

        public static string RegexReplace(this string text, string pattern, Func<Match, string> matchEvaluator)
        {
            return text == null ? text : Regex.Replace(text, pattern, new MatchEvaluator(matchEvaluator));
        }

        public static string ToAlphaNumeric(this string text)
        {
            return text == null ? text : Regex.Replace(text, "[^a-zA-Z0-9]", string.Empty);
        }

        public static string NullToEmpty(this string text)
        {
            return text ?? string.Empty;
        }

        public static string ToBase26(int number)
        {
            var result = string.Empty;

            number = Math.Abs(number);
            
            do
            {
                int remainder = number % 26;
                result = (char)(remainder + 'A') + result;
                number = (number - remainder) / 26;
            } 
            while (number > 0);

            return result;
        }

        public static int FromBase26(string number)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(number))
            {
                result = (number[0] - 'A');

                for (int i = 1; i < number.Length; i++)
                {
                    result *= 26;
                    result += (number[i] - 'A');
                }
            }

            return result;
        }

        public static bool IsNumber(this string text)
        {
            double number = 0;
            return text == null ? false : double.TryParse(text, out number);
        }

        public static string Pluralize(this string word)
        {
            return Pluralizer.Pluralize(word);
        }

        public static string Pluralize(this string word, int count)
        {
            return count == 1 ? Pluralizer.Singularize(word) : Pluralizer.Pluralize(word);
        }

        public static string Singularize(this string word)
        {
            return Pluralizer.Singularize(word);
        }

        public static string ReplaceReferences(this string source, Dictionary<string, string> references)
        {
            if (source != null && references != null && references.Keys.Count > 0)
            {
                Replacer rp = new Replacer(references);
                source = rp.Replace(source);
            }
            return source;
        }

        class Replacer
        {
            Dictionary<string, string> _dictionary = new Dictionary<string, string>();

            public Replacer(Dictionary<string, string> dictionary)
            {
                _dictionary = dictionary;
            }

            public string Replace(string source)
            {
                if (string.IsNullOrEmpty(source)) return "";
                Regex regex = new Regex("{(?<key>[\\w]+)}");
                MatchEvaluator ev = new MatchEvaluator(ReplaceReference);
                return regex.Replace(source, ev);
            }

            string ReplaceReference(Match m)
            {
                string key = m.Groups["key"].Value;
                if (_dictionary.Keys.Contains(key))
                {
                    return _dictionary[key];
                }
                return m.Value;
            }
        }
    }
}
