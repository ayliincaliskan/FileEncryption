using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace fileEncryption
{
	class FileReadWrite
	{
		public static byte[] fileData;
		public static string plaintext;
		public static string fileName;
		public static string filePath;

		public byte[] ByteOku(string pathFile)			//seçilen dosyamı byte olarak okuyorum
		{
			try
			{
				fileData = File.ReadAllBytes(pathFile);
				plaintext = File.ReadAllText(pathFile);
				fileName = Path.GetFileName(pathFile);
				filePath = pathFile;
				return fileData;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

		}
		public bool ByteYaz(byte[] data)			//işleme aldığım dosyanın yeni halini yazdırıyorum
		{
			try
			{
				File.WriteAllBytes(filePath, data);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
