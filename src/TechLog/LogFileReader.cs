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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace SSavel.V8Utils.TechLog
{
    public class LogFileReader
    {
        private static readonly FieldInfo CharPosField = typeof(StreamReader).GetField("charPos",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly FieldInfo CharLenField = typeof(StreamReader).GetField("charLen",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        private static readonly FieldInfo CharBufferField = typeof(StreamReader).GetField("charBuffer",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public LogFileReader(LogFile file, long startPos) : this(file, LogRecordOptions.None, startPos)
        {
        }

        public LogFileReader(LogFile file, LogRecordOptions recordOptions, long startPos = 0)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            RecordOptions = recordOptions;
            Position = startPos;

            UpdateInfo();
        }

        public LogFile File { get; }

        public LogRecordOptions RecordOptions { get; }

        public long Position { get; private set; }

        public long Length { get; private set; }

        public void Read(Action<LogRecord> consumer)
        {
            Read(consumer, CancellationToken.None);
        }

        public void Read(Action<LogRecord> consumer, CancellationToken cancelToken)
        {
            UpdateInfo();

            if (cancelToken.IsCancellationRequested)
                return;

            using (var stream = new FileStream(File.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                stream.Seek(Position, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    var lines = new List<string>();
                    Match previousMatch = null;
                    var position = Position;

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var match = LogRecord.Regex.Match(line);
                        if (match.Success)
                        {
                            if (previousMatch != null)
                            {
                                var recordString = string.Join("\n", lines);
                                var record = new LogRecord(File, recordString, previousMatch, RecordOptions);
                                consumer.Invoke(record);
                            }

                            lines.Clear();
                            previousMatch = match;
                            Position = position;

                            if (cancelToken.IsCancellationRequested)
                                return;
                        }

                        lines.Add(line);
                        position = ActualPosition(reader);
                    }

                    if (Length == stream.Position && previousMatch != null && lines.Count > 0)
                    {
                        var recordString = string.Join("\n", lines);
                        var record = new LogRecord(File, recordString, previousMatch, RecordOptions);
                        consumer.Invoke(record);
                        Position = Length;
                    }
                }
            }
        }

        public void UpdateInfo()
        {
            var fileInfo = new FileInfo(File.Path);
            Length = fileInfo.Length;
        }

        private static long ActualPosition(StreamReader reader)
        {
            var charBuffer = (char[]) CharBufferField.GetValue(reader);
            var charLen = (int) CharLenField.GetValue(reader);
            var charPos = (int) CharPosField.GetValue(reader);

            return reader.BaseStream.Position -
                   reader.CurrentEncoding.GetByteCount(charBuffer, charPos, charLen - charPos);
        }
    }
}