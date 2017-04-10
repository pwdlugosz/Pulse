using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Aggregates;
using Pulse.Query;
using Pulse.Query.Join;
using Pulse.ScalarExpressions;

namespace Pulse
{

    class Program
    {

        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();
            ClusteredScribeTable x1 = Pulse.Testing.SampleTables.SampleClusteredScribeTable(Enviro, "Test1", 10000, 0);
            Query.Aggregate.DictionaryAggregateEngine engine = new Query.Aggregate.DictionaryAggregateEngine();
            ScalarExpression a = ScalarExpression.Field(x1, "GROUP10", 0);


            //Table v = engine.InnerJoin("TEMP", "Q1", x1, x2, new RecordMatcher(new Key(0)), exp);
            //v.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\NL_InnerJoin_Test1.txt");

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

            Table v = engine.InnerJoin(Enviro, "TEMP", "Q1", x1, x2, new RecordMatcher(new Key(0)), exp);
            v.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Test\NL_InnerJoin_Test1.txt");

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
