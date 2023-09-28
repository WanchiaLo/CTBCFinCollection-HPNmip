using hp.log;
using hp.sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HPNmipSvc
{
    public partial class HPNmipSvc : ServiceBase
    {
        System.Timers.Timer svcTimer;
        ILogger svcLogger;
        ProcessCollection procCollection;        

        //default value
        string _name = "[HPNmip Service]";
        //string _appPath = @"C:\ctbcfin\bankpro\";
        string _appPath = System.IO.Directory.GetParent(Environment.CurrentDirectory.ToString()).ToString();
        string _logPath = @"C:\ctbcfin\bankpro\log\";        
        string _proPath = @"C:\ctbcfin\bankpro\sftp\";
        string _jocsPath = @"C:\ctbcfin\bankpro\sftp\work\";
        string _logDir = @"log\";
        string _binDir = @"bin\";
        string _proDir = @"sftp\";
        string _jocsDir =@"sftp\work\";
        string _ftpDir = @"sftp\ftp\";
        string _binPath = @"C:\ctbcfin\bankpro\bin\";
        string _exeName = "HPNmip.exe";
        string _cmdPath = @"C:\ctbcfin\bankpro\sftp\work\cmd"; // _cmdPath = _appPath + _cmdDir
        string _cmdDir = @"sftp\work\cmd\";  
        string _cmdSTOP = "CMDSTOP";

        int _interval = 60; 

        public HPNmipSvc()
        {
            InitializeComponent();

            try
            {
                _name = this.ServiceName;

                if (ConfigurationManager.AppSettings["AppPath"] != null)
                {
                    _appPath = ConfigurationManager.AppSettings["AppPath"];
                }                

                if (ConfigurationManager.AppSettings["BinDir"] != null)
                {
                    _binPath = _appPath + ConfigurationManager.AppSettings["BinDir"];
                }
                _binPath = _appPath + _binDir;

                if (ConfigurationManager.AppSettings["LogDir"] != null)
                {
                    _logDir = ConfigurationManager.AppSettings["LogDir"];
                }
                _logPath = _appPath + _logDir;

                if (ConfigurationManager.AppSettings["ProDir"] != null)
                {
                    _proPath = ConfigurationManager.AppSettings["ProDir"];
                }
                _proPath = _appPath + _proDir;

                if (ConfigurationManager.AppSettings["JocsDir"] != null)
                {
                    _jocsPath = ConfigurationManager.AppSettings["JocsDir"];
                }
                _jocsPath = _appPath + _jocsDir;

                if (ConfigurationManager.AppSettings["ExeName"] != null)
                {
                    _exeName = ConfigurationManager.AppSettings["ExeName"];
                }

                if (ConfigurationManager.AppSettings["CmdDir"] != null)
                {
                    _cmdDir = ConfigurationManager.AppSettings["CmdDir"];
                }
                _cmdPath = _appPath + _cmdDir;

                if (ConfigurationManager.AppSettings["CmdSTOP"] != null)
                {
                    _cmdSTOP = ConfigurationManager.AppSettings["CmdSTOP"];
                }

                if (ConfigurationManager.AppSettings["Interval"] != null)
                {
                    _interval = Convert.ToInt16(ConfigurationManager.AppSettings["Interval"]);
                }

                svcTimer = new System.Timers.Timer();
                svcTimer.Interval = 1000 * _interval;
                svcTimer.Elapsed += new System.Timers.ElapsedEventHandler(svcTimer_Elapsed);

                ListLogger logger = new ListLogger();
                logger.AddLog(new FileDateLog(string.Format(@"{0}svcs", _logPath), true, 30));
                svcLogger = logger;
                
                svcLogger.WriteLine(_name, string.Format("_interval={0}", _interval));
                svcLogger.WriteLine(_name, string.Format("_appPath={0}", _appPath));
                svcLogger.WriteLine(_name, string.Format("_binPath={0}", _binPath));
                svcLogger.WriteLine(_name, string.Format("_logPath={0}", _logPath));
                svcLogger.WriteLine(_name, string.Format("_proPath={0}", _proPath));
                svcLogger.WriteLine(_name, string.Format("_jocsPath={0}", _jocsPath));
                svcLogger.WriteLine(_name, string.Format("_exeName={0}", _exeName));

                string bufBanks = string.Empty;
                if (ConfigurationManager.AppSettings["Banks"] != null)
                {
                    bufBanks = ConfigurationManager.AppSettings["Banks"];
                }

                procCollection = new ProcessCollection(svcLogger);
                procCollection.CreateProcess += new CreateProcessEventHandler(procCollection_CreateProcess);

                string[] banks = bufBanks.Split(',');
                foreach (string bank in banks)
                {
                    string ID = bank;
                    string AppArgs = "-bank " + bank;
                    FileInfo AppFile = new FileInfo(_binPath + _exeName);

                    ////procCollection.Add(new ProcessInfo
                    ////{
                    ////    ID = bank,                        
                    ////    AppFile = new FileInfo(_binPath + _exeName),
                    ////    AppArgs = "-bank " + bank
                    ////});

                    procCollection.Add(new ProcessInfo
                    {
                        ID = ID,
                        AppFile = AppFile,
                        AppArgs = AppArgs
                    });

                    svcLogger.WriteLine(_name, string.Format("ID={0}", ID));
                    svcLogger.WriteLine(_name, string.Format("AppFile={0}", AppFile.Name));
                    svcLogger.WriteLine(_name, string.Format("AppArgs={0}", AppArgs));
                }
            }
            catch (Exception ex)
            {
                svcLogger.WriteException(_name, "[HPNmipSvc]", ex);
                EventLog.WriteEntry(ex.Message);
                EventLog.WriteEntry(ex.StackTrace);
            }

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                procCollection.Start();
                svcTimer.Start();
            }
            catch (Exception ex)
            {
                svcLogger.WriteException(_name, "[OnStart]", ex);
            }

        }

        protected override void OnStop()
        {
            try
            {
                svcTimer.Stop();
                procCollection.Stop();
            }
            catch (Exception ex)
            {
                svcLogger.WriteException(_name, "[OnStop]", ex);
            }

        }

        private void svcTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                procCollection.CheckAndRemove();
                procCollection.Start();
            }
            catch (Exception ex)
            {
                svcLogger.WriteException(_name, "[Timer]", ex);
            }
        }

        public ProcessBase procCollection_CreateProcess(ILogger logger, ProcessInfo info)
        {
            NmipProcess proc = new NmipProcess(logger, info);
            proc.AppPath = _appPath;
            proc.LogPath = _logPath;
            proc.BinPath = _binPath;
            proc.ProPath = _proPath;
            proc.JocsPath = _jocsPath;
            proc.CmdPath = _cmdPath;
            proc.CmdDir = _cmdDir;
            proc.CmdSTOP = _cmdSTOP;

            svcLogger.WriteLine(_name, string.Format("proc.AppPath={0}", proc.AppPath));
            svcLogger.WriteLine(_name, string.Format("proc.LogPath={0}", proc.LogPath));
            svcLogger.WriteLine(_name, string.Format("proc.BinPath={0}", proc.BinPath));
            svcLogger.WriteLine(_name, string.Format("proc.ProPath={0}", proc.ProPath));
            svcLogger.WriteLine(_name, string.Format("proc.JocsPath={0}", proc.JocsPath));
            svcLogger.WriteLine(_name, string.Format("proc.CmdPath={0}", proc.CmdPath));
            svcLogger.WriteLine(_name, string.Format("proc.CmdDir={0}", proc.CmdDir));
            svcLogger.WriteLine(_name, string.Format("proc.CmdSTOP={0}", proc.CmdSTOP));

            return proc;
        }

    }
}
