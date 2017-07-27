using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents a 'String' keyed collection that allows either keyed index lookups or direct index lookups
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T>
    {

        protected Dictionary<string, int> _RefSet;
        protected List<T> _Heap;
        protected List<bool> _IsReadOnly;
        internal Guid _UID;

        public Heap()
        {
            _RefSet = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _Heap = new List<T>();
            _IsReadOnly = new List<bool>();
            this.Identifier = "UNKNOWN";
            this._UID = Guid.NewGuid();
        }

        // Properties //
        public int Count
        {
            get { return this._RefSet.Count; }
        }

        public T this[string Name]
        {

            get
            {
                return this._Heap[this._RefSet[Name]];
            }

            set
            {
                this._Heap[this._RefSet[Name]] = value;
            }

        }

        public T this[int Pointer]
        {

            get
            {
                return this._Heap[Pointer];
            }

            set
            {
                this._Heap[Pointer] = value;
            }

        }

        public string Identifier
        {
            get;
            set;
        }

        // Methods //
        public bool Exists(string Name)
        {
            return this._RefSet.ContainsKey(Name);
        }

        public int GetPointer(string Name)
        {
            return this._RefSet[Name];
        }

        public void Allocate(string Name, T Value)
        {
            if (this.Exists(Name))
                throw new Exception(string.Format("Cannot allocate '{0}', an allocation with that name already exists", Name));
            this._RefSet.Add(Name, this._Heap.Count);
            this._Heap.Add(Value);
        }

        public void Deallocate(string Name)
        {

            if (this.Exists(Name))
            {
                int ptr = this.GetPointer(Name);
                this._RefSet.Remove(Name);
            }

        }

        public void Reallocate(string Name, T Value)
        {

            if (this.Exists(Name))
            {
                this[Name] = Value;
                return;
            }

            this.Allocate(Name, Value);

        }

        internal void Collide(string Name, T Value, int Pointer)
        {
            if (this.Exists(Name))
                throw new Exception(string.Format("Cannot allocate '{0}', an allocation with that name already exists", Name));
            this._RefSet.Add(Name, Pointer);
            this._Heap.Add(Value);
        }

        public void Vacum()
        {

            List<T> NewHeap = new List<T>();

            int NewPointer = 0;

            foreach (KeyValuePair<string, int> kv in this._RefSet)
            {

                // Accumulate a Value to the new heap //
                NewHeap.Add(this._Heap[kv.Value]);

                // Reset the pointer //
                this._RefSet[kv.Key] = NewPointer;

                // Increment the pointer //
                NewPointer++;

            }

            // Point the new heap //
            this._Heap = NewHeap;

        }

        public string Name(int Pointer)
        {
            return this._RefSet.Keys.ToArray()[Pointer];
        }

        public Dictionary<string, T> Entries
        {
            get
            {
                Dictionary<string, T> values = new Dictionary<string, T>();
                foreach (KeyValuePair<string, int> kv in this._RefSet)
                    values.Add(kv.Key, this[kv.Value]);
                return values;
            }
        }

        public List<T> Values
        {
            get { return this._Heap; }
        }

        public void Import(Heap<T> Value)
        {

            foreach (KeyValuePair<string, T> x in Value.Entries)
            {

                if (!this.Exists(x.Key))
                    this.Allocate(x.Key, x.Value);

            }

        }

    }

}
