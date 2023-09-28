//https://msdn.microsoft.com/zh-tw/library/system.security.cryptography.rsapkcs1keyexchangeformatter%28v=vs.110%29.aspx
//http://www.bouncycastle.org/csharp/
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace HPSVCS.sys
{
    public class CrytoHelper
    {
        public CrytoHelper()
        {
            TmpFile = System.Environment.CurrentDirectory.ToString() + "\\kcv.data";
            //TmpFile = @"c:\Temp\work\kcv.data";
        }

        public string TmpFile { get; set; }

        // Encrypt a file using a public key.
        public void EncryptFile(string inFile, string outFile, RSACryptoServiceProvider rsaPublicKey, int blockSizeBytes)
        {
            //密文資料檔案所使用的Session Key 值為:52CD045B7FFD4F1C -FD3419BC3D865175，KCV值為:E6792D
            //byte[] svcs_key = new byte[16] { 0x52, 0xCD, 0x04, 0x5B, 0x7F, 0xFD, 0x4F, 0x1C, 0xFD, 0x34, 0x19, 0xBC, 0x3D, 0x86, 0x51, 0x75 };

            using (TripleDES tdes = TripleDES.Create())
            {
                // Create instance of AesManaged for symetric encryption of the data.
                tdes.KeySize = 128;
                tdes.BlockSize = 64;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                //tdes.Key = svcs_key;

                using (ICryptoTransform transform = tdes.CreateEncryptor())
                {
                    RSAPKCS1KeyExchangeFormatter keyFormatter = new RSAPKCS1KeyExchangeFormatter(rsaPublicKey);
                    byte[] keyEncrypted = keyFormatter.CreateKeyExchange(tdes.Key, tdes.GetType());

                    
                    //byte[] keyEncrypted = rsaPublicKey.Encrypt(tdes.Key, true);

                    byte[] kcv_base = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    //byte[] kcv_key = new byte[16] { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f };
                    byte[] kcv_key = tdes.Key;
                    byte[] keyCheckValue = Encrypt(kcv_base, kcv_key, TmpFile);
                    //byte[] keyCheckValue = new byte[3] { 0, 0, 0 };

                    using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                    {
                        outFs.WriteByte(0x00);
                        outFs.Write(keyEncrypted, 0, keyEncrypted.Length);
                        for (int i = keyEncrypted.Length; i < 256; i++)
                            outFs.WriteByte(0x00);
                        outFs.Write(keyCheckValue, 0, 3);

                        // Now write the cipher text using
                        // a CryptoStream for encrypting.
                        using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {

                            // By encrypting a chunk at
                            // a time, you can save memory
                            // and accommodate large files.
                            int count = 0;
                            int offset = 0;

                            // blockSizeBytes can be any arbitrary size.
                            //int blockSizeBytes = tdes.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];
                            int bytesRead = 0;

                            using (FileStream inFs = new FileStream(inFile, FileMode.Open))
                            {
                                do
                                {
                                    count = inFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamEncrypted.Write(data, 0, count);
                                    bytesRead += blockSizeBytes;
                                }
                                while (count > 0);
                                inFs.Close();
                            }

                            outStreamEncrypted.FlushFinalBlock();
                            outStreamEncrypted.Close();
                        }

                        outFs.Close();
                    }
                }
            }
        }

        // Encrypt a file using a public key.
        public void EncryptFileByAesSessionKey(string sP_PublicKeyFileName, string sP_InFileName, string sP_OutFileName, int nP_BlockSizeBytes, out byte[] P_EncryptedSessionKey)
        {
            //密文資料檔案所使用的Session Key 值為:52CD045B7FFD4F1C -FD3419BC3D865175，KCV值為:E6792D
            //byte[] svcs_key = new byte[16] { 0x52, 0xCD, 0x04, 0x5B, 0x7F, 0xFD, 0x4F, 0x1C, 0xFD, 0x34, 0x19, 0xBC, 0x3D, 0x86, 0x51, 0x75 };

            //sample ... http://codereview.stackexchange.com/questions/36453/aes-encryption-c-net
                
            //using (Aes L_Aes = Aes.Create())
            Aes L_Aes = new AesCryptoServiceProvider();
            
            //down, 建立session key(L_Encryptor.Key就是session key)
            L_Aes.KeySize = 128;
            L_Aes.BlockSize = 128;
            L_Aes.IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            L_Aes.Mode = CipherMode.CBC;
            L_Aes.Padding = PaddingMode.PKCS7;

                
	 
            //L_Encryptor.Key = svcs_key;
            //up, 建立session key(L_Aes.Key就是session key)
                

            //down, 讀取public key Parameter
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter L_PublicKeyParameter=  ReadAsymmetricKeyParameter(sP_PublicKeyFileName);
            //up, 讀取public key Parameter

            //down, 用 public key 加密session key

                
            byte[] L_KeyEncrypted = RsaEncryptWithPublic(L_Aes.Key, L_PublicKeyParameter);

            P_EncryptedSessionKey = new byte[L_KeyEncrypted.Length];
            Array.Copy(L_KeyEncrypted, P_EncryptedSessionKey, L_KeyEncrypted.Length);    
                
            //up, 用 public key 加密session key

            //down, 加密檔案..
            //EncryptFile(sP_InFileName, @"c:\temp\enc.txt", L_Aes.Key); for test..
            EncryptFile(sP_InFileName, sP_OutFileName, L_Aes.Key);
                
            //DecryptFile(@"c:\temp\enc.txt", @"c:\temp\dec.txt", L_Aes.Key); for test
            //up, 加密檔案..
        }


        
        //public string RsaEncryptWithPublic(string clearText, string publicKey)
        public byte[] RsaEncryptWithPublic(byte[] P_ClearByteAry, Org.BouncyCastle.Crypto.AsymmetricKeyParameter P_PublicKeyParameter)
        {
        

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            

                

            encryptEngine.Init(true, P_PublicKeyParameter);


            return encryptEngine.ProcessBlock(P_ClearByteAry, 0, P_ClearByteAry.Length);
            

        }
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter ReadAsymmetricKeyParameter(string P_PublicKeyFileName)
        {
            var L_FileStream = System.IO.File.OpenText(P_PublicKeyFileName);
            var L_PemReader = new Org.BouncyCastle.OpenSsl.PemReader(L_FileStream);
            var L_KeyParameter = (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)L_PemReader.ReadObject();
            return L_KeyParameter;
        }

        public string RsaDecrypt(string base64Input, string privateKey)
        {
            var bytesToDecrypt = Convert.FromBase64String(base64Input);

            //get a stream from the string
            AsymmetricCipherKeyPair keyPair;
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, keyPair.Private);
            }

            var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));
            return decrypted;
        }
        // Decrypt a file using a private key.
        public void DecryptFile(string inFile, string outFile, RSACryptoServiceProvider rsaPrivateKey, int blockSizeBytes)
        {
            // Create instance of AesManaged for symetric decryption of the data.
            using (TripleDES tdes = TripleDES.Create())
            {
                tdes.KeySize = 128;
                tdes.BlockSize = 64;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (FileStream inFs = new FileStream(inFile, FileMode.Open))
                {
                    //inFs.Seek(0, SeekOrigin.Begin);
                    inFs.ReadByte();

                    int keyLength = rsaPrivateKey.KeySize / 8;
                    byte[] KeyEncrypted = new byte[keyLength];
                    inFs.Read(KeyEncrypted, 0, KeyEncrypted.Length);
                    for (int i = KeyEncrypted.Length; i < 256; i++)
                        inFs.ReadByte();

                    byte[] keyCheckValue = new byte[3];
                    inFs.Read(keyCheckValue, 0, 3);

                    byte[] KeyDecrypted = rsaPrivateKey.Decrypt(KeyEncrypted, false);

                    tdes.Key = KeyDecrypted;
                    //tdes.IV = KeyDecrypted;

                    // Decrypt the key.
                    using (ICryptoTransform transform = tdes.CreateDecryptor())
                    {
                        using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                        {
                            int count = 0;
                            int offset = 0;

                            //int blockSizeBytes = tdes.BlockSize / 8;
                            byte[] data = new byte[blockSizeBytes];

                            using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                            {
                                do
                                {
                                    count = inFs.Read(data, 0, blockSizeBytes);
                                    offset += count;
                                    outStreamDecrypted.Write(data, 0, count);

                                }
                                while (count > 0);

                                outStreamDecrypted.FlushFinalBlock();
                                outStreamDecrypted.Close();
                            }
                            outFs.Close();
                        }
                        inFs.Close();
                    }
                }
            }
        }

        public void EncryptFile(string sP_InputFileName, string sP_OutputFileName, byte[] P_KeyAry)
        {
            try
            {
                using (RijndaelManaged L_RManager = new RijndaelManaged())
                {
                    byte[] key = P_KeyAry;

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    L_RManager.Key = P_KeyAry;
                    L_RManager.Mode = System.Security.Cryptography.CipherMode.CBC;
                    L_RManager.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    L_RManager.IV = IV;
                    using (FileStream fsCrypt = new FileStream(sP_OutputFileName, FileMode.Create))
                    {
                        using (ICryptoTransform encryptor = L_RManager.CreateEncryptor(key, IV))
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                            {
                                using (FileStream fsIn = new FileStream(sP_InputFileName, FileMode.Open))
                                {
                                    int data;
                                    while ((data = fsIn.ReadByte()) != -1)
                                    {
                                        cs.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to encrypt file
            }
        }

        public void DecryptFile(string sP_InputFileName, string sP_OutputFileName, byte[] P_KeyAry)
        {
            try
            {
                using (RijndaelManaged L_RManager = new RijndaelManaged())
                {
                    byte[] key = P_KeyAry;

                    /* This is for demostrating purposes only. 
                     * Ideally you will want the IV key to be different from your key and you should always generate a new one for each encryption in other to achieve maximum security*/
                    byte[] IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    L_RManager.Key = P_KeyAry;
                    L_RManager.Mode = System.Security.Cryptography.CipherMode.CBC;
                    L_RManager.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    L_RManager.IV = IV;

                    using (FileStream fsCrypt = new FileStream(sP_InputFileName, FileMode.Open))
                    {
                        using (FileStream fsOut = new FileStream(sP_OutputFileName, FileMode.Create))
                        {
                            using (ICryptoTransform decryptor = L_RManager.CreateDecryptor())
                            {
                                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        fsOut.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // failed to decrypt file
            }
        }
        public static byte[] Encrypt(byte[] data, byte[] key, string outFile)
        {
            
            using (TripleDES tdes = TripleDES.Create())
            {
                // Create instance of AesManaged for symetric encryption of the data.
                tdes.KeySize = 128;
                tdes.BlockSize = 64;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                tdes.Key = key;

                using (ICryptoTransform transform = tdes.CreateEncryptor())
                {
                    using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                    {
                        // Now write the cipher text using
                        // a CryptoStream for encrypting.
                        using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                        {

                            outStreamEncrypted.Write(data, 0, data.Length);
                            outStreamEncrypted.FlushFinalBlock();
                            outStreamEncrypted.Close();
                        }

                        outFs.Close();
                    }
                }
            }

            byte[] result = new byte[8];

            using (FileStream inFs = new FileStream(outFile, FileMode.Open))
            {
                inFs.Read(result, 0, 8);
                inFs.Close();
            }

            File.Delete(outFile);
            return result;
        }

    }
}
