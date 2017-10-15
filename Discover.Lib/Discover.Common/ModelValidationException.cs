using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Discover.Common
{
    public class ModelValidationException : ValidationException
    {
        
    }

    // Strongly-typed version permits lambda expression syntax to reference properties
    public class ModelValidationException<TModel> : ModelValidationException
    {
        public void ErrorFor<TProperty>(Expression<Func<TModel, TProperty>> property, string message)
        {
            Errors.Add(new RuleViolation { Property = property, Message = message });
        }
    }
}
