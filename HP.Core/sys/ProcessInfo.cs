using System;
using System.Text;
using System.IO;

namespace hp.sys
{
    public class ProcessInfo
    {
        public string ID { get; set; }
        public FileInfo AppFile { get; set; }
        public string AppArgs { get; set; }
    }
}
