namespace Customer.Wallet.Tests.IntegrationTests
{
    using Customer.Wallet.Core.Domain;
    using Customer.Wallet.Storage;

    using NUnit.Framework;

    using Raven.Abstractions.Exceptions;
    using Raven.Client.Document;

    [TestFixture]
    public class ConcurrencyTests
    {
        [Test]
        public void ConcurrentUpdatesWillThrowAConcurrencyException()
        {
            using (var store = new DocumentStore { Url = "http://localhost:8080/", DefaultDatabase = "Wallet" })
            {
                store.Initialize();

                var accountId = 0;
                using (var s = store.OpenSession())
                {
                    accountId = new AccountRepository(s).Create(new Account(0, 99, 100m));
                }

                using (var session1 = store.OpenSession())
                using (var session2 = store.OpenSession())
                {
                    session1.Advanced.UseOptimisticConcurrency = true;
                    session2.Advanced.UseOptimisticConcurrency = true;

                    var repo1 = new AccountRepository(session1);
                    var repo2 = new AccountRepository(session2);

                    var account1 = repo1.Get(accountId);
                    var account2 = repo2.Get(accountId);

                    account1.Deposit(1m);
                    repo1.Update(account1);

                    account2.Deposit(2m);

                    // Saving the second text will throw a concurrency exception
                    Assert.Throws<ConcurrencyException>(() => repo2.Update(account2));
                }

                using (var s = store.OpenSession())
                {
                    var repo = new AccountRepository(s);
                    var account = repo.Get(accountId);
                    Assert.That(101m, Is.EqualTo(account.Balance));
                    repo.Delete(account);
                }
            }
        }
    }
}
