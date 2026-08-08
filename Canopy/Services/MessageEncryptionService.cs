using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace Canopy.Services
{
    public class MessageEncryptionService : IMessageEncryptionService
    {
        private readonly IDataProtector _protector;

        public MessageEncryptionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Canopy.ChatMessages.v1");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            try
            {
                return _protector.Unprotect(cipherText);
            }
            catch (CryptographicException)
            {
                return "[message not availible]";
            }
        }
    }

}
