using System;
using System.Text;

namespace hp.util
{
    public static class ByteUtil
    {
        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder str = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    str.Append(bytes[i].ToString("X2"));
                }
                hexString = str.ToString();
            }
            return hexString;
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static string GetString(byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        public static byte[] GetBytes(string s)
        {
            return Encoding.Default.GetBytes(s);
        }

        public static byte[] SizeB(string s, int len)
        {
            byte[] bytesSrc = GetBytes(s);
            byte[] bytesDst = new byte[len];

            if (bytesSrc.Length >= len)
            {
                Array.Copy(bytesSrc, bytesDst, len);
            }
            else
            {
                int diff = len - bytesSrc.Length;
                Array.Copy(bytesSrc, bytesDst, bytesSrc.Length);

                for (int i = 0; i < diff; i++)
                {
                    bytesDst[bytesSrc.Length + i] = 0x20;
                }
            }

            return bytesDst;
        }

        public static int CopyBytesWithSize(byte[] bytesDst, int startDst, decimal valSrc, int len, int dec)
        {
            if (dec <= 0)
                return CopyBytesWithSize(bytesDst, startDst, (int)valSrc, len);

            string format = "{0:" + new string('0', len - dec) + "." + new string('0', dec) + "}";
            string strSrc = string.Format(format, valSrc).Replace(".", "");

            return CopyBytesWithSize(bytesDst, startDst, strSrc, len);
        }

        public static int CopyBytesWithSize(byte[] bytesDst, int startDst, int valSrc, int len)
        {
            string format = "{0:" + new string('0', len) + "}";
            string strSrc = string.Format(format, valSrc);

            return CopyBytesWithSize(bytesDst, startDst, strSrc, len);
        }

        public static int CopyBytesWithSize(byte[] bytesDst, int startDst, string strSrc, int len)
        {
            byte[] bytesSrc = SizeB(strSrc, len);

            return CopyBytes(bytesDst, startDst, bytesSrc, 0, len);
        }


        public static int CopyBytes(byte[] bytesDst, int startDst, byte[] bytesSrc, int startSrc, int len)
        {
            Array.Copy(bytesSrc, startSrc, bytesDst, startDst, len);
            return startDst + len;
        }

        public static byte[] AddLength(string s)
        {
            return AddLength(GetBytes(s));
        }

        public static byte[] AddLength(byte[] bytes)
        {
            byte[] bytesNew = new byte[bytes.Length + 4];
            string buf = string.Format("{0:0000}", bytes.Length);

            Array.Copy(GetBytes(buf), 0, bytesNew, 0, 4);
            Array.Copy(bytes, 0, bytesNew, 4, bytes.Length);

            return bytesNew;
        }
    }
}
