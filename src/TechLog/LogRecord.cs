// Copyright 2020 Sergey Savelev
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
using SSavel.V8Utils.Platform;

namespace SSavel.V8Utils.TechLog
{
    [Serializable]
    [DataContract]
    public class LogRecord
    {
        // mm:ss.tttttt-d, <name>, <level>, <properties>
        internal static readonly Regex Regex = new Regex(
            @"^(?<Minute>\d+):(?<Second>\d+)\.(?<Microsecond>\d+)-(?<Duration>\d+),(?<Name>\w+),(?<Level>\d+),",
            RegexOptions.Compiled);

        private static readonly Regex RegexProp = new Regex(@",([^,]+?)=(['|""]?)", RegexOptions.Compiled);
        private static readonly Regex RegexQuoter1 = new Regex(@"'+", RegexOptions.Compiled);
        private static readonly Regex RegexQuoter2 = new Regex(@"""+", RegexOptions.Compiled);
        private static readonly KeyValuePair<string, string> Replacer1 = new KeyValuePair<string, string>("''", "'");
        private static readonly KeyValuePair<string, string> Replacer2 = new KeyValuePair<string, string>("\"\"", "\"");

        internal LogRecord(LogFile file, string source, Match initialMatch,
            LogRecordOptions options = LogRecordOptions.None)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (initialMatch == null) throw new ArgumentNullException(nameof(initialMatch));
            //if (file.Version == null) throw new ArgumentNullException(nameof(file.Version));

            File = file;

            var durationDivisor = file.Version < Versions.V83 ? 10 : 1000;

            Time = new DateTime(
                file.Hour.Year,
                file.Hour.Month,
                file.Hour.Day,
                file.Hour.Hour,
                int.Parse(initialMatch.Groups["Minute"].Value),
                int.Parse(initialMatch.Groups["Second"].Value),
                int.Parse(initialMatch.Groups["Microsecond"].Value) / durationDivisor,
                DateTimeKind.Unspecified);
            Duration = long.Parse(initialMatch.Groups["Duration"].Value) / durationDivisor;
            Event = initialMatch.Groups["Name"].Value;
            Level = int.Parse(initialMatch.Groups["Level"].Value);

            if ((options & LogRecordOptions.FillPropertiesString) != 0)
                PropertiesString = source.Substring(initialMatch.Length);


            if ((options & LogRecordOptions.FillProperties) != 0) ParseProperties(source, initialMatch);
        }

        public LogFile File { get; private set; }

        [DataMember] public DateTime Time { get; private set; }
        [DataMember] public long Duration { get; private set; }
        [DataMember] public string Event { get; private set; }
        [DataMember] public int Level { get; private set; }

        [DataMember] public string PropertiesString { get; private set; }

        [DataMember] public IReadOnlyDictionary<string, string> Properties { get; private set; }

        private void ParseProperties(string source, Capture initialMatch)
        {
            var properties = new Dictionary<string, string>();

            var startPos = initialMatch.Length - 1;
            var boundary = source.Length;

            while (startPos < boundary)
            {
                var propMatch = RegexProp.Match(source, startPos);
                if (!propMatch.Success)
                    break;

                var propName = propMatch.Groups[1].Value;
                var propQuote = propMatch.Groups[2].Value;
                string propValue;

                startPos = propMatch.Index + propMatch.Length;

                if (string.IsNullOrEmpty(propQuote)) // Simple property
                {
                    var nextPropMatch = RegexProp.Match(source, startPos);
                    if (nextPropMatch.Success)
                        propValue = source.Substring(startPos, nextPropMatch.Index - startPos);
                    else
                        propValue = source.Substring(startPos);
                    startPos += propValue.Length;
                }
                else // Quoted property
                {
                    var quoteRegex = propQuote == "'" ? RegexQuoter1 : RegexQuoter2;

                    var pos = startPos;
                    while (pos < boundary)
                    {
                        var quoteMatch = quoteRegex.Match(source, pos);
                        if (!quoteMatch.Success) // Didn't find closing quote
                        {
                            pos = boundary;
                        }
                        else if (quoteMatch.Length % 2 == 1) // It's closing quote
                        {
                            pos = quoteMatch.Index + quoteMatch.Length - 1;
                            break;
                        }
                        else // It's inner quote
                        {
                            pos = quoteMatch.Index + quoteMatch.Length;
                        }
                    }

                    propValue = source.Substring(startPos, pos - startPos);
                    startPos = pos + 1;

                    var quoteReplacer = propQuote == "'" ? Replacer1 : Replacer2;
                    propValue = propValue.Replace(quoteReplacer.Key, quoteReplacer.Value);
                }

                properties[propName] = propValue;
                // try
                // {
                //     properties.Add(propName, propValue);
                // }
                // catch (ArgumentException)
                // { }
            }

            Properties = properties;
        }
    }
}