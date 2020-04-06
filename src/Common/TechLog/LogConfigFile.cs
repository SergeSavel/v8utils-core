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
using System.IO;
using System.Text.RegularExpressions;
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.TechLog
{
    public class LogConfigFile
    {
        private static readonly Regex RegexConfLocation = new Regex(@"^\s*ConfLocation\s*=\s*(.+)\s*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        internal readonly string Path;

        internal readonly Version Version;

        private LogConfigFile(string path, Version version)
        {
            Path = path;
            Version = version;
        }

        public static LogConfigFile[] GetFiles(IEnumerable<Agent> agents)
        {
            var result = new List<LogConfigFile>();

            var majorVersions = new HashSet<Version>();
            foreach (var agent in agents)
                majorVersions.Add(Versions.GetMajor(agent.Version));

            foreach (var majorVersion in majorVersions)
            {
                var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var agent in agents)
                {
                    if (Versions.GetMajor(agent.Version) != majorVersion)
                        continue;

                    string confLocation = null;

                    var confCfgPath = System.IO.Path.Combine(agent.VersionDir, @"bin\conf\conf.cfg");
                    if (File.Exists(confCfgPath))
                    {
                        var confCfgText = File.ReadAllText(confCfgPath);
                        var match = RegexConfLocation.Match(confCfgText);
                        if (match.Success)
                            confLocation = match.Groups[1].Value;
                    }

                    if (confLocation == null)
                        confLocation = System.IO.Path.Combine(agent.CommonDir, "conf");

                    var path = System.IO.Path.Combine(confLocation, "logcfg.xml");
                    paths.Add(path);
                }

                foreach (var path in paths)
                    result.Add(new LogConfigFile(path, majorVersion));
            }

            return result.ToArray();
        }
    }
}