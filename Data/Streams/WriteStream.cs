using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// The base class for all record writers
    /// </summary>
    public abstract class WriteStream
    {

        /// <summary>
        /// Gets the columns of the underlying data structure
        /// </summary>
        public abstract Schema Columns
        {
            get;
        }

        /// <summary>
        /// Inserts a record into the stream
        /// </summary>
        /// <param name="Value"></param>
        public abstract void Insert(Record Value);

        /// <summary>
        /// Inserts many records into the stream
        /// </summary>
        /// <param name="Value"></param>
        public abstract void BulkInsert(IEnumerable<Record> Value);

        /// <summary>
        /// Gets the total number of writes this stream has made
        /// </summary>
        /// <returns></returns>
        public abstract long WriteCount();

        /// <summary>
        /// Closes the stream, releasing all resources; this calls the 'PreSerialize' method form the page table
        /// </summary>
        public abstract void Close();

    }

}
