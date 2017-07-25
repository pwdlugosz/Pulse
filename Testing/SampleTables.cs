﻿using System;
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

        public static HeapTable SampleHeapTable(Host Host, string Name, int Count, int Offset)
        {

            HeapTable t = Host.CreateTable("TEMP", Name, KeyValueColumns());
            RecordWriter w = t.OpenWriter();
            WriteKeyValue(w, Count, Offset);
            w.Close();
            return t;

        }

        public static ClusteredTable SampleClusteredTable(Host Host, string Name, int Count, int Offset)
        {

            ClusteredTable t = Host.CreateTable("TEMP", Name, KeyValueColumns(), new Key(0));
            RecordWriter w = t.OpenWriter();
            WriteKeyValue(w, Count, Offset);
            w.Close();
            return t;

        }

        private static void WriteKeyValue(RecordWriter Writer, int Count, int Offset)
        {

            RandomCell rc = new RandomCell(127);

            for (int i = 0; i < Count; i++)
            {
                RecordBuilder rb = new RecordBuilder();
                rb.Add(new Cell(i + Offset));
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
