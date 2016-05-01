using log4net;
using SecuritySystemService.Helpers;
using SecuritySystemService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace SecuritySystemService.Controllers
{
    public class SecurityController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SecurityController));

        private static SecurityAlarmState CurrentState = SecurityAlarmState.Off;
        private static readonly List<Process> ProcessesStarted = new List<Process>();
        private static readonly object SyncLock = new object();

        [HttpGet]
        [ActionName("state")]
        public async Task<SecurityCallStateResult> GetState()
        {
            try
            {
                await Task.Yield();
                return SecurityCallResultHelper.FromSuccessful(CurrentState);
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return SecurityCallResultHelper.FromUnsuccessful(CurrentState);
        }

        [HttpGet]
        [ActionName("notify")]
        public async Task<SecurityCallStateResult> Notify(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("NotifyCommand", "NotifyCommandArgs", new { DurationInSeconds = seconds });
                CurrentState = SecurityAlarmState.Notify;
                return SecurityCallResultHelper.FromSuccessful(CurrentState);
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return SecurityCallResultHelper.FromUnsuccessful(CurrentState);
        }

        [HttpGet]
        [ActionName("warn")]
        public async Task<SecurityCallStateResult> Warn(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("WarnCommand", "WarnCommandArgs", new { DurationInSeconds = seconds });
                return SecurityCallResultHelper.FromSuccessful(CurrentState);
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return SecurityCallResultHelper.FromUnsuccessful(CurrentState);
        }

        [HttpGet]
        [ActionName("alarm")]
        public async Task<SecurityCallStateResult> Alarm(int seconds)
        {
            try
            {
                await Task.Yield();
                StartProcess("AlarmCommand", "AlarmCommandArgs", new { DurationInSeconds = seconds });
                return SecurityCallResultHelper.FromSuccessful(CurrentState);
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return SecurityCallResultHelper.FromUnsuccessful(CurrentState);
        }

        [HttpGet]
        [ActionName("stop")]
        public async Task<SecurityCallStateResult> Stop()
        {
            try
            {
                await Task.Yield();
                StartProcess("StopCommand", "StopCommandArgs");
                return SecurityCallResultHelper.FromSuccessful(CurrentState);
            }
            catch (AggregateException ex)
            {
                Log.Debug($"Aggregate Error in Notify. Error: {ex}");
            }
            catch (Exception ex)
            {
                Log.Debug($"Error in Notify. Error: {ex}");
            }
            return SecurityCallResultHelper.FromUnsuccessful(CurrentState);
        }

        private async Task StartProcess(string commandKey, string commandArgsKey, object parameters = null)
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
            var p = Process.Start(s);
            lock (SyncLock)
            {
                ProcessesStarted.Add(p);
            }
            await p.WaitForExitAsync();
            lock (SyncLock)
            {
                ProcessesStarted.Remove(p);
                if(ProcessesStarted.Count == 0)
                {
                    CurrentState = SecurityAlarmState.Off;
                }
            }
        }
    }
}
