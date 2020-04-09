namespace SSavel.V8Utils.Platform
{
    public interface IInfobase
    {
        ICluster Cluster { get; }
        string Name { get; }
        string Description { get; }
        string ConnectionString { get; }
    }
}