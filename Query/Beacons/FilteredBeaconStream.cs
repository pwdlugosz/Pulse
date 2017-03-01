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
    /// Represents a beacon stream that filters the underlying data
    /// </summary>
    public class FilteredBeaconStream : BeaconStream
    {

        private BeaconStream _BaseStream;
        private Filter _Where = null;

        public FilteredBeaconStream(Host Host, BeaconStream Base, Filter Where)
            : base(Host, new FieldResolver(Host))
        {
            this._BaseStream = Base;
            this._Where = Where;
            this.Variants.Import(this._BaseStream.Variants);

            //if (!this.CheckFilter())
            //    this.Advance();

        }

        public override void Advance()
        {

            if (!this._BaseStream.CanAdvance)
                return;

            do
            {
                this._BaseStream.Advance();
            } while (this._BaseStream.CanAdvance && !this.CheckFilter());

            this.Variants.SetValues(this._BaseStream.Variants);

        }

        public override bool CanAdvance
        {
            get { return this._BaseStream.CanAdvance; }
        }

        public override long Position()
        {
            return this._BaseStream.Position();
        }

        protected bool CheckFilter()
        {
            if (this._Where == null)
                return true;
            return this._Where.Evaluate(this._BaseStream.Variants);
        }

    }


}
