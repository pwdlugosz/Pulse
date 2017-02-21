using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// A basic read stream
    /// </summary>
    public class VanillaReadStream : ReadStream
    {

        protected Table _Storage;
        protected Page _CurrentPage;
        protected int _CurrentPageID = -1;
        protected int _CurrentRecordIndex = -1;
        protected RecordKey _Lower;
        protected RecordKey _Upper;
        protected int _Ticks = 0;

        public VanillaReadStream(Table Data, RecordKey LKey, RecordKey UKey)
            : base()
        {

            this._Lower = LKey;
            this._Upper = UKey;
            this._Storage = Data;
            if (Data.PageCount == 0)
            {
                this._CurrentPage = null;
                this._CurrentPageID = -1;
                this._CurrentRecordIndex = -1;
            }
            else
            {
                this._CurrentPage = this._Storage.GetPage(this._Lower.PAGE_ID);
                this._CurrentPageID = this._CurrentPage.PageID;
                this._CurrentRecordIndex = this._Lower.ROW_ID;
            }

        }

        public VanillaReadStream(Table Data, IElementHeader Header)
            : this(Data, VanillaReadStream.OriginKey(Data, Header), VanillaReadStream.TerminalKey(Data, Header))
        {
        }

        public VanillaReadStream(Table Data)
            : this(Data, VanillaReadStream.OriginKey(Data, Data.Header), VanillaReadStream.TerminalKey(Data, Data.Header))
        {
        }

        public override bool CanAdvance
        {

            get
            {

                if (this._CurrentPage == null)
                    return false;
                else if (this._CurrentPageID == -1)
                    return false;
                return !(this._CurrentPageID == this._Upper.PAGE_ID && this._CurrentRecordIndex > this._Upper.ROW_ID);

            }

        }

        public override bool CanRevert
        {
            get
            {
                if (this._CurrentPage == null)
                    return false;
                else if (this._CurrentPageID == -1)
                    return false;
                return !(this._CurrentPageID == this._Lower.PAGE_ID && this._CurrentRecordIndex < this._Lower.ROW_ID);
            }
        }

        public override Schema Columns
        {
            get { return this._Storage.Columns; }
        }

        public override void Advance()
        {

            this._CurrentRecordIndex++;
            if (this._CurrentRecordIndex >= this._CurrentPage.Count)
            {
                this._CurrentRecordIndex = 0;
                this._CurrentPageID = this._CurrentPage.NextPageID;

                if (this._CurrentPageID != -1)
                    this._CurrentPage = this._Storage.GetPage(this._CurrentPageID);

            }

            this._Ticks++;

        }

        public override void Advance(int Itterations)
        {
            for (int i = 0; i < Itterations; i++)
                this.Advance();
        }

        public override void Revert()
        {

            this._CurrentRecordIndex--;
            if (this._CurrentRecordIndex < 0)
            {

                this._CurrentPageID = this._CurrentPage.LastPageID;
                if (this._CurrentPageID != -1)
                {
                    this._CurrentPage = this._Storage.GetPage(this._CurrentPageID);
                    this._CurrentRecordIndex = this._CurrentPage.Count - 1;
                }

            }

            this._Ticks--;

        }

        public override void Revert(int Itterations)
        {
            for (int i = 0; i < Itterations; i++)
                this.Revert();
        }

        public override Record Read()
        {
            return this._CurrentPage.Select(this._CurrentRecordIndex);
        }

        public override Record ReadNext()
        {
            Record r = this.Read();
            this.Advance();
            return r;
        }

        public override int PageID()
        {
            return this._CurrentPage.PageID;
        }

        public override int RecordID()
        {
            return this._CurrentRecordIndex;
        }

        public override long Position()
        {
            return this._Ticks;
        }

        public static RecordKey OriginKey(Table Parent, IElementHeader Header)
        {
            if (Header.OriginPageID == -1)
                return RecordKey.RecordNotFound;
            return new RecordKey(Header.OriginPageID, 0);
        }

        public static RecordKey TerminalKey(Table Parent, IElementHeader Header)
        {
            if (Header.TerminalPageID == -1)
                return RecordKey.RecordNotFound;
            return new RecordKey(Header.TerminalPageID, Parent.GetPage(Header.TerminalPageID).Count - 1);
        }

    }


}
