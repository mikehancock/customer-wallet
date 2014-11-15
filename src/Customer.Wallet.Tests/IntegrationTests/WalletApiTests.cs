namespace Customer.Wallet.UnitTests.IntegrationTests
{
    using System;
    using System.Net;

    using Customer.Wallet.Tests;

    using NUnit.Framework;

    [TestFixture(Category = "Integration")]
    public class WalletApiTests
    {
        private readonly Uri controllerUri = new Uri("http://localhost:50170/api/Wallet");
        private JsonRequestBuilder builder;
        private JsonClient jsonClient;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            
            this.builder = new JsonRequestBuilder().WithUri(controllerUri);
            this.jsonClient = new JsonClient();
        }

        [Test]
        public void CreateAccountForUser()
        {
            var response = this.jsonClient.Post(new JsonRequestBuilder().WithUri(this.controllerUri).WithBody(1));

            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }
            finally
            {
                this.jsonClient.Delete(new JsonRequestBuilder().WithUri(new Uri(controllerUri + "/" + response.Content)));
            }
        }

        [Test]
        public void GetAccountBalanceForUser()
        {
            var accountId = this.jsonClient.Post(new JsonRequestBuilder().WithUri(this.controllerUri).WithBody(1)).Content;
            var response = this.jsonClient.Get<Account>(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));

            try
            {
                Assert.That(response.Balance, Is.EqualTo(0m));
            }
            finally
            {
                this.jsonClient.Delete(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));
            }
        }

        [Test]
        public void DepositBalanceForUser()
        {
            var accountId = this.jsonClient.Post(new JsonRequestBuilder().WithUri(this.controllerUri).WithBody(1)).Content;
            this.jsonClient.Put(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId  + "/Deposit")).WithBody(1m));
            var response = this.jsonClient.Get<Account>(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));

            try
            {
                Assert.That(response.Balance, Is.EqualTo(1m));
            }
            finally
            {
                this.jsonClient.Put(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId + "/Withdraw")).WithBody(1m));
                this.jsonClient.Delete(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));
            }
        }

        [Test]
        public void WithdrawBalanceForUser()
        {
            var accountId = this.jsonClient.Post(new JsonRequestBuilder().WithUri(this.controllerUri).WithBody(1)).Content;
            this.jsonClient.Put(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId + "/Deposit")).WithBody(5m));
            this.jsonClient.Put(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId + "/Withdraw")).WithBody(2m));
            var response = this.jsonClient.Get<Account>(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));

            try
            {
                Assert.That(response.Balance, Is.EqualTo(3m));
            }
            finally
            {
                this.jsonClient.Put(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId + "/Withdraw")).WithBody(3m));
                this.jsonClient.Delete(new JsonRequestBuilder().WithUri(new Uri(this.controllerUri + "/" + accountId)));
            }
        }
    }
}
