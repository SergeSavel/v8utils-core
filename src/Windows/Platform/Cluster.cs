using System.Runtime.Serialization;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.Windows.Platform
{
    [DataContract]
    public class Cluster : ICluster
    {
        internal Cluster(Agent agent)
        {
            Agent = agent;
        }

        public Agent Agent { get; }

        [DataMember] public string Host { get; internal set; }

        [DataMember] public int Port { get; internal set; }

        [DataMember] public string Name { get; internal set; }

        public override string ToString()
        {
            return $"{Host}:{Port.ToString()}";
        }
    }
}