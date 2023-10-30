using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class AESEncrypt
{
    // SHA256Managed 는 salt 값을 사용하기 위해 해쉬를 뽑아내는 용도로만 사용하고 있음
    protected SHA256Managed sha256Managed = new SHA256Managed();

    protected RijndaelManaged aes = new RijndaelManaged();
    protected static readonly string _privateKey = "R[=n#m!*h@~";

    public AESEncrypt()
    {
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
    }

    /// <summary>
    /// 암호화
    /// </summary>
    /// <param name="strEncryptData">암호화 할 데이터</param>
    /// <param name="key">키</param>
    /// <returns>암호화된 데이터</returns>
    public byte[] AESEncrypt256(string strEncryptData, string key = "")
    {
        string privateKey = _privateKey;
        if (string.IsNullOrWhiteSpace(key) == false)
            privateKey = key;

        byte[] encryptData = Encoding.UTF8.GetBytes(strEncryptData);

        // SHA256Managed 는 salt 값을 사용하기 위해 해쉬를 뽑아내는 용도로만 사용하고 있음
        var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(privateKey.Length.ToString()));

        var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 5);    //반복 5
        var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
        var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(secretKey, iv), CryptoStreamMode.Write))
            {
                cs.Write(encryptData, 0, encryptData.Length);
            }
            xBuff = ms.ToArray();
        }

        short len = Convert.ToInt16(xBuff.Length);
        byte[] byteLengthArray = BitConverter.GetBytes(len);
        byte[] byteDataArray = new byte[xBuff.Length + 2];
        Buffer.BlockCopy(byteLengthArray, 0, byteDataArray, 0, 2);
        Buffer.BlockCopy(xBuff, 0, byteDataArray, 2, xBuff.Length);

        return byteDataArray;
    }


    /// <summary>
    /// 복호화
    /// </summary>
    /// <param name="decryptData">복호화 할 데이터</param>
    /// <param name="key">키</param>
    /// <returns>복호화된 데이터</returns>
    public string AESDecrypt256(byte[] decryptData, string key = "")
    {
        string privateKey = _privateKey;
        if (string.IsNullOrWhiteSpace(key) == false)
            privateKey = key;

        // SHA256Managed 는 salt 값을 사용하기 위해 해쉬를 뽑아내는 용도로만 사용하고 있음
        var salt = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(privateKey.Length.ToString()));

        var PBKDF2Key = new Rfc2898DeriveBytes(privateKey, salt, 5);    //반복 5
        var secretKey = PBKDF2Key.GetBytes(aes.KeySize / 8);
        var iv = PBKDF2Key.GetBytes(aes.BlockSize / 8);
        byte[] xBuff = null;

        byte[] byteLengthArray = new byte[2];
        Buffer.BlockCopy(decryptData, 0, byteLengthArray, 0, 2);
        short len = (short)(byteLengthArray[0] | (byteLengthArray[1] << 8));
        int iLen = Convert.ToInt32(len);
        byte[] byteDataArray = new byte[iLen];
        Buffer.BlockCopy(decryptData, 2, byteDataArray, 0, iLen);

        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(secretKey, iv), CryptoStreamMode.Write))
            {
                cs.Write(byteDataArray, 0, byteDataArray.Length);
            }
            xBuff = ms.ToArray();
        }
        return Encoding.UTF8.GetString(xBuff);
    }
}
