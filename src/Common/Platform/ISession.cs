// Copyright 2020 Serge Savelev
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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