using System;
using System.Diagnostics;
using System.Media;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystem.SecurityAlarm
{
    public class Player
    {
        private readonly TaskCompletionSource<bool> CompletedTaskCompletionSource = new TaskCompletionSource<bool>();

        public Stopwatch Stopwatch = new Stopwatch();

        public Task<bool> CompletedTask { get { return CompletedTaskCompletionSource.Task; } }

        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

        public async Task Play(PlayType playType, TimeSpan duration, CancellationToken stopToken, CancellationToken restartToken)
        {
            if (!await _sync.WaitAsync(duration))
            {
                return;
            }
            try
            {
                Stopwatch.Restart();
                SoundPlayer p = null;
                SpeechSynthesizer ss = null;
                int count = -1;
                while (Stopwatch.Elapsed < duration && !stopToken.IsCancellationRequested && !restartToken.IsCancellationRequested)
                {
                    count++;
                    if (playType == PlayType.Notify)
                    {
                        if (p == null)
                        {
                            p = new SoundPlayer("Blip.wav");
                        }
                        if (count % 30 == 0)
                        {
                            p.Play();
                        }
                        await Task.Delay(500);
                    }
                    else if (playType == PlayType.Warn)
                    {
                        if (p == null)
                        {
                            p = new SoundPlayer("Warn.wav");
                            p.PlayLooping();
                        }
                        if (count % 10 == 0)
                        {
                            if (ss == null)
                            {
                                ss = new SpeechSynthesizer();
                                ss.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                                ss.Volume = 100;
                            }
                            ss?.SpeakAsync("Disable security system");
                        }
                        await Task.Delay(500);
                    }
                    else if (playType == PlayType.Alarm)
                    {
                        if (p == null)
                        {
                            p = new SoundPlayer("PoliceSiren.wav");
                            p.PlayLooping();
                        }
                        await Task.Delay(500);
                    }
                }
                p?.Stop();
                p?.Dispose();
                p = null;
                ss?.Pause();
                ss?.Dispose();
                ss = null;
                if (!restartToken.IsCancellationRequested && !stopToken.IsCancellationRequested)
                {
                    CompletedTaskCompletionSource.TrySetResult(true);
                }
            }
            finally
            {
                _sync.Release();
            }
        }

        public async Task Stop()
        {
            await Task.Yield();
            var sp = new SoundPlayer("Beep.wav");
            sp.PlaySync();
            CompletedTaskCompletionSource.TrySetResult(true);
        }
    }

    public enum PlayType
    {
        Notify,
        Warn,
        Alarm
    }
}
