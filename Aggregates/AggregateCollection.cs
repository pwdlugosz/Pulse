using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Data;

namespace Pulse.Aggregates
{

    public sealed class AggregateCollection : IColumns
    {

        private Heap<Aggregate> _Aggregates;
        
        public AggregateCollection()
        {
            this._Aggregates = new Heap<Aggregate>();
        }

        public int Count
        {
            get { return this._Aggregates.Count; }
        }

        public int SignitureLength()
        {
            return this._Aggregates.Values.Sum((x) => { return x.SignitureLength(); });
        }

        public Aggregate this[int IndexOf]
        {
            get { return this._Aggregates[IndexOf]; }
        }

        public Aggregate this[string Name]
        {
            get { return this._Aggregates[Name]; }
        }

        public IEnumerable<Aggregate> Expressions
        {
            get { return this._Aggregates.Values; }
        }

        public Schema Columns
        {
            get
            {

                Schema s = new Schema();
                for (int i = 0; i < this._Aggregates.Count; i++)
                {
                    s.Add(this._Aggregates.Name(i), this._Aggregates[i].AggregateAffinity(), this._Aggregates[i].AggregateSize());
                }
                return s;

            }
        }

        public void Add(string Alias, Aggregate Value)
        {
            Value.AggregateCollectionOffset = this._Aggregates.Values.Sum((x) => { return x.SignitureLength(); });
            this._Aggregates.Allocate(Alias, Value);
        }

        /// <summary>
        /// Sets up the values in a work-data record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Initialize(Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Initialize(Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Performs a single accumulation over a single record
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Accumulate(Variants, Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Merges two work data records; this is only used during parallel processing
        /// </summary>
        /// <param name="MergeIntoWork"></param>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        public void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            foreach (Aggregate a in this._Aggregates.Values)
            {
                a.Merge(MergeIntoWork, Work, Offset + a.AggregateCollectionOffset);
            }

        }

        /// <summary>
        /// Finalizes the work data return the final cell result
        /// </summary>
        /// <param name="Work"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public Record Evaluate(Record Work, int Offset)
        {

            Cell[] c = new Cell[this.Count];
            int i = 0;
            foreach (Aggregate a in this._Aggregates.Values)
            {
                c[i] = a.Evaluate(Work, a.AggregateCollectionOffset + Offset);
                i++;
            }
            return new Record(c);

        }



    }


}
