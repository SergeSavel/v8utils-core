using System.Runtime.Serialization;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.Windows.Platform
{
    [DataContract]
    public class Infobase : IInfobase
    {
        internal Infobase(ICluster cluster)
        {
            Cluster = cluster;
        }

        public ICluster Cluster { get; }

        [DataMember] public string Name { get; internal set; }

        //[DataMember]
        public string Description { get; internal set; }

        public string ConnectionString => $"Srvr=\"{Cluster.Name}:{Cluster.Port.ToString()}\";Ref=\"{Name}\";";

        public override string ToString()
        {
            var portPresentation = Cluster.Port == 1541 ? string.Empty : $":{Cluster.Port.ToString()}";
            return $"{Name}@{Cluster.Host}{portPresentation}";
        }
    }
}