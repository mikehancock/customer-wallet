namespace Customer.Wallet.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IRepository<T>
    {
        T FindOne(Expression<Func<T, bool>> predicate);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        IEnumerable<T> FindAll();

        bool Exists(Expression<Func<T, bool>> predicate);

        int Create(T aggregateRoot);

        void Update(T aggregateRoot);

        void Delete(T aggregateRoot);
    }
}

