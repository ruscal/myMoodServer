using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.DomainModel;

namespace MyMood.Domain
{
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// The identity of this domain entity
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// The date and time that this domain entity was first created
        /// </summary>
        public DateTime CreationDate { get; protected set; }

        public DateTime LastEditedDate { get; set; }

        protected Entity()
        {
            Id = Discover.SequentialGuid.NewCombGuid();
            CreationDate = DateTime.UtcNow;
            LastEditedDate = DateTime.UtcNow;
        }

        public virtual void OverrideIdentity(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}
