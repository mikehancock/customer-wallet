namespace Customer.Wallet.Web.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Customer.Wallet.Core;
    using Customer.Wallet.Core.Domain;

    public class WalletController : ApiController
    {
        private readonly IRepository<Account> store;

        public WalletController(IRepository<Account> store)
        {
            this.store = store;
        }

        public decimal Get(int id)
        {
            return this.store.FindOne(account => account.Id == id).Balance;
        }

        public HttpResponseMessage Post([FromBody]int userId)
        {
            var exists = this.store.Exists(account => account.UserId == userId);

            if (exists)
            {
                throw new AccountExistsException(userId);
            }

            var newAccount = new Account(0, userId, 0m);

            this.store.Create(newAccount);

            var response = Request.CreateResponse(HttpStatusCode.Created, newAccount);

            var uri = Url.Link("DefaultApi", new { id = newAccount.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        [HttpPut]
        public void Withdraw(int id, [FromBody] decimal amount)
        {
            var account = this.store.FindOne(a => a.Id == id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            account = account.Withdraw(amount);

            this.store.Update(account);
        }

        [HttpPut]
        public void Deposit(int id, [FromBody] decimal amount)
        {
            var account = this.store.FindOne(a => a.Id == id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            account = account.Deposit(amount);

            this.store.Update(account);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var account = this.store.FindOne(a => a.Id == id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            if (account.Balance != 0m)
            {
                throw new InvalidOperationException("cannot delete an account with a non-zero balance");
            }

            this.store.Delete(account);
        }
    }
}
