using System;
using System.IO;
using System.Text;

namespace hp.util
{
    public static class MyEncodeUtil
    {
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }

        // Using encoding from BOM or UTF8 if no BOM found,
        // check if the file is valid, by reading all lines
        // If decoding fails, use the local "ANSI" codepage

        //* Test
        ////Stream fs = File.OpenRead(@".\TestData\TextFile_ansi.csv");
        ////var detectedEncoding = DetectFileEncoding(fs);
        public static string DetectFileEncoding(Stream fileStream)
        {
            var Utf8EncodingVerifier = Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
            using (var reader = new StreamReader(fileStream, Utf8EncodingVerifier,
                   detectEncodingFromByteOrderMarks: true, leaveOpen: true, bufferSize: 1024))
            {
                string detectedEncoding;
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                    }
                    detectedEncoding = reader.CurrentEncoding.BodyName;
                }
                catch (Exception e)
                {
                    // Failed to decode the file using the BOM/UT8. 
                    // Assume it's local ANSI
                    detectedEncoding = "ISO-8859-1";
                }
                // Rewind the stream
                fileStream.Seek(0, SeekOrigin.Begin);
                return detectedEncoding;
            }
        }

        #region 轉換BIG5
        /// <summary>
        /// 轉換BIG5
        /// </summary>
        /// <param name=”strUtf”>輸入UTF-8</param>
        /// <returns></returns>
        public static string ConvertBig5(string strUtf8)
        {
            Encoding utf81 = Encoding.GetEncoding("utf-8");
            Encoding big51 = Encoding.GetEncoding(950);
            byte[] strUtf81 = utf81.GetBytes(strUtf8.Trim());
            byte[] strBig51 = Encoding.Convert(utf81, big51, strUtf81);
            char[] big5Chars1 = new char[big51.GetCharCount(strBig51, 0, strBig51.Length)];
            big51.GetChars(strBig51, 0, strBig51.Length, big5Chars1, 0);
            string tempString1 = new string(big5Chars1);
            return tempString1;
        }
        #endregion

        //Example for convert from Big 5 to UTF8:
        public static string ConvertBIG5toUTF8(string big5String)
        {
            Encoding big5 = Encoding.GetEncoding(950);
            Encoding utf8 = Encoding.GetEncoding("utf-8");

            // convert string to bytes
            byte[] big5Bytes = big5.GetBytes(big5String);

            // convert encoding from big5 to utf8
            byte[] utf8Bytes = Encoding.Convert(big5, utf8, big5Bytes);

            char[] utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);

            return new string(utf8Chars);
        }

        public static string ConvertUTF8toBIG5(string utf8String)
        {
            Encoding big5 = Encoding.GetEncoding(950);
            Encoding utf8 = Encoding.GetEncoding("utf-8");

            // convert string to bytes
            byte[] utf8Bytes = utf8.GetBytes(utf8String);

            // convert encoding from big5 to utf8
            byte[] big5Bytes = Encoding.Convert(utf8, big5, utf8Bytes);

            char[] big5Chars = new char[big5.GetCharCount(big5Bytes, 0, big5Bytes.Length)];
            big5.GetChars(big5Bytes, 0, big5Bytes.Length, big5Chars, 0);

            return new string(big5Chars);
        }

        //偵測byte[]是否為BIG5編碼
        public static bool IsBig5Encoding(byte[] bytes)
        {
            Encoding big5 = Encoding.GetEncoding(950);
            //將byte[]轉為string再轉回byte[]看位元數是否有變
            return bytes.Length == big5.GetByteCount(big5.GetString(bytes));
        }

        //偵測檔案否為BIG5編碼
        public static bool IsBig5Encoding(string file)
        {
            if (!File.Exists(file)) return false;
            return IsBig5Encoding(File.ReadAllBytes(file));
        }

        //偵測byte[]是否為utf-8編碼
        public static bool IsUft8Encoding(byte[] bytes)
        {
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            //將byte[]轉為string再轉回byte[]看位元數是否有變
            return bytes.Length == utf8.GetByteCount(utf8.GetString(bytes));
        }

        //偵測檔案否為utf-8編碼
        public static bool IsUft8Encoding(string file)
        {
            if (!File.Exists(file)) return false;
            return IsUft8Encoding(File.ReadAllBytes(file));
        }

    }
}
