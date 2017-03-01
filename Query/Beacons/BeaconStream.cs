using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Query.Beacons
{

    /// <summary>
    /// Similar to a read stream, except beacon streams only read-forward and work off field-resolvers rather than records
    /// </summary>
    public abstract class BeaconStream
    {

        public BeaconStream(Host Host, FieldResolver Variants)
        {
            this.Variants = Variants;
            this.Host = Host;
        }

        public virtual FieldResolver Variants
        {
            get;
            protected set;
        }

        public Host Host
        {
            get;
            private set;
        }

        public virtual void Advance(int Itterations)
        {
            for (int i = 0; i < Itterations; i++)
                this.Advance();
        }

        public abstract bool CanAdvance
        {
            get;
        }

        public virtual bool Go()
        {
            bool b = this.CanAdvance;
            this.Advance();
            return b;
        }

        public abstract void Advance();

        public abstract long Position();

    }

}
