namespace mon
{
    interface IPolledHandler<in T> where T:class
    {
        void HandlePolled(object source, T args);
    }
}
