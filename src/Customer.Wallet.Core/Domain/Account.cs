namespace Customer.Wallet.Core.Domain
{
    using System;

    public class Account
    {
        public Account(int id, int userId, decimal balance)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException("userId");

            if (balance < 0)
                throw new InvalidOperationException("balance cannot be negative");

            this.Id = id;
            this.UserId = userId;
            this.Balance = balance;
        }

        public int Id { get; private set; }

        public int UserId { get; private set; }

        public decimal Balance { get; private set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount");

            if (amount > this.Balance)
                throw new InvalidOperationException(string.Format("withdrawal amount {0} exceeds balance {1}", amount, this.Balance));

            // I would prefer to return a new instance of Account and make this immutable, but that cuases issues with Raven updates
            this.Balance -= amount;
        }

        public void Deposit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }

            // I would prefer to return a new instance of Account and make this immutable, but that cuases issues with Raven updates
            this.Balance += amount;
        }
    }
}