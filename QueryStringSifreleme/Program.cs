using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QueryStringSifreleme
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "Herşey Güzel Olacak! :)";

            var strEncryptred = Sifre.Sifrele(str);
            var strDecrypted = Sifre.SifreCoz(strEncryptred);

            Console.WriteLine($"Şifrelenmiş = {strEncryptred}");
            Console.WriteLine($"Çözülmüş = {strDecrypted}");

            Console.ReadKey();


        }


        public static class SafeUrl
        {

            private const string Plus = "+";
            private const string Minus = "-";
            private const string Slash = "/";
            private const string Underscore = "_";
            private const string EqualSign = "=";
            private const string Pipe = "|";

            private static readonly IDictionary<string, string> _mapper;

            static SafeUrl()
            {
                _mapper = new Dictionary<string, string>
        {
            { Plus, Minus },
            { Slash, Underscore },
            { EqualSign, Pipe }
        };
            }

            public static string EncodeBase64Url(string base64Str)
            {
                if (string.IsNullOrEmpty(base64Str)) return base64Str;

                foreach (var pair in _mapper)
                    base64Str = base64Str.Replace(pair.Key, pair.Value);

                return base64Str;
            }
            public static string DecodeBase64Url(string safe64Url)
            {
                if (string.IsNullOrEmpty(safe64Url)) return safe64Url;
                foreach (var pair in _mapper)
                    safe64Url = safe64Url.Replace(pair.Value, pair.Key);
                return safe64Url;
            }
        }

        public static class Sifre
        {

            public static string Sifrele(string data)
            {
                string IV = "2wDwCbJtSVuTlXhL";//16 karakterlik 128 bit lik bir anahtar 
                string KEY = "OZMd2MfM6YuoFNLXM50FpJdjX0R926GF"; //32 karakterlik 256 bit lik bir anahtar 
                byte[] buffer = null;

                Aes aes = Aes.Create();
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Key = Encoding.UTF8.GetBytes(KEY);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(data);
                        }
                    }
                    buffer = ms.ToArray();
                }
                return SafeUrl.EncodeBase64Url(Convert.ToBase64String(buffer));
            }

            public static string SifreCoz(string data)
            {
                string IV = "2wDwCbJtSVuTlXhL";//16 karakterlik 128 bit lik bir anahtar 
                string KEY = "OZMd2MfM6YuoFNLXM50FpJdjX0R926GF"; //32 karakterlik 256 bit lik bir anahtar 
                byte[] buffer = Convert.FromBase64String(SafeUrl.DecodeBase64Url(data));
                string result = null;

                Aes aes = Aes.Create();
                aes.IV = Encoding.UTF8.GetBytes(IV);
                aes.Key = Encoding.UTF8.GetBytes(KEY);

                ICryptoTransform encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            result = sr.ReadToEnd();
                        }
                    }
                }

                return result;
            }


        }
    }
}
