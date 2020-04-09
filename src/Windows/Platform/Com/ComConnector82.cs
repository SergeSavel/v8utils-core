using System;
using System.Runtime.InteropServices;
using SSavel.V8Utils.Platform;
using V82;

namespace SSavel.V8Utils.Windows.Platform.Com
{
    public class ComConnector82 : IComConnector

    {
        private bool _disposed;
        internal COMConnector ComConnector { get; private set; } = new COMConnector();

        public IAgentConnection ConnectAgent(Agent agent)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            return new AgentConnection82(agent, this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (ComConnector != null)
            {
                Marshal.ReleaseComObject(ComConnector);
                ComConnector = null;
            }

            _disposed = true;
        }

        ~ComConnector82()
        {
            Dispose(false);
        }
    }
}