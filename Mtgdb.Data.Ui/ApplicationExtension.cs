using System;
using System.Threading.Tasks;

namespace Mtgdb.Ui
{
    public static class ApplicationExtension
    {
        public static Task RunOnSignal(this IApplication app, Action action, AsyncSignal signal) =>
            app.When(signal).Run(action);

        public static Task Wait(this IApplication app, AsyncSignal signal) =>
            signal.Wait(app.CancellationToken);

        public static Task WaitAll(this IApplication app, AsyncSignal signal1, AsyncSignal signal2) =>
            Task.WhenAll(
                signal1.Wait(app.CancellationToken),
                signal2.Wait(app.CancellationToken));

        public static IDeferredCallback When(this IApplication app, AsyncSignal signal) =>
            new DeferredCallback(app, signal);

        private class DeferredCallback : IDeferredCallback
        {
            private readonly IApplication _app;
            private readonly AsyncSignal _signal;

            public DeferredCallback(IApplication app, AsyncSignal signal)
            {
                _app = app;
                _signal = signal;
            }

            public Task Run(Action callback) =>
                Task.Run(async () =>
                {
                    await _signal.Wait(_app.CancellationToken);
                    callback();
                }, _app.CancellationToken);
        }
    }

    public interface IDeferredCallback
    {
        Task Run(Action action);
    }
}
