namespace Customer.Wallet.UnitTests
{
    using System;

    using Customer.Wallet.Core.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class AccountTests
    {
        [TestCase(1)]
        [TestCase(99999999)]
        public void ConstructWithValidUserIdTest(int expected)
        {
            var actual = new Account(0, expected, 0m);

            Assert.That(actual.UserId, Is.EqualTo(expected));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ConstructWithInvalidUserId(int userId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Account(0, userId, 0m));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(999999999)]
        public void ConstructWithValidBalance(decimal expected)
        {
            var actual = new Account(0, 1, expected);

            Assert.That(actual.Balance, Is.EqualTo(expected));
        }

        [Test]
        public void WithdrawAmountLessThanBalance()
        {
            var account = new Account(0, 1, 3m);

            account.Withdraw(2m);

            Assert.That(account.Balance, Is.EqualTo(1m));
        }

        [Test]
        public void WithdrawZeroReturnsSameBalance()
        {
            var account = new Account(0, 1, 3m);

            account.Withdraw(0m);

            Assert.That(account.Balance, Is.EqualTo(3m));
        }

        [Test]
        public void WithdrawAmountLargerThanBalanceThrowsException()
        {
            var account = new Account(0, 1, 3m);

            Assert.Throws<InvalidOperationException>(() => account.Withdraw(5m));
        }

        [Test]
        public void DepositPositiveAmountToAccountReturnsAccountWithNewBalance()
        {
            var account = new Account(0, 1, 3m);

            account.Deposit(1m);

            Assert.That(account.Balance, Is.EqualTo(4m));
        }

        [Test]
        public void DepositNegativeAmountThrowsException()
        {
            var account = new Account(0, 1, 3m);

            Assert.Throws<ArgumentOutOfRangeException>(() => account.Deposit(-1m));
        }
    }
}
