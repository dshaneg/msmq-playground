using System.Threading;
using System.Threading.Tasks;

namespace mon
{
    class PollDriver<T> where T:class
    {
        private readonly IPoller<T> _pollService;
        private readonly IPolledHandler<T> _handlerService;
        private readonly int _pollDelayMilliseconds;

        private bool _started;
        private readonly CancellationTokenSource _cancelSource;

        public PollDriver(
            IPoller<T> pollService, 
            IPolledHandler<T> handlerService, 
            int pollDelayMilliseconds)
        {
            _pollService = pollService;
            _handlerService = handlerService;
            _pollDelayMilliseconds = pollDelayMilliseconds;

            _cancelSource = new CancellationTokenSource();
        }

        public async void Start()
        {
            if (_started)
                return;

            _started = true;
            _pollService.Polled += _handlerService.HandlePolled;
            
            while (!_cancelSource.Token.IsCancellationRequested)
            {
                _pollService.Poll();

                await Task.Delay(_pollDelayMilliseconds, _cancelSource.Token);
            }

            _pollService.Polled -= _handlerService.HandlePolled;
            _started = false;
        }

        public void Stop()
        {
            if (!_started)
                return;

            _cancelSource.Cancel();
        }
    }
}
