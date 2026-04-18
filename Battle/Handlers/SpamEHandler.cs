using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class SpamEHandler
    {
        private InputSimulator sim;
        private CancellationTokenSource _cts;
        private Random _rnd = new Random();

        public SpamEHandler(InputSimulator sim)
        {
            this.sim = sim;
        }

        public Task Activate()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
                return Task.CompletedTask;

            _cts = new CancellationTokenSource();
            _ = Loop(_cts.Token);
            return Task.CompletedTask;
        }
        
        public Task Deactivate()
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        private async Task Loop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    sim.KeyPress(Keys.E);
                    Debug.WriteLine("E");
                    await Task.Delay(45 + _rnd.Next(11)); // 40..50
                }
            }
            catch (TaskCanceledException) { }
        }
    }
}
