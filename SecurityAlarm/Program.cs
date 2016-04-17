using FlawlessCode;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystem.SecurityAlarm
{
    public class Program
    {
        private static readonly string NotifyOption = "Notify";
        private static readonly string WarnOption = "Warn";
        private static readonly string AlarmOption = "Alarm";
        private static readonly string StopOption = "Stop";

        private static CancellationTokenSource StopTokenSource;
        private static CancellationTokenSource RestartTokenSource;

        private static readonly string AppId = "{C4AF5C95-731B-4DD4-8D63-D3441EA2C9C6}";
        private static Lazy<Player> Player = new Lazy<Player>();


        static void Main(string[] args)
        {
            using (var si = new SingleInstance(Guid.Parse(AppId)))
            {
                if (si.IsFirstInstance)
                {
                    Console.WriteLine("First Instance");
                    Directory.SetCurrentDirectory(Executable.GetDirectory());
                    Task.Factory.StartNew(() => Process(args));
                    si.ArgumentsReceived += OnSingleInstanceArgumentsReceived;
                    si.ListenForArgumentsFromSuccessiveInstances();
                    while (!Player.Value.CompletedTask.IsCompleted)
                    {
                        Thread.Sleep(250);
                    }
                }
                else
                {
                    Console.WriteLine("Not First Instance - calling first instance");
                    si.PassArgumentsToFirstInstance(args);
                }
                Console.WriteLine("Exiting");
            }
        }

        private static async Task Process(string[] args)
        {
            Console.WriteLine("Processing args - " + string.Join(" ", args));
            var argsHandler = new ArgumentsHandler(args);
            var notifyValue = argsHandler.GetArgumentValue(NotifyOption).ToNullableInt();
            var warnValue = argsHandler.GetArgumentValue(WarnOption).ToNullableInt();
            var alarmValue = argsHandler.GetArgumentValue(AlarmOption).ToNullableInt();
            var hasStopArg = argsHandler.HasArgument(StopOption);

            if (hasStopArg)
            {
                StopTokenSource?.Cancel();
                await Player.Value.Stop();
            }
            else if (alarmValue != null)
            {
                await PlayAlarm(TimeSpan.FromSeconds(alarmValue.Value));
            }
            else if (warnValue != null)
            {
                await PlayWarning(TimeSpan.FromSeconds(warnValue.Value));
            }
            else if (notifyValue != null)
            {
                await PlayNotification(TimeSpan.FromSeconds(notifyValue.Value));
            }
        }

        private static async void OnSingleInstanceArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
        {
            await Process(e.Args);
        }

        private static async Task PlayNotification(TimeSpan duration)
        {
            await Play(PlayType.Notify, duration);
        }

        private static async Task PlayWarning(TimeSpan duration)
        {
            await Play(PlayType.Warn, duration);
        }

        private static async Task PlayAlarm(TimeSpan duration)
        {
            await Play(PlayType.Alarm, duration);
        }

        private static async Task Play(PlayType playType, TimeSpan duration)
        {
            StopTokenSource = new CancellationTokenSource();
            RestartTokenSource?.Cancel();
            RestartTokenSource = new CancellationTokenSource();
            await Player.Value.Play(playType, duration, StopTokenSource.Token, RestartTokenSource.Token);
        }
    }
}
