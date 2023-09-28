using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using hp.log;

namespace hp.sys
{
    public delegate ProcessBase CreateProcessEventHandler(ILogger Logger, ProcessInfo info);

    public class ProcessCollection
    {
        public event CreateProcessEventHandler CreateProcess;

        List<ProcessBase> procList = new List<ProcessBase>();
        List<ProcessInfo> infoList = new List<ProcessInfo>();

        string _name = "[Procs]";

        public ProcessCollection(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public void Add(ProcessInfo info)
        {
            infoList.Add(info);
        }

        public void Remove(ProcessInfo info)
        {
            infoList.Remove(info);
        }

        public void Start()
        {
            foreach (ProcessInfo info in infoList)
            {
                Boolean findIt = false;

                foreach (ProcessBase p in procList)
                {
                    if (info.ID.Equals(p.ID))
                    {
                        findIt = true;
                        break;
                    }
                }

                if (!findIt)
                {
                    ProcessBase proc = CreateProcess(Logger, info);
                    procList.Add(proc);
                    proc.Start();
                    //Logger.WriteLine(_name, string.Format("Start. Add {0}, PID: {1}", proc.Name, proc.Proc.Id));
                }
            }
        }

        public void Stop()
        {
            foreach (ProcessBase p in procList)
            {
                p.Stop();
            }
        }

        public void CheckAndRemove()
        {
            foreach (ProcessBase p in procList)
            {
                if (!p.IsWork())
                {
                    p.Kill();
                    //p.Close();
                    procList.Remove(p);
                    Logger.WriteLine(_name, string.Format("No Work. Remove {0}, PID: {1}", p.Name, p.Proc.Id));
                    break;
                }
                else if (p.HasExited())
                {
                    //p.Close();
                    procList.Remove(p);
                    Logger.WriteLine(_name, string.Format("HasExited. Remove {0}, PID: {1}", p.Name, p.Proc.Id));
                    break;
                }
            }
        }

    }
}
