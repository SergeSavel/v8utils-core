using System;

namespace SSavel.V8Utils.Platform
{
    public interface ISession
    {
        ICluster Cluster { get; }
        string AppId { get; }
        uint BlockedByDbms { get; }
        uint BlockedByLs { get; }
        ulong BytesAll { get; }
        ulong BytesLast5Min { get; }
        ulong CallsAll { get; }
        ulong CallsLast5Min { get; }
        int? Connection { get; }
        ulong? CpuTimeCurrent { get; }
        ulong? CpuTimeLast5Min { get; }
        ulong? CpuTimeTotal { get; }
        ulong DbmsBytesAll { get; }
        ulong DbmsBytesLast5Min { get; }
        ulong DbmsDurationAll { get; }
        ulong DbmsDurationCurrent { get; }
        ulong DbmsDurationLast5Min { get; }
        string DbProcInfo { get; }
        long DbProcTook { get; }
        DateTime DbProcTookAt { get; }
        ulong DurationAll { get; }
        ulong DurationCurrent { get; }
        ulong DurationLast5Min { get; }
        bool? Hibernate { get; }
        string Host { get; }
        int Id { get; }
        string Infobase { get; }
        DateTime LastActiveAt { get; }
        long? MemoryCurrent { get; }
        long? MemoryLast5Min { get; }
        long? MemoryTotal { get; }
        string ProcessHost { get; }
        string ProcessPid { get; }
        int? ProcessPort { get; }
        ulong? ReadCurrent { get; }
        ulong? ReadLast5Min { get; }
        ulong? ReadTotal { get; }
        string ServiceCurrent { get; }
        ulong? ServiceDurationCurrent { get; }
        ulong? ServiceDurationLast5Min { get; }
        ulong? ServiceDurationTotal { get; }
        DateTime StartedAt { get; }
        string UserName { get; }
        ulong? WriteCurrent { get; }
        ulong? WriteLast5Min { get; }
        ulong? WriteTotal { get; }
    }
}