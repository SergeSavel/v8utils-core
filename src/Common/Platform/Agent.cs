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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SSavel.V8Utils.Platform
{
    [DataContract]
    public class Agent
    {
        private static readonly Regex RxPath =
            new Regex(@"(([^""]*\\1C[^""]*\\)(\d+\.\d+\.\d+\.\d+)\\)bin\\ragent\.exe",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RxParamQuoted = new Regex(@"^\s*""([^""]*)""\s*", RegexOptions.Compiled);
        private static readonly Regex RxParamCommon = new Regex(@"^\s*(\S+)\s*", RegexOptions.Compiled);

        public Agent(string commandLine)
        {
            if (commandLine == null) throw new ArgumentNullException(nameof(commandLine));

            commandLine = commandLine.Trim();

            var pathMatch = RxPath.Match(commandLine);
            if (!pathMatch.Success)
                throw new ArgumentException("Unexpected command line.");

            Path = pathMatch.Groups[0].Value;
            VersionDir = pathMatch.Groups[1].Value;
            CommonDir = pathMatch.Groups[2].Value;

            VersionString = pathMatch.Groups[3].Value;

            var position = 0;
            var boundary = commandLine.Length;
            var args = new List<string>();
            while (position < boundary)
            {
                var match = RxParamQuoted.Match(commandLine, position);
                if (!match.Success)
                    match = RxParamCommon.Match(commandLine, position);
                if (!match.Success)
                    throw new ArgumentException("Unexpected command line.");

                args.Add(match.Groups[1].Value);
                position += match.Length;
            }

            Server = Environment.MachineName;

            string prevArg = null;
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-debug":
                        Debug = true;
                        break;
                }

                switch (prevArg)
                {
                    case "-port":
                        Port = int.Parse(arg);
                        break;
                    case "-d":
                        WorkDir = arg;
                        break;
                }

                prevArg = arg;
            }
        }

        [DataMember] public string Path { get; }

        public string CommonDir { get; }
        public string VersionDir { get; }
        public string WorkDir { get; }

        [DataMember] public string Server { get; }

        [DataMember] public int Port { get; }

        public Version Version { get; private set; }

        [DataMember]
        public string VersionString
        {
            get => Version.ToString();
            private set => Version = new Version(value);
        }

        [DataMember] public bool Debug { get; }

        public string ConnectionString => $"tcp://{Server}:{Port.ToString()}";

        public override string ToString()
        {
            return $"{Server}:{Port.ToString()}";
        }
    }
}