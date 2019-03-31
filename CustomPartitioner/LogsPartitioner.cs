using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomPartitioner
{
    public class LogsPartitioner : Partitioner<string>
    {
        private readonly Logs logs;

        public LogsPartitioner(string filePath)
        {
            this.logs = new Logs(File.ReadLines(filePath));
        }

        public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
        {
            if (partitionCount <= 0)
                throw new ArgumentOutOfRangeException();

            var partitions = new List<string>[partitionCount];
            for (var i = 0; i < partitionCount; i++)
                partitions[i] = new List<string>();

            var chunkSize = 1;
            var currentChunkCount = 1;
            var currentPartition = 0;
            foreach (var line in this.logs)
            {
                if (currentChunkCount > chunkSize)
                {
                    chunkSize++;
                    currentChunkCount = 1;
                    currentPartition = (currentPartition + 1) % partitionCount;
                }

                partitions[currentPartition].Add(line);
                currentChunkCount++;
            }

            return partitions.Select(list => list.AsEnumerable().GetEnumerator()).ToList();
        }
    }

    public class Logs : IEnumerable<string>
    {
        private readonly IEnumerable<string> originalStrings;

        public Logs(IEnumerable<string> rawStringsEnumerable)
        {
            this.originalStrings = rawStringsEnumerable;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new LogsEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class LogsEnumerator : IEnumerator<string>
        {
            private readonly Regex datePrefixRegex = new Regex(@"^\d{4}-\d{2}-\d{2}");
            private readonly IEnumerator<string> originalEnumerator;
            private readonly StringBuilder sb = new StringBuilder();

            public LogsEnumerator(Logs logs)
            {
                this.originalEnumerator = logs.originalStrings.GetEnumerator();
            }

            public string Current { get; private set; }

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
            }

            public void Reset()
            {
            }

            public bool MoveNext()
            {
                if (!this.originalEnumerator.MoveNext())
                    return false;

                this.Current = GetCurrentLog();
                return true;
            }

            private string GetCurrentLog()
            {
                var str = this.originalEnumerator.Current;

                if (str == null)
                    throw new NullReferenceException();

                if (this.datePrefixRegex.IsMatch(str))
                    return str;

                this.sb.Clear();
                this.sb.Append(str);

                var currentString = str;
                while (this.originalEnumerator.MoveNext() && !currentString.Substring(0, 6).Equals("   ---"))
                {
                    currentString = this.originalEnumerator.Current;
                    this.sb.Append(currentString);
                    this.sb.Append('\n');
                }

                return this.sb.ToString();
            }
        }
    }
}