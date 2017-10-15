using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Discover.Emailing.Config
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class AccountCollection : ConfigurationElementCollection
    {
        public int Count
        {
            get
            {
                return base.Count;
            }
        }

        public Account this[int index]
        {
            get
            {
                return base.BaseGet(index) as Account;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public Account this[string name]
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Name == name) return this[i];
                }
                return null;
            }

        }

        

        protected override ConfigurationElement CreateNewElement()
        {
            return new Account();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Account)element).Name;
        }
  

        public Account GetAccountByEmailAddress(string emailAddress)
        {
            if (Count > 0)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].EmailAddress.ToLower() == emailAddress.ToLower())
                    {
                        return this[i];
                    }
                }
            }
            return null;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

    }
}
