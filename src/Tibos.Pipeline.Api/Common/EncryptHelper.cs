using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tibos.Pipeline.Api.Common
{
    /// <summary>
    /// 加密工具
    /// </summary>
    public class EncryptHelper
    {
        private static readonly byte[] AES_IV = Encoding.UTF8.GetBytes("0102030405060708");//初始向量

        /// <summary>
        /// AES 加密 支付宝SDK
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesEncrypt_Alipay(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = null;
            if (IsBase64String(encryptKey))
            {
                keyArray = Convert.FromBase64String(encryptKey);
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(encryptKey);
            }
            Byte[] toEncryptArray = Encoding.GetEncoding(charset).GetBytes(bizContent);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateEncryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// AES解密 支付宝SDK
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesDencrypt_Alipay(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = null;
            if (IsBase64String(encryptKey))
            {
                keyArray = Convert.FromBase64String(encryptKey);
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(encryptKey);
            }
            Byte[] toEncryptArray = Convert.FromBase64String(bizContent);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateDecryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.GetEncoding(charset).GetString(resultArray);
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesEncrypt(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = null;
            if (IsBase64String(encryptKey))
            {
                keyArray = Convert.FromBase64String(encryptKey);
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(encryptKey);
            }
            Byte[] toEncryptArray = Encoding.GetEncoding(charset).GetBytes(bizContent);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            //rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateEncryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesDencrypt(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = null;
            if (IsBase64String(encryptKey))
            {
                keyArray = Convert.FromBase64String(encryptKey);
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(encryptKey);
            }
            Byte[] toEncryptArray = Convert.FromBase64String(bizContent);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            //rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateDecryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.GetEncoding(charset).GetString(resultArray);
        }

        /// <summary>
        /// Qpay AES 加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesEncrypt_Qpay(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            Byte[] toEncryptArray = Encoding.GetEncoding(charset).GetBytes(bizContent);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            //rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateEncryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return ToHexString(resultArray);
        }

        /// <summary>
        /// Qpay AES 解密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesDencrypt_Qpay(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            Byte[] toEncryptArray = FromHexString(bizContent);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            //rDel.IV = AES_IV;
            ICryptoTransform cTransform = rDel.CreateDecryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.GetEncoding(charset).GetString(resultArray);
        }

        /// <summary>
        /// 腾讯手游APP AES 加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesEncrypt_APP(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            Byte[] toEncryptArray = Encoding.GetEncoding(charset).GetBytes(bizContent);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.IV = Encoding.UTF8.GetBytes("1234567890123456");
            ICryptoTransform cTransform = rDel.CreateEncryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return ToHexString(resultArray);
        }

        /// <summary>
        /// 腾讯手游APP AES 解密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="bizContent"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string AesDencrypt_APP(string encryptKey, string bizContent, string charset)
        {
            Byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            Byte[] toEncryptArray = FromHexString(bizContent);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.IV = Encoding.UTF8.GetBytes("1234567890123456");
            ICryptoTransform cTransform = rDel.CreateDecryptor(rDel.Key, rDel.IV);
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.GetEncoding(charset).GetString(resultArray);
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RsaSign(string data, string privateKey, string charset)
        {
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
            rsaCsp.PersistKeyInCsp = false;
            rsaCsp.FromXmlString(privateKey);

            byte[] dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="signContent"></param>
        /// <param name="sign"></param>
        /// <param name="publicKey"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static bool RsaCheck(string signContent, string sign, string publicKey, string charset)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;
            rsa.FromXmlString(publicKey);

            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
            return bVerifyResultOriginal;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publicKey"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RsaEncrypt(string content, string publicKey, string charset)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;
            rsa.FromXmlString(publicKey);
            byte[] data = Encoding.GetEncoding(charset).GetBytes(content);
            int maxBlockSize = rsa.KeySize / 8 - 11; //加密块最大长度限制
            if (data.Length <= maxBlockSize)
            {
                byte[] cipherbytes = rsa.Encrypt(data, false);
                return Convert.ToBase64String(cipherbytes);
            }
            MemoryStream plaiStream = new MemoryStream(data);
            MemoryStream crypStream = new MemoryStream();
            Byte[] buffer = new Byte[maxBlockSize];
            int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
            while (blockSize > 0)
            {
                Byte[] toEncrypt = new Byte[blockSize];
                Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                Byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                crypStream.Write(cryptograph, 0, cryptograph.Length);
                blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
            }
            return Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);

        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="privateKey"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RsaDecrypt(string content, string privateKey, string charset)
        {
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
            rsaCsp.FromXmlString(privateKey);
            byte[] data = Convert.FromBase64String(content);
            int maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
            if (data.Length <= maxBlockSize)
            {
                byte[] cipherbytes = rsaCsp.Decrypt(data, false);
                return Encoding.GetEncoding(charset).GetString(cipherbytes);
            }
            MemoryStream crypStream = new MemoryStream(data);
            MemoryStream plaiStream = new MemoryStream();
            Byte[] buffer = new Byte[maxBlockSize];
            int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
            while (blockSize > 0)
            {
                Byte[] toDecrypt = new Byte[blockSize];
                Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                Byte[] cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                plaiStream.Write(cryptograph, 0, cryptograph.Length);
                blockSize = crypStream.Read(buffer, 0, maxBlockSize);
            }
            return Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
        }

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="charset">编码（utf-8,gb2312)</param>
        /// <returns></returns>
        public static string Md5Encrypt(string content, string charset)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(content));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// BASE64编码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string content, string charset)
        {
            byte[] bytes = Encoding.GetEncoding(charset).GetBytes(content);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// BASE64解吗
        /// </summary>
        /// <param name="content"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string content, string charset)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.GetEncoding(charset).GetString(bytes);
        }

        /// <summary>
        /// Unicode 转中文
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ConvertChinese(string content)
        {
            StringBuilder sBuilder = new StringBuilder();
            MatchCollection mc = Regex.Matches(content, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            byte[] byt = new byte[2];
            foreach (Match m in mc)
            {
                byt[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                byt[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                sBuilder.Append(Encoding.Unicode.GetString(byt));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertDBC(string content)
        {
            char[] c = content.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        ///  IS base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            str = str.Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={1,3}$", RegexOptions.None);
        }

        /// <summary>
        /// TripleDES 加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">设置 TripleDES 算法的机密密钥（24位）</param>
        /// <param name="iv">设置对称算法的初始化向量 (IV)（8位）</param>
        /// <returns></returns>
        public static string TripleDesEncrypt(string content, string key, string iv)
        {
            SymmetricAlgorithm tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = Encoding.UTF8.GetBytes(key);
            tripleDES.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform cTransform = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, cTransform, CryptoStreamMode.Write);
            byte[] byt = Encoding.UTF8.GetBytes(content);
            cStream.Write(byt, 0, byt.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="input">获取或设置对称算法的初始化向量（8位）</param>
        /// <param name="key">获取或设置数据加密标准 (DES) 算法的机密密钥（8位）</param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesEncrypt(string content, string key, string iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(iv);

            ICryptoTransform cTransform = des.CreateEncryptor(des.Key, des.IV);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, cTransform, CryptoStreamMode.Write);
            byte[] byt = Encoding.UTF8.GetBytes(content);
            cStream.Write(byt, 0, byt.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// TripleDES 解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">设置 TripleDES 算法的机密密钥（24位）</param>
        /// <param name="iv">设置对称算法的初始化向量(IV)（8位）</param>
        /// <returns></returns>
        public static string TripleDesDecrypt(string content, string key, string iv)
        {
            SymmetricAlgorithm tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = Encoding.UTF8.GetBytes(key);
            tripleDES.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform cTransform = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, cTransform, CryptoStreamMode.Write);
            byte[] byt = Convert.FromBase64String(content);
            cStream.Write(byt, 0, byt.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="input">获取或设置对称算法的初始化向量（8位）</param>
        /// <param name="key">获取或设置数据加密标准 (DES) 算法的机密密钥（8位）</param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DesDecrypt(string content, string key, string iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(iv);

            ICryptoTransform cTransform = des.CreateDecryptor(des.Key, des.IV);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, cTransform, CryptoStreamMode.Write);
            byte[] byt = Convert.FromBase64String(content);
            cStream.Write(byt, 0, byt.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// 风火轮登录加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesEncryptFHL(string encryptKey, string content)
        {
            byte[] keyArray = Encoding.ASCII.GetBytes(encryptKey);
            Byte[] toEncryptArray = Encoding.Unicode.GetBytes(content);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// 卡门网登录加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AesEncryptKaMen(string encryptKey, string content)
        {
            byte[] keyArray = new byte[32];
            char[] cKey = encryptKey.ToCharArray();
            for (int i = 0; i < 32; i++)
            {
                keyArray[i] = (byte)cKey[i];
            }

            Byte[] toEncryptArray = Encoding.Unicode.GetBytes(content);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// DES解密 -云接口（CC）专用
        /// </summary>
        /// <param name="input">源</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string DesDecryptApi(string input, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key);
            des.IV = new byte[8];
            ICryptoTransform cTransform = des.CreateDecryptor(des.Key, des.IV);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, cTransform, CryptoStreamMode.Write);
            byte[] byt = Convert.FromBase64String(input);
            cStream.Write(byt, 0, byt.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// 京东图书features解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encryptKey"></param>
        /// <returns></returns>
        public static string AesDecryptJdBook(string content, string encryptKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(encryptKey);
            byte[] toEncryptArray = Convert.FromBase64String(content);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;        //必须设置为ECB  
            rDel.Padding = PaddingMode.PKCS7;  //必须设置为PKCS7  
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// String To Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        /// <summary>
        /// Hex To String
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes; // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

        public static string Encryptmd5(string encryptStr)
        {
            byte[] result = Encoding.Default.GetBytes(encryptStr.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        public static string Md5Hash(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encod">编码（utf-8,gb2312)</param>
        /// <returns></returns>
        public static string Md5Hash(string input, string encod)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.GetEncoding(encod).GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
