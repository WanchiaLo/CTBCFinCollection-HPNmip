using System;
using System.IO;
using System.Text;
using hp.secure;
using hp.helper;
using HPSVCS.sys;
using hp.log;

namespace HPSVCS
{
    class SvcsTester
    {
        public static void EncryptFile(ILogger logger, string keyFile, string srcFile)
        {
            RSAKeyWorker keyWorker = new RSAKeyWorker();
            keyWorker.KeyPublicPfx = keyWorker.KeyLoadFromSvcsFile(128, keyFile);

            IonicZipHelper zipHelper = new IonicZipHelper(logger);
            CrytoHelper crytoHelper = new CrytoHelper();

            string zipFile = srcFile + ".01.zip";
            string encFile = zipFile + ".p7";

            zipHelper.Zip(zipFile, srcFile);
            crytoHelper.EncryptFile(zipFile, encFile, keyWorker.KeyPublicPfx, 163840);

            File.Delete(crytoHelper.TmpFile);
        }
    }
}
