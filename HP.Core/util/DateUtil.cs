using System;

namespace hp.util
{
    public static class DateUtil
    {
        public static string GetDate(string format)
        {
            return string.Format("{0:" + format + "}", DateTime.Now);
        }

        public static string GetSysDate()
        {
            return GetDate("yyyyMMdd");
        }

        public static string GetSysDate100C6()
        {
            //民國100年問題，yymmdd
            string date = GetSysDate();
            long yy = long.Parse(date.Substring(0, 4)) - 1911;

            return string.Format("{0:000}", yy).Substring(1, 2) + date.Substring(4, 4);
        }

        public static string GetSysDate100C7()
        {
            //民國100年問題，yyymmdd
            string date = GetSysDate();
            long yy = long.Parse(date.Substring(0, 4)) - 1911;

            return string.Format("{0:000}", yy) + date.Substring(4, 4);
        }

        public static string GetSysTime()
        {
            return GetDate("HHmmss");
        }

        public static string GetSysDateTime()
        {
            return GetDate("yyyyMMddHHmmss");
        }

        public static string RelateYMD(string psDate, int piYY, int piMM, int piDD)
        {
            if (String.IsNullOrEmpty(psDate)) return "";
            if (psDate.Length != 8)
            {
                return "Date Format is error: [" + psDate + "]";
            }
            string sFmt = psDate.Substring(0, 4) + "/" + psDate.Substring(4, 2) + "/" + psDate.Substring(6, 2);
            DateTime dt;
            try
            {
                dt = Convert.ToDateTime(sFmt);
            }
            catch { return "Date is error:" + psDate; }

            dt = dt.AddYears(piYY);
            dt = dt.AddMonths(piMM);
            dt = dt.AddDays(piDD);

            return dt.ToString("yyyyMMdd");
        }

        public static string RelateYMD(int piYY, int piMM, int piDD)
        {
            string ls_today = DateTime.Today.ToString("yyyyMMdd");
            return RelateYMD(ls_today, piYY, piMM, piDD);
        }

        public static string RelateYM(string psDate, int piYY, int piMM)
        {
            if (String.IsNullOrEmpty(psDate)) return "";
            return RelateYMD(psDate + "01", piYY, piMM, 0).Substring(0, 6);
        }

        public static string RelateYM(int piYY, int piMM)
        {
            //-return YYYYMM-
            string ls_today = DateTime.Today.ToString("yyyyMMdd");
            return RelateYMD(ls_today, piYY, piMM, 0).Substring(0, 6);
        }

    }
}
