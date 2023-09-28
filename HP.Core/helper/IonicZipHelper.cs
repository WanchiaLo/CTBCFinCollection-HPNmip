using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ionic.Zip;
using hp.log;

namespace hp.helper
{
    public class IonicZipHelper
    {
        //string _name = "[ZipHelper]";

        public IonicZipHelper(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public void Zip(string zipFilename, string fileToPack)
        {
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.AddFile(fileToPack, "");
                zipFile.Save(zipFilename);
            }
        }

        public void ZipPwsd(string zipFilename, string fileToPack)
        {
            using (ZipFile zipFile = new ZipFile())
            {
                zipFile.Password = "1234";
                zipFile.AddFile(fileToPack, "");
                zipFile.Save(zipFilename);
            }
        }

        public void UnzipPwsd(string zipToUnpack, string unzipPath, string unzipPwsd)
        {
            using (ZipFile zipFile = ZipFile.Read(zipToUnpack))
            {
                if (!unzipPwsd.Equals(string.Empty))
                {
                    zipFile.Password = unzipPwsd;
                }
                foreach (ZipEntry entry in zipFile)
                {
                    entry.Extract(unzipPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public void Zip(string zipFilename, List<string> filesToPack)
        {
            using (ZipFile zipFile = new ZipFile())
            {
                foreach (string file in filesToPack)
                {
                    zipFile.AddFile(file, "");
                }

                zipFile.Save(zipFilename);
            }
        }

        public void Zip(string zipFilename, List<string> filesToPack, string zipPath)
        {
            using (ZipFile zipFile = new ZipFile())
            {
                foreach (string f in filesToPack)
                {
                    if (File.Exists(zipPath + f))
                        zipFile.AddFile(zipPath + f, "");
                }

                zipFile.Save(zipFilename);
            }
        }      

        public List<string> Unzip(string zipToUnpack, string unzipPath)
        {
            List<string> UnzipFiles = new List<string>();

            using (ZipFile zipFile = ZipFile.Read(zipToUnpack))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    entry.Extract(unzipPath, ExtractExistingFileAction.OverwriteSilently);
                    UnzipFiles.Add(entry.FileName);
                }
            }

            return UnzipFiles;
        }

        public List<string> UnzipListPwsd(string zipToUnpack, string unzipPath, string unzipPwsd)
        {
            List<string> UnzipFiles = new List<string>();

            using (ZipFile zipFile = ZipFile.Read(zipToUnpack))
            {
                if (!unzipPwsd.Equals(string.Empty))
                {
                    zipFile.Password = unzipPwsd;
                }

                foreach (ZipEntry entry in zipFile)
                {
                    entry.Extract(unzipPath, ExtractExistingFileAction.OverwriteSilently);
                    UnzipFiles.Add(entry.FileName);
                }
            }

            return UnzipFiles;
        }

    }
}
