using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Data
{

    /// <summary>
    /// Acceptors are similar to write streams, but they take field resolvers rather than records
    /// </summary>
    public abstract class AcceptorStream
    {

        public AcceptorStream()
        {
        }

        public abstract void Accept(FieldResolver Variants);

        public abstract long WriteCount();

        public abstract void Close();

        public virtual void Consume(BeaconStream Stream)
        {
            while (Stream.CanAdvance)
            {
                Stream.Advance();
                this.Accept(Stream.Variants);
            }
        }

    }

}

