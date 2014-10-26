namespace Customer.Wallet.UnitTests.IntegrationTests
{
    using System;
    using System.Net;

    using NUnit.Framework;

    [TestFixture(Category = "Integration")]
    public class WalletApiTests
    {
        private JsonRequestBuilder builder;
        private JsonClient jsonClient;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            var controllerUri = new Uri("http://localhost:50170/api/Wallet");
            this.builder = new JsonRequestBuilder().WithUri(controllerUri);
            this.jsonClient = new JsonClient();
        }

        [Test]
        public void CreateAccountForUser()
        {
            var request = this.builder.WithBody(1);
            var response = this.jsonClient.Post(request);

            try
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }
            finally
            {
                // For some reason delete isn't working, not sure   what is wrong as this should just be standard route - ran out time trying to figure this one out
                this.jsonClient.Delete(request);
            }
        }

        [Test]
        public void GetAccountBalanceForUser()
        {
            var request = this.builder.WithBody(99);
            this.jsonClient.Post(request);
            var response = this.jsonClient.Get<decimal>(request);

            Assert.That(response, Is.EqualTo(0m));

            this.jsonClient.Delete(request);
        }
    }
}
