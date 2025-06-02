using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace InventoryAppRemoteAPI.Util
{
    public class DecryptKey
    {
        private const string api = "Bedyip5Wx+S/F7X3pjaxDS9c2YlJkE0yOTKfZPHOteo=";
        private const string aeskey = "5pxiTbC8Ty6H4Yp6FA5eGi5P82NC0yOh";
        private const string iv = "iOGfBfecKtjx62fk";

        public static bool IsValidKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            byte[] encryptedBytes = Convert.FromBase64String(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(aeskey);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream(encryptedBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new System.IO.StreamReader(cs))
                {
                    string testablekey = reader.ReadToEnd();
                    return testablekey.Equals(api);
                }
            }
        }

    }
}
