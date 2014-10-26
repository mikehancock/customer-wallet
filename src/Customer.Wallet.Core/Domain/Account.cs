namespace Customer.Wallet.Core.Domain
{
    using System;

    public class Account
    {
        public Account(int id, int userId, decimal balance)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (balance < 0)
            {
                throw new InvalidOperationException("balance cannot be negative");
            }

            this.Id = id;
            this.UserId = userId;
            this.Balance = balance;
        }

        public int Id { get; private set; }

        public int UserId { get; private set; }

        public decimal Balance { get; private set; }

        public Account Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }

            return new Account(this.Id, this.UserId, this.Balance - amount);
        }

        public Account Deposit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }

            return new Account(this.Id, this.UserId, this.Balance + amount);
        }
    }
}