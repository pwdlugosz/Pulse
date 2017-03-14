using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query.Beacons;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Acceptors
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
            while (Stream.Go())
            {
                this.Accept(Stream.Variants);
            }
        }

    }

}

