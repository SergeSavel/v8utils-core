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
using System.Globalization;
using System.Runtime.Serialization;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.Windows.Platform
{
    [DataContract]
    public class Session : ISession
    {
        internal Session(ICluster cluster)
        {
            Cluster = cluster;
        }

        public ICluster Cluster { get; }

        [DataMember] public string AppId { get; internal set; }

        [DataMember] public uint BlockedByDbms { get; internal set; }
        [DataMember] public uint BlockedByLs { get; internal set; }

        [DataMember] public ulong BytesAll { get; internal set; }
        [DataMember] public ulong BytesLast5Min { get; internal set; }

        [DataMember] public ulong CallsAll { get; internal set; }
        [DataMember] public ulong CallsLast5Min { get; internal set; }

        [DataMember] public int? Connection { get; internal set; }

        [DataMember] public ulong? CpuTimeCurrent { get; internal set; }
        [DataMember] public ulong? CpuTimeLast5Min { get; internal set; }
        [DataMember] public ulong? CpuTimeTotal { get; internal set; }

        [DataMember] public ulong DbmsBytesAll { get; internal set; }
        [DataMember] public ulong DbmsBytesLast5Min { get; internal set; }

        [DataMember] public ulong DbmsDurationAll { get; internal set; }
        [DataMember] public ulong DbmsDurationCurrent { get; internal set; }
        [DataMember] public ulong DbmsDurationLast5Min { get; internal set; }

        [DataMember] public string DbProcInfo { get; internal set; }
        [DataMember] public long DbProcTook { get; internal set; }
        [DataMember] public DateTime DbProcTookAt { get; internal set; }

        [DataMember] public ulong DurationAll { get; internal set; }
        [DataMember] public ulong DurationCurrent { get; internal set; }
        [DataMember] public ulong DurationLast5Min { get; internal set; }

        [DataMember] public bool? Hibernate { get; internal set; }
        [DataMember] public string Host { get; internal set; }

        [DataMember] public int Id { get; internal set; }
        [DataMember] public string Infobase { get; internal set; }
        [DataMember] public DateTime LastActiveAt { get; internal set; }

        [DataMember] public long? MemoryCurrent { get; internal set; }
        [DataMember] public long? MemoryLast5Min { get; internal set; }
        [DataMember] public long? MemoryTotal { get; internal set; }
        [DataMember] public string ProcessHost { get; internal set; }
        [DataMember] public string ProcessPid { get; internal set; }
        [DataMember] public int? ProcessPort { get; internal set; }

        [DataMember] public ulong? ReadCurrent { get; internal set; }
        [DataMember] public ulong? ReadLast5Min { get; internal set; }
        [DataMember] public ulong? ReadTotal { get; internal set; }

        [DataMember] public string ServiceCurrent { get; internal set; }
        [DataMember] public ulong? ServiceDurationCurrent { get; internal set; }
        [DataMember] public ulong? ServiceDurationLast5Min { get; internal set; }
        [DataMember] public ulong? ServiceDurationTotal { get; internal set; }

        [DataMember] public DateTime StartedAt { get; internal set; }
        [DataMember] public string UserName { get; internal set; }

        [DataMember] public ulong? WriteCurrent { get; internal set; }
        [DataMember] public ulong? WriteLast5Min { get; internal set; }
        [DataMember] public ulong? WriteTotal { get; internal set; }

        public override string ToString()
        {
            return $"{Id.ToString()}@{StartedAt.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}