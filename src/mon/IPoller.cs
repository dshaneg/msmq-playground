using System;

namespace mon
{
    public interface IPoller<T> where T:class
    {
        event EventHandler<T> Polled;
 
        void Poll();
    }
}
