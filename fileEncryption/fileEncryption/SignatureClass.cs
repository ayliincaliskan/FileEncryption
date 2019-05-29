using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace fileEncryption
{
	class SignatureClass
	{
		#region İmzalama
		public string generatesignature(RSAParameters privatekey1)  
		{
			var sha = SHA256.Create();					//SHA256 ile daha sonra RSA ile şifreliyorum
			var hashedData = sha.ComputeHash(FileReadWrite.fileData);
			var rsa = RSA.Create();
			rsa.ImportParameters(privatekey1);
			var signer = new RSAPKCS1SignatureFormatter(rsa);
			signer.SetHashAlgorithm("SHA256");
			return Convert.ToBase64String(signer.CreateSignature(hashedData));
		}
		#endregion
		#region imza doğrulama
		public bool validate(string signature,RSAParameters publickey1)       
		{
			if (signature == null)
			{
				MessageBox.Show("Veri boş geldi");
				return false;
			}
			else
			{
				var sha = SHA256.Create();					//SHA256 ile şifreliyorum ve imza doğrulama yapıyorum
				var hasheddata = sha.ComputeHash(FileReadWrite.fileData);
				byte[] signaturebytes = Convert.FromBase64String(signature);
				var rsa = RSA.Create();
				rsa.ImportParameters(publickey1);
				var checker = new RSAPKCS1SignatureDeformatter(rsa);
				checker.SetHashAlgorithm("SHA256");
				return checker.VerifySignature(hasheddata, signaturebytes);			//aynıysa true farklıysa false döndürüyorum
			}
		}
		#endregion
	}
}
