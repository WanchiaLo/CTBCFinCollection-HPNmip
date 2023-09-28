using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace hp.util
{
    public class DirUtil
    {
        /// <summary>
        /// 取得特定目錄下的檔案
        /// </summary>
        /// <remarks>create by Tom on 2010/10/01</remarks>
        /// <param name="path">特定目錄</param>
        /// <returns></returns>
        public static List<FileInfo> GetFiles(string path)
        {
            if (Directory.Exists(path) == false)
                throw new DirectoryNotFoundException();

            List<FileInfo> fileList = new List<FileInfo>();
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files)
            {
                FileInfo fi = new FileInfo(fileName);
                fileList.Add(fi);
            }

            return fileList;
        }
        
    }
}
