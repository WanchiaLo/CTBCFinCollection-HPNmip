using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ctbcfin.core.datacls
{
    public class BULKPAYMEMTRQ
    {
        public string REGION; //* 交易 ID 國別
        public string ID; //* 交易 ID 
        public string QRYSEQNO; //* 查詢-交易傳送序號
        public string QRYTRNID; //* 查詢-APIM 交易序號
        public string QRYTYPE; //* 回覆結果方式
        public string RESULTTYPE; //* 查詢交易狀態
        public string TXDAT; //* 客戶指定交易日期
        public string SERNUM; //* 單一明細序號
        public string ACSRNO; //* 連接序號
    }

    public class INFO
    {
        public string FUNNAM;
        public string DATTYP;
        public string ENCODING;
        public string DATE;
        public string TIME;
        public string SEQNO;
        public string LOGREGION;
    }

    public class BCICRQ
    {        
        public BULKPAYMEMTRQ CDATA;
    }
}
