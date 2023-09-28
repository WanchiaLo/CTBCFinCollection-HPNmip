using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using hp.log;

namespace hp.sys
{
    public class ProcessBase
    {
        public ProcessBase(ILogger logger, ProcessInfo info)
        {
            ID = info.ID;
            AppFile = info.AppFile;
            AppArgs = info.AppArgs;
            Name = string.Format("[PROC{0}]", ID);
            Logger = logger;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public ILogger Logger { get; set; }
        public Process Proc { get; set; }

        public FileInfo AppFile { get; set; }
        public string AppArgs { get; set; }

        public void Start()
        {
            try
            {
                Proc = new Process();
                Proc.StartInfo.FileName = AppFile.FullName;
                Proc.StartInfo.WorkingDirectory = AppFile.DirectoryName;
                Proc.StartInfo.Arguments = AppArgs;
                //Proc.StartInfo.CreateNoWindow = true;
                Proc.StartInfo.UseShellExecute = false;
                Proc.StartInfo.RedirectStandardInput = true;
                Proc.StartInfo.RedirectStandardOutput = true;
                Proc.StartInfo.RedirectStandardError = true;
                Proc.OutputDataReceived += new DataReceivedEventHandler(Proc_OutputDataReceived);
                Proc.Start();

                Proc.BeginOutputReadLine();
                Logger.WriteLine(Name, string.Format("Start. PID: {0}", Proc.Id));
            }
            catch (Exception ex)
            {
                Logger.WriteException(Name, "[Start]", ex);
            }
        }

        public virtual void Stop() { }

        public void Kill()
        {
            try
            {
                Proc.Kill();
                Logger.WriteLine(Name, string.Format("Kill. PID: {0}", Proc.Id));
            }
            catch (Exception ex)
            {
                Logger.WriteException(Name, "[Kill]", ex);
            }
        }

        public void Close()
        {
            try
            {
                Proc.Close();
                Logger.WriteLine(Name, string.Format("Close. PID: {0}", Proc.Id));
            }
            catch (Exception ex)
            {
                Logger.WriteException(Name, "[Close]", ex);
            }
        }

        public string GetProcInfo(string id)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("\tPROC [{0}]: {1:0000}, {2}, {3}, {4}, {5} {6}\n",
                                    id,
                                    Proc.Id,
                                    Proc.ProcessName,
                                    Proc.BasePriority.ToString(),
                                    Proc.WorkingSet64,
                                    Proc.StartInfo.FileName,
                                    Proc.StartInfo.Arguments));

            /*
            foreach (ProcessThread pt in Proc.Threads)
            {
                sb.Append(string.Format("\t\tTHR:{0:0000}, {1}, {2}, {3}, {4}\n", 
                                    pt.Id,
                                    pt.BasePriority.ToString(),
                                    pt.CurrentPriority.ToString(),
                                    pt.ThreadState.ToString(),
                                    pt.WaitReason.ToString()));
            }
             */

            return sb.ToString();
        }

        public virtual bool IsWork() { return true; }

        public bool HasExited()
        {
            return Proc.HasExited;
        }

        void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Logger.WriteLine("", e.Data);
        }
    }
}
