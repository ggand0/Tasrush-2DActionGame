// pentium : http://nonounit.web.fc2.com/　様よりいただきました

using System.IO;
using System.Security.Cryptography;

//書き込み例
//string str = "変換するよ。";
//MemoryStream ms = new MemoryStream( Encoding.UTF8.GetBytes( str ) );
//Encryption.EncryptionData( ms , "test.txt" );

//読み込み例
//MemoryStream ms = new MemoryStream();
//Encryption.DecryptionFile( "test.txt" , ms );

namespace _2DActionGame
{
	public class Encryption
	{
		const string PASSWORD = "Password";

		/// <summary>
		/// データを暗号化し、ファイルに出力します。
		/// </summary>
		/// <param name="filepath">暗号化したいファイルの名前</param>
		/// <param name="outfilepath">出力ファイルの名前</param>
		public static void EncryptionData(string filepath, string outfilepath)
		{
			//AesCryptoServiceProviderオブジェクトの作成
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider()) {
				aes.BlockSize = 128;											//ブロックサイズ(128bit)
				aes.KeySize = 128;												//キー最大長(128bit)
				aes.Mode = CipherMode.CBC;			//CBCモード
				aes.Padding = PaddingMode.PKCS7;	//パディングモード
				aes.GenerateIV();
				byte[] bytesIV = aes.IV;										//初期化ベクトルの設定と取得（ブロックサイズと同サイズ=128bit）
				aes.Key = AESTransformBytes(PASSWORD);						//キーの設定
				ICryptoTransform encrypt = aes.CreateEncryptor();	//AES暗号化オブジェクトの作成

				//FileStreamの生成
				using (System.IO.FileStream outfs = new System.IO.FileStream(outfilepath, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
					outfs.Write(bytesIV, 0, 16);	//IVを先頭に書き込む(128bit=16bytes)

					//CryptoStreamの作成
					using (CryptoStream cs = new CryptoStream(
						outfs, encrypt, CryptoStreamMode.Write)) {
						//暗号化データを書き出していく
						using (System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
							byte[] buffer = new byte[1024];
							int len;

							while ((len = fs.Read(buffer, 0, buffer.Length)) > 0) {
								cs.Write(buffer, 0, len);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// データを暗号化し、ファイルに出力します。
		/// </summary>
		/// <param name="ms">暗号化したいメモリストリーム</param>
		/// <param name="outfilepath">出力ファイルの名前</param>
		public static void EncryptionData(MemoryStream ms, string outfilepath)
		{
			//AesCryptoServiceProviderオブジェクトの作成
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider()) {
				aes.BlockSize = 128;						//ブロックサイズ(128bit)
				aes.KeySize = 128;							//キー最大長(128bit)
				aes.Mode = CipherMode.CBC;					//CBCモード
				aes.Padding = PaddingMode.PKCS7;			//パディングモード
				aes.GenerateIV();
				byte[] bytesIV = aes.IV;					//初期化ベクトルの設定と取得（ブロックサイズと同サイズ=128bit）
				aes.Key = AESTransformBytes(PASSWORD);	//キーの設定
				ICryptoTransform encrypt = aes.CreateEncryptor();	//AES暗号化オブジェクトの作成

				//FileStreamの生成
				using (System.IO.FileStream outfs = new System.IO.FileStream(outfilepath, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
					outfs.Write(bytesIV, 0, 16);	//IVを先頭に書き込む(128bit=16bytes)

					//CryptoStreamの作成
					using (CryptoStream cs = new CryptoStream(
						outfs, encrypt, CryptoStreamMode.Write)) {
						//暗号化データを書き出していく
						byte[] buffer = new byte[4096];//[2048];//byte[1024]
						int length;

						while ((length = ms.Read(buffer, 0, buffer.Length)) > 0) {
							cs.Write(buffer, 0, length);// bufferは87くらいまでしか使われてない
						}
					}
				}
			}
		}

		/// <summary>
		/// 指定したファイルを復号化し、データに出力します。
		/// </summary>
		/// <param name="filepath">復号化したいファイルの名前</param>
		/// <param name="ms">データを格納するためのメモリストリーム</param>
		/// <returns></returns>
		public static void DecryptionFile(string filepath, MemoryStream ms)
		{
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider()) {
				aes.BlockSize = 128;											//ブロックサイズ(128bit)
				aes.KeySize = 128;												//キー最大長(128bit)
				aes.Mode = CipherMode.CBC;			//CBCモード
				aes.Padding = PaddingMode.PKCS7;	//パディングモード
				byte[] bytesIV = new byte[16];
				aes.Key = AESTransformBytes(PASSWORD);						//キーの設定

				//暗号化データを読み込んでいく
				using (System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
					//IVを先頭から取り出してAesCryptoServiceProviderオブジェクトにセット
					fs.Read(bytesIV, 0, 16);
					aes.IV = bytesIV;

					//AES 復号オブジェクトの作成
					ICryptoTransform encrypt = aes.CreateDecryptor();

					//仮MemoryStreamの生成
					using (System.IO.MemoryStream ms_tmp = new System.IO.MemoryStream()) {
						//CryptoStreamの作成
						CryptoStream cs = new CryptoStream(
							ms_tmp, encrypt, CryptoStreamMode.Write);

						//データの書き込み
						byte[] buffer = new byte[1024];

						int len;

						while ((len = fs.Read(buffer, 0, buffer.Length)) > 0) {
							cs.Write(buffer, 0, len);
						}

						ms_tmp.Seek(0, SeekOrigin.Begin);	//位置を初期化
						ms_tmp.CopyTo(ms);	//コピー
						ms.Seek(0, SeekOrigin.Begin);	//位置を初期化
					}
				}
			}
		}

		/// <summary>
		/// 指定したファイルを復号化し、データに出力します。
		/// </summary>
		/// <param name="filepath"></param>
		/// <param name="outfilepath"></param>
		/// <returns></returns>
		public static void DecryptionFile(string filepath, string outfilepath)
		{
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider()) {
				aes.BlockSize = 128;											//ブロックサイズ(128bit)
				aes.KeySize = 128;												//キー最大長(128bit)
				aes.Mode = CipherMode.CBC;			//CBCモード
				aes.Padding = PaddingMode.PKCS7;	//パディングモード
				byte[] bytesIV = new byte[16];
				aes.Key = AESTransformBytes(PASSWORD);						//キーの設定

				//暗号化データを読み込んでいく
				using (System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
					//IVを先頭から取り出してAesCryptoServiceProviderオブジェクトにセット
					fs.Read(bytesIV, 0, 16);
					aes.IV = bytesIV;

					//AES 復号オブジェクトの作成
					ICryptoTransform encrypt = aes.CreateDecryptor();

					//MemoryStreamの生成
					using (System.IO.FileStream outfs = new System.IO.FileStream(outfilepath, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
						//CryptoStreamの作成
						using (CryptoStream cs = new CryptoStream(
							outfs, encrypt, CryptoStreamMode.Write)) {
							//データの書き込み
							byte[] buffer = new byte[1024];

							int len;

							while ((len = fs.Read(buffer, 0, buffer.Length)) > 0) {
								cs.Write(buffer, 0, len);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// AES用のメソッドです。string型のパスワードをbyte[]型に変換し、また長さの調整を行います。
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] AESTransformBytes(string password)
		{
			byte[] bytesPassword = System.Text.Encoding.UTF8.GetBytes(password);	//UTF8でエンコード
			byte[] bytesKey = new byte[16];    //キー長128bit

			//有効なキーサイズになっていない場合は調整する
			for (int i = 0; i < 16; i++) {
				if (i < bytesPassword.Length) {
					bytesKey[i] = bytesPassword[i];	//値コピー
				} else {
					bytesKey[i] = 0;  //余白はゼロで埋める
				}
			}

			return bytesKey;
		}
	}
}
