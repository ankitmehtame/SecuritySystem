using log4net;
using SecuritySystem.SecurityAlarm;
using System;
using System.IO;
using System.ServiceProcess;

namespace SecuritySystemService
{
    static class Program
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                Directory.SetCurrentDirectory(Executable.GetDirectory(typeof(Program)));
                if (!Environment.UserInteractive)
                {
                    Log.Info("Starting service");
                    var service = new SecuritySystemService();
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new [] { service };
                    ServiceBase.Run(ServicesToRun);
                }
                else
                {
                    Log.Info("Starting as interactive console app. Press enter to exit.");
                    var service = new SecuritySystemService();
                    service.StartService(args);
                    Console.ReadLine();
                    service.StopService();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unhandled exception. {0}\r\n{1}", ex.Message
                    + (ex.InnerException == null ? null : Environment.NewLine + ex.InnerException.Message)
                    , ex.StackTrace);
            }
            finally
            {
                Log.InfoFormat("Stopped");
            }
        }
    }
}
