namespace Customer.Wallet.Core.Domain
{
    using System;

    public class AccountExistsException : Exception
    {
        public AccountExistsException(int userId)
        {
            this.UserId = userId;
        }

        public int UserId { get; private set; }
    }
}