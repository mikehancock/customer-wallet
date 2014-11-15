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

        public HttpResponseMessage Get(int id)
        {
            var account = this.store.Get(id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            return Request.CreateResponse(HttpStatusCode.Found, account);
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

            return Request.CreateResponse(HttpStatusCode.Created, newAccount.Id);
        }

        [HttpPut]
        [Route("api/Wallet/{id:int}/Withdraw")]
        public void Withdraw(int id, [FromBody] decimal amount)
        {
            var account = this.store.Get(id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            account.Withdraw(amount);

            this.store.Update(account);
        }

        [HttpPut]
        [Route("api/Wallet/{id:int}/Deposit")]
        public void Deposit(int id, [FromBody] decimal amount)
        {
            var account = this.store.Get(id);

            if (account == null)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            account.Deposit(amount);

            this.store.Update(account);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var account = this.store.Get(id);

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
