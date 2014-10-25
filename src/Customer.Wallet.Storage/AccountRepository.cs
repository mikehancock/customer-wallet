namespace Customer.Wallet.Storage.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Customer.Wallet.Core;
    using Customer.Wallet.Core.Domain;

    using Raven.Client;

    public class AccountRepository : IRepository<Account>
    {
        private readonly IDocumentSession session;

        public AccountRepository(IDocumentSession session)
        {
            this.session = session;
        }

        public Account FindOne(Expression<Func<Account, bool>> predicate)
        {
            return this.session.Query<Account>().Where(predicate).FirstOrDefault();
        }

        public IQueryable<Account> FindBy(Expression<Func<Account, bool>> predicate)
        {
            return this.session.Query<Account>().Where(predicate);
        }

        public IEnumerable<Account> FindAll()
        {
            return this.session.Query<Account>();
        }

        public bool Exists(Expression<Func<Account, bool>> predicate)
        {
            return this.session.Query<Account>().Any(predicate);
        }

        public int Create(Account aggregateRoot)
        {
            this.session.Store(aggregateRoot);
            this.session.SaveChanges();
            return aggregateRoot.Id;
        }

        public void Update(Account aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public void Delete(Account aggregateRoot)
        {
            this.session.Delete(aggregateRoot);
            this.session.SaveChanges();
        }
    }
}