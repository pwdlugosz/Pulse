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
    /// Represents a beacon stream that has a different output column set
    /// </summary>
    public class SelectBeaconStream : BeaconStream
    {

        private BeaconStream _stream;
        private ExpressionCollection _Fields;
        
        public SelectBeaconStream(Host Host, BeaconStream Base, ExpressionCollection Fields)
            : base(Host, new FieldResolver(Host))
        {
            this._stream = Base;
            this.Variants.AddSchema("@@@", Fields.Columns);
            this._Fields = Fields;
            this.Variants.SetValue(0, this._Fields.Evaluate(this._stream.Variants));
        }

        public override void Advance()
        {

            this._stream.Advance();
            this.Variants.SetValue(0, this._Fields.Evaluate(this._stream.Variants));
        }

        public override bool CanAdvance
        {
            get { return this._stream.CanAdvance; }
        }

        public override long Position()
        {
            return this._stream.Position();
        }

    }



}

