using Application.Interfaces;
using BCrypt.Net;

namespace Infrastructure.Security;
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plaintext) => BCrypt.Net.BCrypt.HashPassword(plaintext);
    public bool Verify(string hash, string plaintext) => BCrypt.Net.BCrypt.Verify(plaintext, hash);
}