namespace Application.Interfaces;
public interface IPasswordHasher
{
    string Hash(string plaintext);
    bool Verify(string hash, string plaintext);
}
