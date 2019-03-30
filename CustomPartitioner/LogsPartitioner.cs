using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CustomPartitioner
{
    public class LogsPartitioner : Partitioner<string>
    {
        public LogsPartitioner(string filePath)
        {
            throw new System.NotImplementedException();
        }
        
        public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
        {
            throw new System.NotImplementedException();
        }
    }
}