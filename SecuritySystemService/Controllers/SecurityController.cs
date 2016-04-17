using log4net;
using SecuritySystemService.Helpers;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace SecuritySystemService.Controllers
{
    public class SecurityController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecurityController));

        [HttpGet]
        [ActionName("notify")]
        public async Task<HttpStatusCode> Notify(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("NotifyCommand", "NotifyCommandArgs", new { DurationInSeconds = seconds });
                return HttpStatusCode.Accepted;
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return HttpStatusCode.InternalServerError;
        }

        [HttpGet]
        [ActionName("warn")]
        public async Task<HttpStatusCode> Warn(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("WarnCommand", "WarnCommandArgs", new { DurationInSeconds = seconds });
                return HttpStatusCode.Accepted;
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return HttpStatusCode.InternalServerError;
        }

        [HttpGet]
        [ActionName("alarm")]
        public async Task<HttpStatusCode> Alarm(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("AlarmCommand", "AlarmCommandArgs", new { DurationInSeconds = seconds });
                return HttpStatusCode.Accepted;
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return HttpStatusCode.InternalServerError;
        }

        [HttpGet]
        [ActionName("stop")]
        public async Task<HttpStatusCode> Stop()
        {
            try
            {
                await Task.Yield();
                StartProcess("StopCommand", "StopCommandArgs");
                return HttpStatusCode.Accepted;
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return HttpStatusCode.InternalServerError;
        }

        private void StartProcess(string commandKey, string commandArgsKey, object parameters = null)
        {
            var command = ConfigurationManager.AppSettings[commandKey];
            var arguments = ConfigurationManager.AppSettings[commandArgsKey];

            var cmd = command;
            var args = arguments;

            var paramsDictionary = parameters.ToStringDictionary();
            foreach (var param in paramsDictionary)
            {
                cmd = cmd.Replace($"{{{{{param.Key}}}}}", param.Value);
                args = args.Replace($"{{{{{param.Key}}}}}", param.Value);
            }

            Log.Debug($"Command line: {cmd} {args}");

            var s = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                Verb = "runas",
            };
            //s.UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //s.Password = 
            Process.Start(s);
            //ProcessAsUser.Launch(@"C:\Windows\System32\notepad.exe");
        }
    }
}
