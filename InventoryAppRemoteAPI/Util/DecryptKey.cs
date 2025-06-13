
using System.Security.Cryptography;
using System.Text;

namespace InventoryAppRemoteAPI.Util
{
    /// <summary>
    /// Handles API key decryption and validation.
    /// Author: Shannon Musgrave
    /// </summary>
    public class DecryptKey
    {
        private static string apikey = "";
        /// <summary>
        /// Configuration object used for retrieving stored encryption keys.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor that initializes the configuration dependency.
        /// </summary>
        /// <param name="config">Injected configuration instance.</param>
        public DecryptKey(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Retrieves a value from the application's configuration settings.
        /// </summary>
        /// <param name="key">The configuration key to retrieve.</param>
        /// <returns>The corresponding configuration value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the key is not found.</exception>
        private string GetJSONItem(string key)
        {
            string? retrieved = _config[key];

            if (string.IsNullOrEmpty(retrieved))
            {
                throw new InvalidOperationException($"The key '{key}' is not configured in the application settings.");
            }

            return retrieved;
        }

        /// <summary>
        /// Validates an encrypted API key by decrypting it and comparing it to the stored key.
        /// </summary>
        /// <param name="key">The encrypted API key received in a request.</param>
        /// <returns>True if the decrypted key matches the stored key; otherwise, false.</returns>
        public bool IsValidKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            return PerformCryptology(key);
        }

        /// <summary>
        /// Validates an encrypted API key by decrypting it and comparing it to the stored key.
        /// </summary>
        /// <param name="key">The encrypted API key received in a request.</param>
        /// <returns>True if the decrypted key matches the stored key; otherwise, false.</returns>

        private bool PerformCryptology(string key)
        {
            if(apikey.Length > 0)
            {
                return key.Equals(apikey);
            }

            string keyfromsettings = GetJSONItem("apikey");

            byte[] bytestoencrypt = Encoding.UTF8.GetBytes(keyfromsettings);
            using (Aes aes = Aes.Create())
            {
                // Retrieve stored encryption key and IV from configuration
                aes.Key = Encoding.UTF8.GetBytes(GetJSONItem("aeskey"));
                aes.IV = Encoding.UTF8.GetBytes(GetJSONItem("aesiv"));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;


                // Perform encryption
                using (ICryptoTransform cryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream())
                using (var cs = new CryptoStream(ms, cryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytestoencrypt, 0, bytestoencrypt.Length);
                    cs.FlushFinalBlock();
                    var convertedString = Convert.ToBase64String(ms.ToArray());
                    apikey = convertedString; // Cache the encrypted key for future use
                    return apikey.Equals(key);
                }
            }
        }
    }
}
