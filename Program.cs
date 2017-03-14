using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Aggregates;
using Pulse.Query;
using Pulse.Query.Join;
using Pulse.Query.Beacons;
using Pulse.Query.Acceptors;
using Pulse.ScalarExpressions;

namespace Pulse
{

    class Program
    {

        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            HeapDreamTable x1 = Pulse.Testing.SampleTables.SampleHeapDreamTable(Enviro, "T1", 10, 0);
            


            Table v = engine.InnerJoin("TEMP", "Q1", x1, x2, new RecordMatcher(new Key(0)), exp);
            v.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\NL_InnerJoin_Test1.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();


        }

        public static void TestJoins()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            HeapDreamTable x1 = Pulse.Testing.SampleTables.SampleHeapDreamTable(Enviro, "T1", 10, 0);
            ClusteredDreamTable x2 = Pulse.Testing.SampleTables.SampleClusteredDreamTable(Enviro, "T2", 10, 5);
            HeapDreamTable x3 = Pulse.Testing.SampleTables.SampleHeapDreamTable(Enviro, "T3", 10, 0);
            ScalarExpressionCollection exp = new ScalarExpressionCollection();
            exp.Add("LEFT", ScalarExpression.Field(x1, "KEY", 0));
            exp.Add("RIGHT", ScalarExpression.Field(x1, "KEY", 1));

            NestedLoopJoinEngine engine = new NestedLoopJoinEngine();

            Table v = engine.InnerJoin("TEMP", "Q1", x1, x2, new RecordMatcher(new Key(0)), exp);
            v.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\NL_InnerJoin_Test1.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();

        }

        public static void TestDictionaryAggregate()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            Schema s = new Schema("Key int, Value double, xyz int");
            //HeapDreamTable x = Enviro.Dream("Test", s);
            HeapScribeTable x = Enviro.CreateTable("TEMP", "Test", s);
            //x.CreateIndex("IDX1", new Key(0));
            RandomCell rng = new RandomCell(127);

            WriteStream writer = x.OpenWriter();
            for (int i = 0; i < 20000; i++)
            {

                Record r = Record.Stitch(rng.NextLong(0, 100), rng.NextDoubleGauss(), new Cell(i));
                x.Insert(r);

            }
            writer.Close();

            //Index idx = x.GetIndex("IDX1");

            ScalarExpressionCollection keys = new ScalarExpressionCollection();
            keys.Add("KEY", ScalarExpression.Field(x, "Key", 0));

            AggregateLookup l = new AggregateLookup();
            AggregateCollection aggs = new Aggregates.AggregateCollection();
            aggs.Add("SUM", l.Lookup("SUM", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("MIN", l.Lookup("MIN", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("MAX", l.Lookup("MAX", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("COUNT", l.Lookup("COUNT", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));

            HeapDreamTable hdt = Enviro.Dream("T", new Schema("KEY INT, SUM DOUBLE, MIN DOUBLE, MAX DOUBLE, COUNT INT"));
            WriteStream wstream = hdt.OpenWriter();
            //OrderedAggregateStream oas = new OrderedAggregateStream(wstream, keys, aggs);
            DictionaryAggregateStream oas = new DictionaryAggregateStream(wstream, keys, aggs);
            VanilaBeaconStream vbs = new VanilaBeaconStream(Enviro, x.OpenReader(), "T");

            oas.Consume(vbs);
            oas.Close();
            wstream.Close();

            hdt.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\Agg_Test2.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();

        }

        public static void TestOrderedAggregatge()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            Schema s = new Schema("Key int, Value double, xyz int");
            //HeapDreamTable x = Enviro.Dream("Test", s);
            HeapScribeTable x = Enviro.CreateTable("TEMP", "Test", s);
            x.CreateIndex("IDX1", new Key(0));
            Index y = Index.CreateExternalIndex(x, new Key(0));
            RandomCell rng = new RandomCell(127);

            WriteStream writer = x.OpenWriter();
            for (int i = 0; i < 20000; i++)
            {

                Record r = Record.Stitch(rng.NextLong(0, 100), rng.NextDoubleGauss(), new Cell(i));
                x.Insert(r);
                y.Insert(r, x.LastInsertKey);

            }
            writer.Close();

            Index idx = x.GetIndex("IDX1");

            ScalarExpressionCollection keys = new ScalarExpressionCollection();
            keys.Add("KEY", ScalarExpression.Field(x, "Key", 0));

            AggregateLookup l = new AggregateLookup();
            AggregateCollection aggs = new Aggregates.AggregateCollection();
            aggs.Add("SUM", l.Lookup("SUM", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("MIN", l.Lookup("MIN", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("MAX", l.Lookup("MAX", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));
            aggs.Add("COUNT", l.Lookup("COUNT", new List<ScalarExpression>() { ScalarExpression.Field(x, "Value", 0) }, Filter.TrueForAll));

            HeapDreamTable hdt = Enviro.Dream("T", new Schema("KEY INT, SUM DOUBLE, MIN DOUBLE, MAX DOUBLE, COUNT INT"));
            WriteStream wstream = hdt.OpenWriter();
            OrderedAggregateStream oas = new OrderedAggregateStream(wstream, keys, aggs);
            VanilaBeaconStream vbs = new VanilaBeaconStream(Enviro, idx.OpenReader(), "T");

            oas.Consume(vbs);
            oas.Close();
            wstream.Close();

            hdt.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\Agg_Test.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();

        }

        public static void TestDictionary()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            Schema s = new Schema("Key int, Value double, xyz int");
            DictionaryScribeTable t = Enviro.CreateTable("TEMP", "TEST1", new Schema("KEY1 INT, KEY2 STRING.3"), new Schema("VALUE1 DOUBLE, VALUE2 INT"));

            for (int i = 0; i < 10000; i++)
            {

                Record k = Record.Stitch(new Cell(i), Enviro.BaseRNG.NextString(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
                Record v = Record.Stitch(Enviro.BaseRNG.NextDoubleGauss(), Enviro.BaseRNG.NextLong());
                t.Add(k, v);

            }

            t.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\DreamDictionartText.txt");

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string fibux = Console.ReadLine();

        }

    }

}
