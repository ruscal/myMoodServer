using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.DomainModel
{
    /// <summary>
    /// Defines the minimal interface that an domain entity must implement
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; }
    }
}
