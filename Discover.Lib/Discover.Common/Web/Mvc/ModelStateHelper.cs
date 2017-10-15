using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Discover.Web.Mvc
{
    public static class ModelStateHelper
    {
        /// <summary>
        /// Adds the given validation error information to the model-state dictionary
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        public static void Add(this ModelStateDictionary modelState, IEnumerable<ValidationResult> errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(error.MemberNames.FirstOrDefault() ?? string.Empty, error.ErrorMessage);
            }
        }

        /// <summary>
        /// Adds the given validation error information to the model-state dictionary
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        /// <param name="modelBindPrefix"></param>
        public static void Add(this ModelStateDictionary modelState, IEnumerable<ValidationResult> errors, string modelBindPrefix)
        {
            foreach (var error in errors)
            {
                var memberName = error.MemberNames.First();

                if (!memberName.StartsWith(modelBindPrefix)) memberName = string.Concat(modelBindPrefix, ".", memberName);

                modelState.AddModelError(memberName, error.ErrorMessage);
            }
        }

        /// <summary>
        /// Generates a set of objects (suitable for inclusion in a JsonResult) containing information about any errors in the model-state dictionary 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static object[] ToErrorInfoObjects(this ModelStateDictionary modelState)
        {
            return (from m in modelState
                    where m.Value.Errors.Count > 0
                    select new
                    {
                        Key = m.Key,
                        AttemptedValue = m.Value.Value != null ? m.Value.Value.RawValue : null,
                        Errors = m.Value.Errors.Select(e => e.ErrorMessage).Concat(m.Value.Errors.Where(e => e.Exception != null).Select(e => e.Exception.Message)).ToArray()
                    })
                    .Cast<object>().ToArray();
        }

    }
}
