using System;
using System.Text;
using System.IO;
using hp.log;

namespace HPSVCS.ask
{
    public class AskCmd : AskWork
    {
        public AskCmd(ILogger logger)
            : base("[AskCmd]", logger)
        {            
        }

        public override bool Run(FileInfo askFile)
        {

            // remane askfile
            if (!Prepare(askFile)) return false;

            /* 代收 文件
             * Command file: I010210101 帳單代收檔 ()
             * Command file: I010210201 帳單代收回覆檔             
            */            

            ErrorCode = 900;
            ErrorMsg = "App services do not support this cmd file. " + askFile.Name;
            Logger.WriteLine(_name, string.Format("{0}: [{1}]", string.Empty + ErrorCode, ErrorMsg));

            return false;
        }

        public bool Prepare(FileInfo askFile)
        {
            string workFile = askFile.FullName + ".wk";
            DeleteFile(workFile);
            File.Move(askFile.FullName, workFile);
            WorkFile = new FileInfo(workFile);
            return true;
        }

    }
}
