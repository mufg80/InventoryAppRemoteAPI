using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace InventoryAppRemoteAPI.Util
{
    public class DecryptKey
    {
        private readonly IConfiguration _config;
        public DecryptKey(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        private string GetJSONItem(string key)
        {
            string retrieved = _config[key];
            if (string.IsNullOrEmpty(retrieved))
            {
                throw new InvalidOperationException("API key is not configured in the application settings.");
            }
            return retrieved;
        }

        public bool IsValidKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            byte[] encryptedBytes = Convert.FromBase64String(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(GetJSONItem("aeskey"));
                aes.IV = Encoding.UTF8.GetBytes(GetJSONItem("iv"));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new System.IO.StreamReader(cs))
                {
                    string testablekey = reader.ReadToEnd();
                    return testablekey.Equals(GetJSONItem("apikey"));
                }
            }
        }

    }
}
