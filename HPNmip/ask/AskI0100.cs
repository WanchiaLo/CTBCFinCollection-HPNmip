using System;
using System.Text;
using System.IO;
using hp.log;

namespace HPSVCS.ask
{
    public class AskI0100 : AskWork
    {
        public AskI0100(ILogger logger)
            : base("[AskI00]", logger)
        {
        }
        
        public override bool Run(FileInfo askFile)
        {

            string[] L_AllFiles = Directory.GetFiles(Param.WorkPath);
            foreach (string filePath in L_AllFiles)
            {
                File.Delete(filePath);
            }           

            return true;
        }

    }
}
