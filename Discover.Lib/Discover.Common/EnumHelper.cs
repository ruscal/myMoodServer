using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Discover.Common
{
    public static class EnumHelper
    {
        public static IEnumerable<KeyValuePair<string, T>> GetNameValuePairsFor<T>()
        {
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                yield return new KeyValuePair<string, T>(Enum.GetName(typeof(T), item), (T)item);
            }
        }

        public static IEnumerable<T> GetValuesFor<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<SelectListItem> GetSelectListItemsFor<T>()
        {
            return ToSelectListItems(GetValuesFor<T>(), null);
        }

        public static IEnumerable<SelectListItem> GetSelectListItemsFor<T>(Func<T, bool> isSelectedPredicate)
        {
            return ToSelectListItems(GetValuesFor<T>(), isSelectedPredicate);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> enumValues)
        {
            return ToSelectListItems(enumValues, null);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> enumValues, Func<T, bool> isSelectedPredicate)
        {
            return enumValues.Select(v => new SelectListItem { Text = GetEnumMemberDisplayName(v), Value = v.ToString(), Selected = isSelectedPredicate != null ? isSelectedPredicate(v) : false });
        }

        public static string DisplayName(this Enum value)
        {
            return GetEnumMemberDisplayName(value);
        }

        public static string ValueString(this Enum value)
        {
            return Convert.ToInt32(value).ToString();
        }

        private static string GetEnumMemberDisplayName(object value)
        {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue).First();
            var displayAttrib = member.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

            return (displayAttrib == null) ?
                value.ToString() :
                displayAttrib.ResourceType == null ? displayAttrib.Name : displayAttrib.GetName();
        }
    }
}
