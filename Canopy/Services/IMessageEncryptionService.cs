namespace Canopy.Services
{
    public interface IMessageEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
