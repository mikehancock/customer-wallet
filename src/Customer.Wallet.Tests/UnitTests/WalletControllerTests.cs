namespace Customer.Wallet.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Hosting;
    using System.Web.Http.Routing;

    using Customer.Wallet.Core;
    using Customer.Wallet.Core.Domain;
    using Customer.Wallet.Web.Controllers;

    using FakeItEasy;

    using NUnit.Framework;

    [TestFixture]
    public class WalletControllerTests
    {
        private WalletController controller;
        private IRepository<Account> accountRepository;
        private UrlHelper urlHelper;

        [SetUp]
        public void SetupBeforeEachTest()
        {
            this.accountRepository = A.Fake<IRepository<Account>>();
            this.controller = new WalletController(this.accountRepository);
            var context = A.Fake<HttpControllerContext>();

            var configuration = A.Fake<HttpConfiguration>();
            var route = configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "values" } });
            context.Configuration = configuration;
            context.RouteData = routeData;
            context.Controller = controller;

            this.controller.ControllerContext = context;
            this.controller.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:51018/api/values/");
            this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, context.Configuration);
            this.controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, context.RouteData);
            this.urlHelper = A.Fake<UrlHelper>();
            this.controller.Url = this.urlHelper;
            A.CallTo(() => this.urlHelper.Link("DefaultApi", A<object>._)).Returns("http://api/1");
        }

        public class Post : WalletControllerTests
        {
            [Test]
            public void UserIdCreatesAccount()
            {
                var userId = 1;

                var actual = this.controller.Post(userId);

                Assert.That(actual, Is.InstanceOf<HttpResponseMessage>());
            }

            [Test]
            public void NegativeUserIdThrowsArguementException()
            {
                var userId = -1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Post(userId));
            }

            [Test]
            public void ExistingUserIdThrowsExistingUserException()
            {
                var userId = 1;
                A.CallTo(() => this.accountRepository.Exists(A<Expression<Func<Account, bool>>>._)).Returns(true);

                Assert.Throws<AccountExistsException>(() => this.controller.Post(userId));
            }
        }

        public class Get : WalletControllerTests
        {
            [Test]
            public void GetExistingAccountReturnsBalance()
            {
                var accountId = 1;

                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                var actual = this.controller.Get(accountId);

                Assert.That(actual, Is.EqualTo(2m));
            }

            [Test]
            public void GetNonExistantAccountReturnsBalance()
            {
                var accountId = 1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Get(accountId));
            }
        }

        public class Delete : WalletControllerTests
        {
            [Test]
            public void ExistingAccountWithZeroBalanceIsDeleted()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 0m));

                this.controller.Delete(accountId);

                A.CallTo(() => this.accountRepository.Delete(A<Account>.That.Matches(a => a.UserId == accountId))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void ExistingAccountWithNonZeroBalanceThrowsException()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 1m));

                Assert.Throws<InvalidOperationException>(() => this.controller.Delete(accountId));
            }

            [Test]
            public void NonExistantAccountIdThrowsException()
            {
                var accountId = 1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Delete(accountId));
            }
        }

        public class Deposit : WalletControllerTests
        {
            [Test]
            public void DepositExistingAccountIncreasesBalance()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                this.controller.Deposit(accountId, 1m);

                A.CallTo(() => this.accountRepository.Update(A<Account>.That.Matches(a => a.Id == 1 && a.Balance == 3m))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void DepositZeroDoesNotChangeBalance()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                this.controller.Deposit(accountId, 0m);

                A.CallTo(() => this.accountRepository.Update(A<Account>.That.Matches(a => a.Id == 1 && a.Balance == 2m))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void DepositNegativeAmountThrowsException()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Deposit(accountId, -1m));
            }

            [Test]
            public void DepositNonExistantAccountThrowsException()
            {
                var accountId = 1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Deposit(accountId, 1m));
            }
        }
        
        public class Withdraw : WalletControllerTests
        {
            [Test]
            public void WithdrawExistingAccountIncreasesBalance()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                this.controller.Withdraw(accountId, 1m);

                A.CallTo(() => this.accountRepository.Update(A<Account>.That.Matches(a => a.Id == 1 && a.Balance == 1m))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void WithdrawZeroDoesNotChangeBalance()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                this.controller.Withdraw(accountId, 0m);

                A.CallTo(() => this.accountRepository.Update(A<Account>.That.Matches(a => a.Id == 1 && a.Balance == 2m))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void WithdrawNegativeAmountThrowsException()
            {
                var accountId = 1;
                A.CallTo(() => this.accountRepository.Get(accountId)).Returns(new Account(1, 1, 2m));

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Withdraw(accountId, -1m));
            }

            [Test]
            public void WithdrawNonExistantAccountThrowsException()
            {
                var accountId = 1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Withdraw(accountId, 1m));
            }
        }
    }
}
