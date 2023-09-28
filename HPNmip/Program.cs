using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using hp.log;
using hp.util;
using HPSVCS.sys;
using hp.helper;

namespace HPSVCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();

            if (args.Length > 0)
            {
                if (args[0].Equals("-bank"))
                {
                    program.RunSVCS(args);
                }
                else if (args[0].Equals("-ftp"))
                    program.RunFtp(args);
                else if (args[0].Equals("-sftp"))
                    program.RunSFtp(args);
                else if (args[0].Equals("-test"))
                    program.RunTest();
                else
                {
                    program.ShowHelp();
                }
            }
            else
            {
                program.ShowHelp();
            }
        }

        void ShowHelp()
        {
            Console.WriteLine("HPNmip [-ftp][-bank]");
        }

        void RunSVCS(string[] args)
        {
            try
            {
                SysParam param = new SysParam(args[1]);

                ListLogger logger = new ListLogger();
                logger.AddLog(new BaseLog());
                logger.AddLog(new FileDateLog(string.Format(@"{0}nmip{1}", param.LogPath, param.BankID), true, 30));

                ////logger.WriteLine("[SvcsMain]", "HPNmip: Version 20220219. Bank:" + param.BankID);

                try
                {
                    SvcsMain main = new SvcsMain("[SvcsMain]", logger);
                    main.Param = param;
                    main.Execute();
                }
                catch (Exception ex)
                {
                    logger.WriteException("HPNmip" + param.BankID, "Main", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //EventLog.WriteEntry(ex.Message);
                //EventLog.WriteEntry(ex.StackTrace);
            }
        }

        void RunFtp(string[] args)
        {
            BaseLogger logger = new BaseLogger();
            logger.ShowDateTime(false);

            string scriptFile = "ftp.cmd";

            if (args.Length > 1) scriptFile = args[1];

            ChilkatFTPHelper ftpHelper = new ChilkatFTPHelper(logger);
            ftpHelper.DoScript(scriptFile);
        }

        void RunSFtp(string[] args)
        {
            BaseLogger logger = new BaseLogger();
            logger.ShowDateTime(false);

            string scriptFile = "sftp.cmd";

            if (args.Length > 1) scriptFile = args[1];

            ChilkatFTPHelper ftpHelper = new ChilkatFTPHelper(logger);
            ftpHelper.DoScript(scriptFile);
        }
        
        void RunTest()
        {            
        }
    }
}
