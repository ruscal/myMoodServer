using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.MultiLingual
{
    public class TranslationException : Exception
    {
        public TranslationException(string message)
            :base(message)
        {
        }

        public TranslationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
