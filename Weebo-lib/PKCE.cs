using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Weebo_lib
{
    public static class PKCE
    {
        public static class code_challenge_method
        {
            public static string Method = plain;
            public const string plain = "plain";
            public const string S256 = "S256";
        }
        public static (string, string) GeneratePKCE(string CodeChallengeMethod = code_challenge_method.plain)
        //https://datatracker.ietf.org/doc/html/rfc7636#section-4
        {
            string code_verifier;
            string code_challenge;

            //Code Verifier
            Random RNG = new Random();
            const string range = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
            code_verifier = (string)Enumerable.Range(0, 20).Select(x => range[RNG.Next(0, range.Length)]);
            //Assume Code Challenge (method = plain)
            code_challenge = code_verifier;
            //Code Challenge (method = S256)
            if (CodeChallengeMethod == code_challenge_method.S256)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    code_challenge = Base64UrlEncode(GetHash(sha256Hash, code_verifier));
                }
            }
            return (code_verifier, code_challenge);
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        //https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=net-6.0#System_Security_Cryptography_HashAlgorithm_ComputeHash_System_Byte
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private static string Base64UrlEncode(string input)
        //https://stackoverflow.com/questions/30246497/using-statement-for-base64urlencode
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }
}
