using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Testing
{

    public static class SampleTables
    {

        public static Schema KeyValueColumns()
        {
            return new Schema("KEY INT, GAUSS_VAL NUM, GROUP10 INT, GROUP100 INT, GROUP1000 INT, TXT STRING.9");
        }

        public static HeapDreamTable SampleHeapDreamTable(Host Host, string Name, int Count)
        {

            HeapDreamTable t = Host.Dream(Name, KeyValueColumns());
            WriteStream w = t.OpenWriter();
            WriteKeyValue(w, Count);
            w.Close();
            return t;

        }

        public static HeapScribeTable SampleHeapScribeTable(Host Host, string Name, int Count)
        {

            HeapScribeTable t = Host.CreateTable("TEMP", Name, KeyValueColumns());
            WriteStream w = t.OpenWriter();
            WriteKeyValue(w, Count);
            w.Close();
            return t;

        }

        public static ClusteredDreamTable SampleClusteredDreamTable(Host Host, string Name, int Count)
        {

            ClusteredDreamTable t = Host.Dream(Name, KeyValueColumns(), new Key(0));
            WriteStream w = t.OpenWriter();
            WriteKeyValue(w, Count);
            w.Close();
            return t;

        }

        public static ClusteredScribeTable SampleClusteredScribeTable(Host Host, string Name, int Count)
        {

            ClusteredScribeTable t = Host.CreateTable("TEMP", Name, KeyValueColumns(), new Key(0));
            WriteStream w = t.OpenWriter();
            WriteKeyValue(w, Count);
            w.Close();
            return t;

        }

        private static void WriteKeyValue(WriteStream Writer, int Count)
        {

            RandomCell rc = new RandomCell(127);

            for (int i = 0; i < Count; i++)
            {
                RecordBuilder rb = new RecordBuilder();
                rb.Add(new Cell(i));
                rb.Add(rc.NextDoubleGauss());
                rb.Add(rc.NextLong(0, 10));
                rb.Add(rc.NextLong(0, 100));
                rb.Add(rc.NextLong(0, 1000));
                rb.Add(rc.NextStringUpperText(9));
                Writer.Insert(rb.ToRecord());
            }

        }

    }

}
