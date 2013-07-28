using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics.UnitTests
{
    class InMemoryDbSet<T> : IDbSet<T> where T : class
    {
        private readonly List<T> _data = new List<T>();

        public InMemoryDbSet()
        {
            var queryable = _data.AsQueryable();
            ElementType = queryable.ElementType;
            Expression = queryable.Expression;
            Provider = queryable.Provider;
            Local = new DbLocalView<T>(_data);
        } 

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
        public T Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public T Remove(T entity)
        {
            if (_data.Contains(entity))
                _data.Remove(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            return Add(entity);
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public DbLocalView<T> Local { get; private set; }
    }
}