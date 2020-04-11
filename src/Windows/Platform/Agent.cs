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
using System.Diagnostics;
using System.Management;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.Windows.Platform
{
    [DataContract]
    public class Agent : IAgent
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

        [DataMember]
        public string VersionString
        {
            get => Version.ToString();
            private set => Version = new Version(value);
        }

        public string ServiceName { get; private set; }
        public string ServiceDisplayName { get; private set; }
        public string ServiceStatus { get; private set; }
        [DataMember] public string ServiceUser { get; private set; }

        [DataMember] public bool Listening { get; private set; }

        [DataMember] public string Path { get; }

        public string CommonDir { get; }
        public string VersionDir { get; }
        public string WorkDir { get; }

        [DataMember] public string Server { get; }

        [DataMember] public int Port { get; }

        public Version Version { get; private set; }

        [DataMember] public bool Debug { get; }

        public string ConnectionString => $"tcp://{Server}:{Port.ToString()}";

        public static ICollection<Agent> GetAllLocal(bool onlyListening = true)
        {
            var result = new List<Agent>();

            var ports = GetRagentListeningPorts();

            var scServices = ServiceController.GetServices();
            foreach (var sc in scServices)
            {
                if (onlyListening && sc.Status != ServiceControllerStatus.Running)
                    continue;

                string path;
                string userName;
                using (var wmiService = new ManagementObject("Win32_Service.Name='" + sc.ServiceName + "'"))
                {
                    wmiService.Get();
                    path = (string) wmiService["PathName"];
                    userName = (string) wmiService["StartName"];
                }

                if (path == null)
                    continue;

                var agent = new Agent(path);

                agent.ServiceName = sc.ServiceName;
                agent.ServiceDisplayName = sc.DisplayName;
                agent.ServiceStatus = sc.Status.ToString();
                agent.ServiceUser = userName;

                agent.Listening = ports.Contains(agent.Port);
                if (onlyListening && !agent.Listening)
                    continue;

                result.Add(agent);
            }

            return result;
        }

        private static ISet<int> GetRagentListeningPorts()
        {
            var ports = new HashSet<int>();
            var pids = new HashSet<string>();

            var processes = Process.GetProcessesByName("ragent");
            foreach (var proc in processes)
                pids.Add(proc.Id.ToString());

            using (var p = new Process())
            {
                var ps = new ProcessStartInfo
                {
                    Arguments = "-a -n -o -p TCP",
                    FileName = "netstat.exe",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                p.StartInfo = ps;
                p.Start();

                var taskError = Task.Run(() => p.StandardError.ReadToEnd());
                var content = p.StandardOutput.ReadToEnd();
                var error = taskError.Result;

                if (p.ExitCode != 0)
                    throw new ApplicationException($"netstat failed: {error}");

                var rows = Regex.Split(content, "\r\n");
                foreach (var row in rows)
                {
                    var tokens = Regex.Split(row, @"\s+");
                    if (tokens.Length >= 6 && "TCP".Equals(tokens[1]) && "LISTENING".Equals(tokens[4]))
                        if (pids.Contains(tokens[5]))
                        {
                            var match = Regex.Match(tokens[2], @":(\d+)");
                            if (match.Success) ports.Add(ushort.Parse(match.Groups[1].Value));
                        }
                }
            }

            return ports;
        }

        public override string ToString()
        {
            return $"{Server}:{Port.ToString()}";
        }
    }
}