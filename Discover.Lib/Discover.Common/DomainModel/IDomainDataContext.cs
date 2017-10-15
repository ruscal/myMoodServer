using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.DomainModel
{
    /// <summary>
    /// Defines the interface that must be supported by a data storage provider in order to persist and locate/query domain entities 
    /// </summary>
    public interface IDomainDataContext
    {
        IQueryable<T> Get<T>() where T : class, IEntity;

        T Add<T>(T entity) where T : class, IEntity;

        void Remove<T>(T entity) where T : class, IEntity;

        int SaveChanges();
    }
}
