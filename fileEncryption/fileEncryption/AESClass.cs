using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace fileEncryption
{
	class AESClass
	{
		#region AES Şifreleme
		public Tuple<byte[],byte[],byte[]> EncryptAesManaged(byte[] raw)
		{
			byte[] encrypte;
			byte[] aeskey;
			byte[] aesiv;
			try
			{
				using (RijndaelManaged aesAlg = new RijndaelManaged())
				{
					aesAlg.GenerateKey();
					aesAlg.GenerateIV();
					aeskey = aesAlg.Key;
					aesiv = aesAlg.IV;
				}
				encrypte = Encrypt(raw, aeskey, aesiv);

				return Tuple.Create(encrypte, aeskey, aesiv);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
				return null;
			}

		}
		public byte[] Encrypt(byte[] data, byte[] Key, byte[] IV)
		{
			byte[] encryptedData = null;

			if (data == null)
				throw new ArgumentNullException("data");

			if (data == Key)
				throw new ArgumentNullException("key");

			if (data == IV)
				throw new ArgumentNullException("iv");

			using (RijndaelManaged aesAlg = new RijndaelManaged())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
				encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);
			}

			return encryptedData;
		}
		#endregion
		#region AES Deşifreleme
		public void EncryptAesManagedDecrypt(byte[] encrypted,byte[] aeskey,byte[]aesıv)
		{
			FileReadWrite frw = new FileReadWrite();
			try
			{
				// Create Aes that generates a new key and initialization vector (IV).    
				// Same key must be used in encryption and decryption    
	
					byte[] decrypted = Decrypt(encrypted, aeskey, aesıv);
				//	byte[] bitti = Encoding.ASCII.GetBytes(decrypted);
					// Print decrypted string. It should be same as raw data    
					frw.ByteYaz(decrypted);
					MessageBox.Show("Şifre çözüldü");

				
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}

		}
		public byte[] Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
		{
			byte[] decryptedData = null;

			if (cipherText == null)
				throw new ArgumentNullException("data");

			if (cipherText == Key)
				throw new ArgumentNullException("key");

			if (cipherText == IV)
				throw new ArgumentNullException("iv");

			using (RijndaelManaged aesAlg = new RijndaelManaged())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				decryptedData = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
			}

			return decryptedData;
		}
		#endregion
	}
}
