using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Discover.Common
{
    public class ValidationException : Exception
    {
        public ValidationException()
            :base()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
            ErrorForModel(message);
        }

        public readonly IList<RuleViolation> Errors = new List<RuleViolation>();
        private readonly static Expression<Func<object, object>> thisObject = x => x;

        public void ErrorForModel(string message)
        {
            Errors.Add(new RuleViolation { Property = thisObject, Message = message });
        }

        public class RuleViolation
        {
            public LambdaExpression Property { get; set; }
            public string Message { get; set; }
        }
    }

   
}
