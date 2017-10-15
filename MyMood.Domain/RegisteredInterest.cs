using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class RegisteredInterest : Entity
    {
         protected RegisteredInterest()
            :base()
        {

        }

         public RegisteredInterest(Event evnt, string emailAddress) :
            this()
        {
            this.Event = evnt;
            this.EmailAddress = emailAddress;
        }

         public string EmailAddress { get; set; }

         public virtual Event Event { get; set; }
    }
}
