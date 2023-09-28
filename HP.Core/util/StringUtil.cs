using System;
using System.Text;

namespace hp.util
{
    public static class StringUtil
    {
        public static string Size(string s, int len)
        {
            string buf;

            if (s.Length >= len)
                buf = s.Substring(0, len);
            else
            {
                int diff = len - s.Length;
                buf = s + new string(' ', diff);
            }

            return buf;
        }

        public static String[] strSplit(String sI_Source, String sI_Sep)
        {
            String[] L_Result;

            if (sI_Source == null)
                L_Result = null;
            else
                L_Result = sI_Source.Split(sI_Sep.ToCharArray());

            return L_Result;
        }

        public static string Fill(string s, int n)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
                sb.Append(s);

            return sb.ToString();
        }

        public static string Cut(ref string s, int n)
        {
            string buf = "";

            if (s.Length >= n)
            {
                buf = s.Substring(0, n);
                s = s.Substring(n).Trim();
            }
            else
            {
                buf = s;
                s = "";
            }

            return buf;
        }

        public static string Cut(ref string s, string sepa)
        {

            return Cut(ref s, sepa, false);
        }

        public static string Cut(ref string s, string sepa, bool skip)
        {
            string buf = "";
            int pos = s.IndexOf(sepa);

            if (pos >= 0)
            {
                buf = s.Substring(0, pos);
                if (skip)
                    s = s.Substring(pos + sepa.Length).Trim();
                else
                    s = s.Substring(pos).Trim();
            }
            else
            {
                buf = s;
                s = "";
            }

            return buf;
        }

        public static bool IsDigit(string s, int n)
        {
            if (s.Length < n)
                return false;

            char[] chars = s.ToCharArray(0, n);

            foreach (char c in chars)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static String RemoveRepeatString(String orign, String s)
        {
            int len = s.Length;
            String buf = orign;

            if (buf.Length < len)
                return buf;

            while (s.Equals(buf.Substring(0, len)))
            {
                buf = buf.Substring(len);

                if (buf.Length < len)
                    break;
            }

            return buf;
        }

        public static String RemoveString(String buf, String s)
        {
            return buf.Substring(s.Length);
        }

        public static String RemoveNonblankRight(String orign)
        {
            String buf = orign;

            try
            {
                while (!" ".Equals(buf.Substring(buf.Length - 1, 1)))
                {
                    buf = buf.Substring(0, buf.Length - 1);
                }
            }
            catch (Exception)
            {
                buf = orign;
            }

            return buf;
        }

        public static String RemoveDigitRight(String orign)
        {
            String buf = orign;

            try
            {
                while ("0123456789".IndexOf(buf.Substring(buf.Length - 1, 1)) >= 0)
                    buf = buf.Substring(0, buf.Length - 1);
            }
            catch (Exception)
            {
                buf = orign;
            }

            return buf;
        }

    }

}
