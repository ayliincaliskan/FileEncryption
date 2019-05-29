using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace fileEncryption
{
	class RSAClass
	{
		#region RSA şifreleme
		public byte[] Encryption(byte[] aeskey,string publickeyString)
		{
			using (var rsa = new RSACryptoServiceProvider(1024))
			{
				try
				{
					rsa.FromXmlString(publickeyString);			//karşı kullanıcının public keyi ile aeskey'ini şifreliyorum

					var encryptedData = rsa.Encrypt(aeskey, true);
					return encryptedData;
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}
		#endregion
		#region RSA deşifreleme
		public byte[] Decryption(byte[] sifreliaeskey,string privatekeyString)
		{
			using (var rsa = new RSACryptoServiceProvider(1024))
			{
				try
				{            
					rsa.FromXmlString(privatekeyString);			//aes keyini private key'im ile deşifreliyorum

					var decryptedBytes = rsa.Decrypt(sifreliaeskey, true);
					return decryptedBytes;
				}
				finally
				{
					rsa.PersistKeyInCsp = false;
				}
			}
		}
		#endregion
	}
}
