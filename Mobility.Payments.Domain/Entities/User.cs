namespace Mobility.Payments.Domain.Entities
{
    using Mobility.Payments.Crosscuting.Enums;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class User : Entity
    {
        private const int saltSize = 16;
        private const int hashSize = 32;
        private const int iterations = 100000;
        private const decimal initialSenderBalance = 1000m;
        private const decimal initialReceiverBalance = 100m;
        private readonly HashAlgorithmName algorithmName = HashAlgorithmName.SHA512;

        public User() { }
        public User(string username, string password, string firstName, string lastName, UserRole userRole)
        {
            this.Username = username;
            this.PasswordHash = this.Hash(password);
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserRole = userRole;
            this.Balance = userRole == UserRole.Sender ? initialSenderBalance : initialReceiverBalance;
        }

        public virtual string Username { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<Payment> Sent { get; set; }
        public virtual ICollection<Payment> Received { get; set; }

        public decimal UpdateBalance(decimal value)
        {
            this.Balance += value;
            this.UpdateTrackValues();
            return this.Balance;
        }

        public bool VerifyPassword(string password)
        {
            var parts = this.PasswordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, hashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }

        private string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, algorithmName, hashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }
    }
}
