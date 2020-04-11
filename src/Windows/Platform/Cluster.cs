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