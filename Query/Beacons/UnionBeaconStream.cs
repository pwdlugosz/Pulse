using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Beacons
{

    /// <summary>
    /// Represents a union of several beacon streams
    /// </summary>
    public sealed class UnionBeaconStream : BeaconStream
    {

        private List<BeaconStream> _Beacons;
        private int _CurrentBeacon = 0;
        private long _Position = 0;

        public UnionBeaconStream(Host Host)
            : base(Host, new FieldResolver(Host))
        {
            this._Beacons = new List<BeaconStream>();
        }

        public void Add(BeaconStream Beacon)
        {

            // Check if this is not the first one //
            if (this._Beacons.Count != 0)
            {
                if (!this._Beacons[0].Variants.CheckColumnSignitures(Beacon.Variants))
                    throw new ArgumentException("Beacons do not have compatible column constructs");
            }
            else
            {
                this.Variants.Import(Beacon.Variants);
            }
            
            this._Beacons.Add(Beacon);

        }

        public override bool CanAdvance
        {
            get
            {
                if (this._CurrentBeacon == this._Beacons.Count -1 && !this._Beacons[this._CurrentBeacon].CanAdvance) 
                    return false;
                return true;
            }
        }

        public override void Advance()
        {

            if (!this.CanAdvance)
                return;

            if (!this._Beacons[this._CurrentBeacon].CanAdvance)
            {
                this._CurrentBeacon++;
            }

            if (!this.CanAdvance)
                return;

            this._Beacons[this._CurrentBeacon].Advance();
            this.Variants.SetValues(this._Beacons[this._CurrentBeacon].Variants);
            
            this._Position++;

        }

        public override long Position()
        {
            return this._Position;
        }

        public static UnionBeaconStream Union(Host Host, params BeaconStream[] Beacons)
        {
            
            UnionBeaconStream x = new UnionBeaconStream(Host);
            foreach (BeaconStream bs in Beacons)
                x.Add(bs);
            return x;

        }

    }


}
