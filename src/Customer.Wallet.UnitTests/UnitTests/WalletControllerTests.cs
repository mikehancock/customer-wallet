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

        public class Delete : WalletControllerTests
        {
            [Test]
            public void ExistingUserWithZeroBalanceIsDeleted()
            {
                var userId = 1;
                A.CallTo(() => this.accountRepository.FindOne(A<Expression<Func<Account, bool>>>._)).Returns(new Account(1, 1, 0m));

                this.controller.Delete(userId);

                A.CallTo(() => this.accountRepository.Delete(A<Account>.That.Matches(a => a.UserId == userId))).MustHaveHappened(Repeated.Exactly.Once);
            }

            [Test]
            public void ExistingUserWithNonZeroBalanceThrowsException()
            {
                var userId = 1;
                A.CallTo(() => this.accountRepository.FindOne(A<Expression<Func<Account, bool>>>._)).Returns(new Account(1, 1, 1m));

                Assert.Throws<InvalidOperationException>(() => this.controller.Delete(userId));
            }

            [Test]
            public void NonExistantUserIdThrowsException()
            {
                var userId = 1;

                Assert.Throws<ArgumentOutOfRangeException>(() => this.controller.Delete(userId));
            }
        }
    }
}
