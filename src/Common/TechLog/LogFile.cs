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
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace SSavel.V8Utils.TechLog
{
    // Represents a single techlog fie.

    [Serializable]
    [DataContract]
    public class LogFile
    {
        private static readonly Regex RegexPath = new Regex(
            @"\\(\w+)_(\d+)\\(\d{2})(\d{2})(\d{2})(\d{2})\.log$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public LogFile(string path, string versionString) : this(path, new Version(versionString))
        {
        }

        public LogFile(string path, Version version)
        {
            var match = RegexPath.Match(path);
            if (!match.Success)
                throw new ArgumentException("Invalid file path.");

            Init(path, version, match);
        }

        private LogFile(string path, Version version, Match match)
        {
            Init(path, version, match);
        }

        [DataMember] public string Path { get; private set; }

        [DataMember] public string ProcessName { get; private set; }

        [DataMember] public int ProcessPid { get; private set; }

        [DataMember] public DateTime Hour { get; private set; }

        public Version Version { get; private set; }

        [DataMember]
        public string VersionString
        {
            get => Version.ToString();
            private set => Version = new Version(value);
        }

        private void Init(string path, Version version, Match match)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            Path = path;

            Version = version;

            ProcessName = match.Groups[1].Value;
            ProcessPid = int.Parse(match.Groups[2].Value);
            Hour = new DateTime(
                2000 + int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value),
                int.Parse(match.Groups[5].Value),
                int.Parse(match.Groups[6].Value),
                0, 0,
                DateTimeKind.Unspecified);
        }

        public static ICollection<LogFile> GetFiles(string path, Version version)
        {
            var filePaths = Directory.GetFiles(path, "*.log", SearchOption.AllDirectories);
            var result = new List<LogFile>(filePaths.Length);

            foreach (var filePath in filePaths)
            {
                var match = RegexPath.Match(filePath);
                if (match.Success) result.Add(new LogFile(filePath, version, match));
            }

            return result.ToArray();
        }

        public static IEnumerable<LogFile> EnumerateFiles(string path, Version version)
        {
            return
                from filePath in Directory.EnumerateFiles(path, "*.log", SearchOption.AllDirectories)
                let match = RegexPath.Match(filePath)
                where match.Success
                select new LogFile(filePath, version, match);
        }
    }
}