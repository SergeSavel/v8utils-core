using System;
using System.Collections.Generic;

namespace SSavel.V8Utils.Platform
{
    public interface IAgentConnection : IDisposable
    {
        Agent Agent { get; }

        ICollection<ICluster> GetClusters();

        ICollection<IInfobase> GetInfobases(ICluster cluster);

        ICollection<ISession> GetSessions(ICluster cluster);
    }
}