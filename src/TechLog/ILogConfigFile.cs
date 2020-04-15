using System;

namespace SSavel.V8Utils.TechLog
{
    public interface ILogConfigFile
    {
        string Path { get; }
        Version Version { get; }
    }
}