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

namespace SSavel.V8Utils
{
    public static class Versions
    {
        public static readonly Version V81 = new Version(8, 1);
        public static readonly Version V82 = new Version(8, 2);
        public static readonly Version V83 = new Version(8, 3);
        public static readonly Version V84 = new Version(8, 4);

        public static bool Is81(Version version)
        {
            return version >= V81 && version < V82;
        }

        public static bool Is82(Version version)
        {
            return version >= V82 && version < V83;
        }

        public static bool Is83(Version version)
        {
            return version >= V83 && version < V84;
        }

        public static Version GetMajor(Version version)
        {
            if (Is81(version)) return V81;
            if (Is82(version)) return V82;
            if (Is83(version)) return V83;
            throw new ArgumentOutOfRangeException(nameof(version));
        }
    }
}