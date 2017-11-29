using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Expressions.TableExpressions
{


    public sealed class TableExpressionValue : TableExpression
    {

        private Table _t;

        public TableExpressionValue(Host Host, TableExpression Parent, Table Value)
            : base(Host, Parent)
        {
            this._t = Value;
            this.Alias = "VALUE";
        }

        public override Schema Columns
        {
            get { return this._t.Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants;
        }

        // Evaluates //
        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {
            Writer.Consume(this._t.OpenReader());
        }

        public override Table Select(FieldResolver Variants)
        {
            return this._t;
        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                return this._t.RecordCount;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return this._t.IsIndexedBy(IndexColumns);
        }

    }

    public sealed class TableExpressionStoreRef : TableExpression
    {

        private string _Name;
        private ObjectStore _Store;

        public TableExpressionStoreRef(Host Host, TableExpression Parent, string Name, ObjectStore Store)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._Store = Store;
        }

        public override Schema Columns
        {
            get { return this._Host.OpenTable(this._Store.GetTable(this._Name)).Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants;
        }

        // Evaluates //
        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {
            Table t = this._Host.OpenTable(this._Store.GetTable(this._Name));
            Writer.Consume(t.OpenReader());
        }

        public override Table Select(FieldResolver Variants)
        {
            string key = this._Store.GetTable(this._Name);
            return this._Host.OpenTable(key);
        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                Table t = this._Host.OpenTable(this._Store.GetTable(this._Name));
                return t.RecordCount;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            Table t = this._Host.OpenTable(this._Store.GetTable(this._Name));
            return t.IsIndexedBy(IndexColumns);
        }



    }

    public sealed class TableExpressionCTOR : TableExpression
    {

        private Schema _Columns;
        private string _Alias;
        private string _Name;
        private Key _Cluster;

        public TableExpressionCTOR(Host Host, TableExpression Parent, Schema Columns, string Alias, string Name, Key Cluster)
            :base(Host, Parent)
        {
            this._Columns = Columns;
            this._Alias = Alias;
            this._Name = Name;
            this._Cluster = Cluster;
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants;
        }

        // Evaluates //
        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {
            // do nothing
        }

        public override Table Select(FieldResolver Variants)
        {
            if (this._Cluster.Count == 0)
                return this._Host.CreateTable(this._Alias, this._Name, this._Columns);
            else
                return this._Host.CreateTable(this._Alias, this._Name, this._Columns, this._Cluster);
        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                return 0;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return Key.LeftSubsetStrong(this._Cluster, IndexColumns);
        }



    }

}
