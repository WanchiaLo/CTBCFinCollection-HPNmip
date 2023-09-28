using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using hp.log;

namespace hp.template
{
    public abstract class ConsoleMain
    {
        protected string _name;

        public ConsoleMain(string name, ILogger _logger)
        {
            _name = name;
            Loop = true;
            Logger = _logger;
            //SvcInterval = 5; 20161124 修改
            SvcInterval = 30;
            TimerInterval = 3;
            StartCmdThread = false;
            StartTimerThread = true;
        }

        public bool Loop { get; set; }
        public ILogger Logger { get; set; }
        public int SvcInterval { get; set; }
        public int TimerInterval { get; set; }
        public Thread TimerThread { get; set; }
        public bool StartTimerThread { get; set; }
        public Thread CmdThread { get; set; }
        public bool StartCmdThread { get; set; }

        public abstract bool Start();
        public abstract void Work();
        public abstract void Stop();
        public virtual void  TimerWork() { }
        public virtual void CmdWork(string cmd) { }

        public void Execute()
        {
            Logger.WriteLine(_name, "Execute Start.");
            
            if (Start())
            {
                if (StartTimerThread)
                {
                    ThreadStart myTimer = new ThreadStart(DoTimer);
                    TimerThread = new Thread(myTimer);
                    TimerThread.IsBackground = true;
                    TimerThread.Start();
                }

                if (StartCmdThread)
                {
                    try
                    {
                        Console.TreatControlCAsInput = true;
                    }
                    catch (Exception ex)
                    {
                        StartCmdThread = false;
                        Logger.WriteLine(_name, ex.Message);
                    }

                    ThreadStart myCmd = new ThreadStart(DoCmd);
                    CmdThread = new Thread(myCmd);
                    CmdThread.IsBackground = true;
                    CmdThread.Start();
                }

                while (Loop)
                {
                    try
                    {
                        //* 
                        Work();

                        Logger.WriteLine(_name, "Sleep SvcInterval:" + SvcInterval);
                        
                        Thread.Sleep(SvcInterval * 1000);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(_name, "[Execute.exception]", ex);
                    }
                }
            }

            //* 
            Stop();

            Logger.Trace(_name, "Execute Stop." + Environment.NewLine);
        }

        void DoTimer()
        {
            Logger.WriteLine(_name, "DoTimer Start.");

            while (Loop)
            {
                try
                {
                    TimerWork();
                    Thread.Sleep(TimerInterval * 1000);
                }
                catch (Exception ex)
                {
                    Logger.WriteException(_name, "[DoTimer]", ex);
                }
            }

            Logger.Trace(_name, "DoTimer Stop.");
        }


        void DoCmd()
        {
            string cmd;

            Logger.WriteLine(_name, "DoCmd Start.");

            while (Loop)
            {
                try
                {
                    //cmd = Console.ReadLine().ToLower();
                    cmd = ReadLine().ToLower();

                    if (cmd.Equals("quit") || cmd.Equals("exit"))
                    {
                        Loop = false;
                        Logger.WriteLine(_name, "CMD:" + cmd);
                    }
                    else
                    {
                        CmdWork(cmd);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteException(_name, "[DoCmd]", ex);
                }
            }

            Logger.WriteLine(_name, "DoCmd Stop.");
        }

        private string ReadLine()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            StringBuilder sb = new StringBuilder();
            bool keyLoop = true;

            while (keyLoop && Loop)
            {
                if (Console.KeyAvailable == false)
                {
                    Thread.Sleep(250);
                    continue;
                }

                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Enter)
                {
                    keyLoop = false;
                    continue;
                }
                else if ((cki.Modifiers & ConsoleModifiers.Control) != 0 &&
                         cki.Key == ConsoleKey.C)
                {
                    Logger.WriteLine(_name, "KEY:Ctrl-C");
                    sb.Clear();
                    sb.Append("exit");
                    keyLoop = false;
                    continue;
                }

                sb.Append(cki.Key.ToString());
            }

            return sb.ToString();
        }

    }
}
