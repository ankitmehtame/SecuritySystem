using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystemService.Helpers
{
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<Process>();
            process.EnableRaisingEvents = true;
            EventHandler handler = null;
            handler = (sender, args) =>
            {
                process.Exited -= handler;
                tcs.TrySetResult(process);
            };
            process.Exited += handler;
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);
            return tcs.Task;
        }
    }
}
