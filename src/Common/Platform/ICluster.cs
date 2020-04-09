namespace SSavel.V8Utils.Platform
{
    public interface ICluster
    {
        Agent Agent { get; }
        string Host { get; }
        int Port { get; }
        string Name { get; }
    }
}