using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace fileEncryption
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		//A kullanıcısı
		private static RSAParameters privatekey1;
		private static RSAParameters publickey1;
		//B kullanıcısı
		private static RSAParameters privatekey2;
		private static RSAParameters publickey2;

		private static string privatekeyString;
		private static string publickeyString;

		private byte[] aeskey;
		private byte[] aesıv;
		private byte[] encrypt;

		private static string DosyaYolu;
		private static string DosyaAdi;
		private static string dosyaadi;


		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				GenKey_SaveInContainer();       //public ve private keylerimi iki kullanıcı içinde yarattım
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.Message);
			}
			button2.Visible = false;
			comboBox2.Visible = false;
			label2.Visible = false;
		}
		public void GenKey_SaveInContainer()
		{
			try
			{
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);    //1.kullanıcı
				privatekey1 = rsa.ExportParameters(true);
				//privatekeyString = rsa.ToXmlString(true);
				publickey1 = rsa.ExportParameters(false);
				//publickeyString = rsa.ToXmlString(false);

				RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider(1024);   //2.kullanıcı
				privatekey2 = rsa2.ExportParameters(true);
				publickey2 = rsa2.ExportParameters(false);
				privatekeyString = rsa2.ToXmlString(true);
				publickeyString = rsa2.ToXmlString(false);


			}
			catch (Exception ex) { MessageBox.Show(ex.Message); }
		
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog file = new OpenFileDialog();			//işlem görecek dosya seçilir
				file.ShowDialog();
				DosyaYolu = file.FileName;
				DosyaAdi = file.SafeFileName;
				FileReadWrite filerw = new FileReadWrite();
				filerw.ByteOku(DosyaYolu);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			comboBox2.Items.Clear();
			try
			{
				//imzalama
				if (comboBox1.SelectedIndex == 0)       
				{
					SignatureClass signatur = new SignatureClass();
					signatur.generatesignature(privatekey1);
					MessageBox.Show("Dosya imzalandı");
					comboBox2.Items.Add("İmzayı Doğrula");
				}

				//şifreleme
				else if (comboBox1.SelectedIndex == 1)
				{
					AESClass aesclass = new AESClass();
					Tuple<byte[], byte[], byte[]> tuple = aesclass.EncryptAesManaged(FileReadWrite.fileData);
					encrypt = tuple.Item1;		//şifrelenen dosyam
					aeskey = tuple.Item2;
					aesıv = tuple.Item3;
					RSAClass rsa = new RSAClass();
					aeskey = rsa.Encryption(aeskey,publickeyString);		//keyimi karşı tarafa RSA ile şifreleyerek gönderiyorum
					FileReadWrite frw = new FileReadWrite();
					frw.ByteYaz(tuple.Item1);
					MessageBox.Show("Dosya şifrelendi");
					comboBox2.Items.Add("Şifrelenen Dosyayı Çöz");
				}

				//imzalama ve şifreleme
				else if (comboBox1.SelectedIndex == 2)
				{
					SignatureClass signatur = new SignatureClass();		//imzalıyorum
					signatur.generatesignature(privatekey1);
					AESClass aesclass = new AESClass();
					Tuple<byte[], byte[], byte[]> tuple = aesclass.EncryptAesManaged(FileReadWrite.fileData);
					encrypt = tuple.Item1;
					aeskey = tuple.Item2;
					aesıv = tuple.Item3;
					RSAClass rsa = new RSAClass();			// aes ile şifreliyorum
					aeskey = rsa.Encryption(aeskey,publickeyString);				//keyimi RSA ile şifreleyerek gönderiyorum
					FileReadWrite frw = new FileReadWrite();
					frw.ByteYaz(tuple.Item1);
					MessageBox.Show("Dosya imzalandı ve şifrelendi");
					comboBox2.Items.Add("İmzayı Doğrula ve Şifreyi Çöz");

				}

				button2.Visible = true;
				comboBox2.Visible = true;
				label2.Visible = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}


		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog();			//işlem görmüş dosyamı seçiyorum
				ofd.ShowDialog();
				dosyaadi = ofd.SafeFileName;
				string dosyayolu = ofd.FileName;
				FileReadWrite frw = new FileReadWrite();
				encrypt=frw.ByteOku(dosyayolu);
				if (dosyaadi == DosyaAdi)
				{
					comboBox2.Enabled = true;
				}
				else if (dosyaadi != DosyaAdi) {
					MessageBox.Show("Dosya ismini değiştirdiniz yada farklı bir dosya seçtiniz. İşlem yapamazsınız!");
					comboBox2.Enabled = false;
				}
				else if (dosyaadi == null) {
					MessageBox.Show("Dosya seçmediniz.Lütfen işlem yapacağınız dosyayı seçiniz.");
					comboBox2.Enabled = false;
				}
				else if (DosyaYolu != dosyayolu) {
					MessageBox.Show("Dosyayı şifrelendiği dizine geri getiriniz!");
					comboBox2.Enabled = false;
				}
				else if (DosyaAdi == null) {
					MessageBox.Show("İşlem gerçekleşen dosya bulunamadı.");
					comboBox2.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (dosyaadi == null)
			{
				MessageBox.Show("Dosya seçiniz");
			}
			else
			{
				try
				{
					comboBox2.Enabled = true;
					//imzamı doğruluyorum
					if (comboBox1.SelectedIndex==0)
					{
						SignatureClass sigval = new SignatureClass();
						string signature = sigval.generatesignature(privatekey1);
						if (sigval.validate(signature,publickey1))			//eğer true dönerse yani imzalar eşit ise doğrulama mesajı gönderiyorum
						{
							MessageBox.Show("imza doğrulandı");
						}
						else				//eğer imzalar eşit değilse(false döndüğünde)
						{
							MessageBox.Show("imzalar eşleşmedi");
						}

					}
					
					//dosyamın şifresini çözüyorum
					else if (comboBox1.SelectedIndex == 1)
					{
						RSAClass rsa = new RSAClass();			//RSA ile şifrelenen keyimi asimetrik şifreleme yöntemi ile deşifreliyorum
						aeskey = rsa.Decryption(aeskey,privatekeyString);
						AESClass aesclass = new AESClass();         //aes ile dosyamın şifresini çözüyorum
						aesclass.EncryptAesManagedDecrypt(encrypt, aeskey, aesıv);

					}
					
					//imzamı doğruluyorum ve dosyamın şifresini çözüyorum
					else if (comboBox1.SelectedIndex == 2)
					{
						RSAClass rsa = new RSAClass();				//RSA ile şifrelenen aes.key'imin şifresini çözüyorum
						aeskey = rsa.Decryption(aeskey,privatekeyString);
						AESClass aesclass = new AESClass();				//Aes ile şifrelenen dosyamı deşifreliyorum
						aesclass.EncryptAesManagedDecrypt(encrypt, aeskey, aesıv);
						SignatureClass sigval = new SignatureClass();			//imzamı doğruluyorum
						string signature = sigval.generatesignature(privatekey1);
						if (sigval.validate(signature,publickey1))					//doğrulama true döndüğünde girdiği işlem
						{
							MessageBox.Show("imza doğrulandı");
						}
						else							//doğrulama false döndüğünde girdiği işlem
						{
							MessageBox.Show("imzalar eşleşmedi");
						}

					}
					dosyaadi = null;
					FileReadWrite.fileData = null;
					FileReadWrite.fileName = null;
					FileReadWrite.filePath = null;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}


		
	}
}
