using System;
using System.Security.Cryptography;
using System.Text;

namespace LanPlatform.Auth
{
    public class AuthUsername : AuthInfo
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public String CryptPassword { get; set; }
        public String Salt { get; set; }

        public AuthUsername()
        {
            Username = "";
            Password = "";
            CryptPassword = "";
            Salt = "";
        }

        public bool Authenticate(String password)
        {
            String cryptPassword = EncryptPassword(password, Salt);

            return cryptPassword.Equals(CryptPassword);
        }

        public static AuthUsername CreateAuth(String username, String password)
        {
            AuthUsername auth = new AuthUsername();

            auth.Username = username;
            auth.Password = password;
            auth.Salt = GenerateSalt();
            auth.CryptPassword = EncryptPassword(password, auth.Salt);

            return auth;
        }

        public static String EncryptPassword(String password, String salt)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(password + salt);
            SHA256Managed crypto = new SHA256Managed();
            byte[] hashBytes = crypto.ComputeHash(passBytes);
            String hashString = "";

            foreach(byte x in hashBytes)
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }

        public static String GenerateSalt()
        {
            String salt = "";
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            byte[] randBytes = new byte[7];

            rand.GetBytes(randBytes);

            for (int x = 0; x < 7; x++)
            {
                char symbol = (char) (randBytes[x] % 94 + 33);

                salt += symbol;
            }

            return salt;
        }
    }
}