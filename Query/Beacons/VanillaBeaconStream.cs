using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Query.Beacons
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class VanilaBeaconStream : BeaconStream
    {

        private ReadStream _stream;

        public VanilaBeaconStream(Host Host, ReadStream RecordReader, string Alias)
            : base(Host, new FieldResolver(Host))
        {
            this._stream = RecordReader;
            this.Variants.AddSchema(Alias, RecordReader.Columns);
        }

        public override void Advance()
        {
            if (!this._stream.CanAdvance)
                return;
            this.Variants.SetValue(0, this._stream.ReadNext());
        }

        public override bool CanAdvance
        {
            get { return this._stream.CanAdvance; }
        }

        public override long Position()
        {
            return this._stream.Position();
        }

        public static BeaconStream Select(Table Element)
        {
            return new VanilaBeaconStream(Element.Host, Element.OpenReader(), Element.Name);
        }

    }


}
