using System;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.Windows.Platform.Com
{
    public interface IComConnector : IDisposable
    {
        IAgentConnection ConnectAgent(Agent agent);
    }
}