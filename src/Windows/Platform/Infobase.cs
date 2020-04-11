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
    public class Infobase : IInfobase
    {
        internal Infobase(ICluster cluster)
        {
            Cluster = cluster;
        }

        public ICluster Cluster { get; }

        [DataMember] public string Name { get; internal set; }

        //[DataMember]
        public string Description { get; internal set; }

        public string ConnectionString => $"Srvr=\"{Cluster.Name}:{Cluster.Port.ToString()}\";Ref=\"{Name}\";";

        public override string ToString()
        {
            var portPresentation = Cluster.Port == 1541 ? string.Empty : $":{Cluster.Port.ToString()}";
            return $"{Name}@{Cluster.Host}{portPresentation}";
        }
    }
}