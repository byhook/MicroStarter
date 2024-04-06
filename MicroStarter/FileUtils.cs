using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MicroStarter;

public class FileUtils
{
    public static string GetMd5Hash(string filePath)
    {
        // 创建一个MD5CryptoServiceProvider对象
        using (MD5 md5 = MD5.Create())
        {
            // 读取文件内容
            using (FileStream stream = File.OpenRead(filePath))
            {
                // 计算MD5哈希值
                byte[] hash = md5.ComputeHash(stream);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:x2}", b);
                }

                return sb.ToString();
            }
        }
    }
}