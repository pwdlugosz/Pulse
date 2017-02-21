using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Similar to a read stream, except beacon streams only read-forward and work off field-resolvers rather than records
    /// </summary>
    public abstract class BeaconStream
    {

        public BeaconStream(FieldResolver Variants)
        {
            this.Variants = Variants;
        }

        public FieldResolver Variants
        {
            get;
            protected set;
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

        public abstract void Advance();

        public abstract long Position();

    }

}
