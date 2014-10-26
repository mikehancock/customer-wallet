namespace Customer.Wallet.Core
{
    using System;
    using System.Linq.Expressions;

    public interface IRepository<T>
    {
        T Get(int id);

        bool Exists(Expression<Func<T, bool>> predicate);

        int Create(T aggregateRoot);

        void Update(T aggregateRoot);

        void Delete(T aggregateRoot);
    }
}

