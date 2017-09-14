using System;
using System.Security.Cryptography;
using System.Web;

namespace LanPlatform.Auth
{
    public class AuthSession : AuthInfo
    {
        public String Key { get; set; }
        public long ExpireDate { get; set; }

        public AuthSession()
        {
            Key = "";
            ExpireDate = 0;
        }

        public bool Authenticate(long id, String key)
        {
            return id == Id && (ExpireDate == 0 || Engine.EngineUtil.CurrentTime < ExpireDate) && Key.Equals(key, StringComparison.OrdinalIgnoreCase);
        }

        public static long GetIdFromCookie(HttpCookie cookie)
        {
            long id = 0;

            Int64.TryParse(cookie.Value, out id);

            return id;
        }

        public static String GenerateKey()
        {
            String key = "";
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            byte[] randBytes = new byte[17];

            rand.GetBytes(randBytes);

            for (int x = 0; x < 17; x++)
            {
                int letter = randBytes[x] % 52;

                char symbol = (letter > 25) ? (char) (letter + 71) : (char) (letter + 65);

                key += symbol;
            }

            return key;
        }
    }
}