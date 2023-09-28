using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace hp.util
{
    public class FileUtil
    {
        public static string GetLine(string path)
        {
            string buf = "";

            if(File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    if (sr.Peek() >= 0)
                        buf = sr.ReadLine();
                }
            }

            return buf;
        }

        public static List<string> GetLines(string path)
        {
            return GetLines(path, false);
        }

        public static void writeToFile(List<string> P_SrcList, string sP_TargetFullPathFileName)
        {
            File.AppendAllLines(sP_TargetFullPathFileName, P_SrcList, Encoding.Default);
            
        }

        public static List<string> GetLines(string path, bool isTrim, int nP_SkipLineCount)
        {
            List<string> lines = new List<string>();
            string sL_Tmp="";
            int nL_LinePos=0;
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    
                    while (sr.Peek() >= 0)
                    {
                        if (isTrim)
                            sL_Tmp = sr.ReadLine().Trim();
                        else
                            sL_Tmp = sr.ReadLine();
                        nL_LinePos++;

                        if (nL_LinePos > nP_SkipLineCount)
                        {
                            lines.Add(sL_Tmp);
                            
                        }
                        
                    }
                }
            }

            return lines;
        }
 
        public static List<string> GetLines(string path, bool isTrim)
        {
            List<string> lines = new List<string>();

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    while (sr.Peek() >= 0)
                    {
                        if (isTrim)
                            lines.Add(sr.ReadLine().Trim());
                        else
                            lines.Add(sr.ReadLine());
                    }
                }
            }

            return lines;
        }
        public static string GetFirstLine(string sP_FullPathFileName)
        {
            string sL_FirstLine="";

            if (File.Exists(sP_FullPathFileName))
            {
                using (StreamReader sr = new StreamReader(sP_FullPathFileName, Encoding.Default))
                {
                    while (sr.Peek() >= 0)
                    {
                        sL_FirstLine =sr.ReadLine();
                        break;
                    }
                }
            }

            return sL_FirstLine;
        }
    }
}
