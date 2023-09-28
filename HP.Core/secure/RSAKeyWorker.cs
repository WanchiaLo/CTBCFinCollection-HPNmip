using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace hp.secure
{
    public class RSAKeyWorker
    {
        public RSACryptoServiceProvider KeyPublicPfx { get; set; }
        public RSACryptoServiceProvider KeyPrivatePfx { get; set; }
        public RSACryptoServiceProvider KeyPublicSvcs { get; set; }

        public bool KeyPrepare(string pfxFile, string pfxPswd)
        {
            // Get the certifcate to use to encrypt the key.
            X509Certificate2 pfx = new X509Certificate2(pfxFile, pfxPswd);
            if (pfx == null)
            {
                Console.WriteLine("Certificatge " + pfxFile + " not found.");
                return false;
            }

            KeyPublicPfx = (RSACryptoServiceProvider)pfx.PublicKey.Key;
            KeyPrivatePfx = (RSACryptoServiceProvider)pfx.PrivateKey;

            return true;
        }

        public bool KeyGenerate()
        {
            RSACryptoServiceProvider rsacsp = new RSACryptoServiceProvider(1024);
            RSAParameters rsapPrivate = rsacsp.ExportParameters(true);
            RSAParameters rsapPublic = rsacsp.ExportParameters(false);

            KeyPublicPfx = new RSACryptoServiceProvider();
            KeyPublicPfx.ImportParameters(rsapPublic);

            KeyPrivatePfx = new RSACryptoServiceProvider();
            KeyPrivatePfx.ImportParameters(rsapPrivate);

            return true;
        }

        public void Key2SvcsFile(RSACryptoServiceProvider rsacsp, int keyLength, string cvcsFile)
        {
            RSAParameters rsap = rsacsp.ExportParameters(false);
            byte[] modules = new byte[keyLength];
            byte[] exponent = new byte[3];
            byte[] checkcode = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            modules = rsap.Modulus;
            exponent = rsap.Exponent;

            using (FileStream fso = new FileStream(cvcsFile, FileMode.CreateNew, FileAccess.Write))
            {
                fso.WriteByte(0x00);
                //fso.Write(modules, 0, 256);
                fso.Write(modules, 0, modules.Length);
                for (int i = modules.Length; i < 256; i++)
                    fso.WriteByte(0x00);
                fso.Write(exponent, 0, 3);
                fso.WriteByte(0x00);
                fso.Write(checkcode, 0, 8);
                fso.Close();
            }
        }

        public RSACryptoServiceProvider KeyLoadFromSvcsFile(int keyLength, string cvcsFile)
        {
            byte[] modules = new byte[keyLength];
            byte[] exponent = new byte[3];
            byte[] checkcode = new byte[8];

            using (FileStream fsi = new FileStream(cvcsFile, FileMode.Open))
            {
                fsi.ReadByte();
                fsi.Read(modules, 0, modules.Length);
                for (int i = modules.Length; i < 256; i++)
                    fsi.ReadByte();
                fsi.Read(exponent, 0, 3);
                fsi.ReadByte();
                fsi.Read(checkcode, 0, 8);
                fsi.Close();
            }

            RSACryptoServiceProvider rsacsp = new RSACryptoServiceProvider(1024);
            RSAParameters rsap = new RSAParameters();

            rsap.Modulus = modules;
            rsap.Exponent = exponent;
            rsacsp.ImportParameters(rsap);

            //Console.WriteLine("RSA public key imported");
            return rsacsp;
        }

        public void Key2XmlFile(RSACryptoServiceProvider rsacsp, string xmlFile, bool includingPrivateParameters)
        {
            string xml = rsacsp.ToXmlString(includingPrivateParameters);

            StreamWriter sw = new StreamWriter(xmlFile, false);
            sw.Write(xml);
            sw.Close();
        }

        public RSACryptoServiceProvider KeyLoadFromXmlFile(string xmlFile)
        {
            RSACryptoServiceProvider rsacsp = new RSACryptoServiceProvider();
            string xml = File.ReadAllText(xmlFile);
            rsacsp.FromXmlString(xml);

            return rsacsp;
        }
    }
}
