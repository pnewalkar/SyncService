using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Maintel.Icon.Portal.SyncService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main(string[] args)
        {
            
            ServiceBase[] ServicesToRun;
            bool initialiseSites = false;
            bool initialiseTickets = false;
            bool initialiseTypes = false;
            //System.Threading.Thread.Sleep(10000);

            //string[] temp = Environment.GetCommandLineArgs();
            //if (args != null && args.Count() > 1)
            //{
            //    if (args[0].ToLower() == "--intialise")
            //    {
            //        if (args[1].ToLower() =="sites")
            //        {
            //            initialiseSites = true;
            //        } 
            //        else if (args[1].ToLower() == "tickets")
            //        {
            //            initialiseTickets = true;
            //        }
            //        else if (args[1].ToLower() == "types")
            //        {
            //            initialiseTypes = true;
            //        }
            //        else if (args[1].ToLower() == "all")
            //        {
            //            initialiseSites = true;
            //            initialiseTickets = true;
            //            initialiseTypes = true;
            //        }
            //    }
            //}

            ServicesToRun = new ServiceBase[]
            {
                 new SyncService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
