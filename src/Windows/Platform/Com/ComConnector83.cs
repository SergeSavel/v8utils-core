using System;
using System.Runtime.InteropServices;
using SSavel.V8Utils.Platform;
using V83;

namespace SSavel.V8Utils.Windows.Platform.Com
{
    public class ComConnector83 : IComConnector
    {
        private bool _disposed;
        internal COMConnector ComConnector { get; private set; } = new COMConnector();

        public IAgentConnection ConnectAgent(Agent agent)
        {
            if (_disposed)
                throw new ObjectDisposedException(ToString());

            return new AgentComConnection83(agent, this);
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

        ~ComConnector83()
        {
            Dispose(false);
        }
    }
}