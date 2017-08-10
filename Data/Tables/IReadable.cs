using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    public interface IReadable : IRecyclable, IColumns
    {

        RecordReader OpenReader();

        bool IsIndexedBy(Key Columns);

        long EstimatedRecordCount { get; }

    }




}
