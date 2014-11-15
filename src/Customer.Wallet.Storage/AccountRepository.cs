namespace Customer.Wallet.Storage
{
    using System;
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
            this.session.Advanced.UseOptimisticConcurrency = true;
        }

        public Account Get(int id)
        {
            var account = this.session.Load<Account>(id);
            return account;
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
            this.session.SaveChanges();
        }

        public void Delete(Account aggregateRoot)
        {
            this.session.Delete(aggregateRoot);
            this.session.SaveChanges();
        }
    }
}