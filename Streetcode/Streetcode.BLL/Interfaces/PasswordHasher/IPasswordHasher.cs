using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.Interfaces.PasswordHasher
{
    public interface IPasswordHasher
    {
        void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }
}
