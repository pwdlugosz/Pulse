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
using Pulse.Data.XTree;

namespace Pulse
{

    class Program
    {

        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();



            Host Enviro = new Host();

            Table x = Testing.SampleTables.SampleHeapTable(Enviro, "SortTest", 100000, 0);

            x.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Temp\Pre_Distinct.txt");

            TableUtil.Distinct(x, new Key(4, 5), new Key(4));

            x.Dump(@"C:\Users\pwdlu_000\Documents\Pulse_Projects\Temp\Post_Distinct.txt");

            //Scripting.ScriptProcessor sp = new Scripting.ScriptProcessor(Enviro);
            //string script = System.IO.File.ReadAllText(@"C:\Users\pwdlu_000\Documents\Pulse\Pulse\Scripting\TestScript.txt");
            //sp.RenderAction(script);

            //Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();


        }

        public static void TestJoins()
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            Host Enviro = new Host();

            Table x = Testing.SampleTables.SampleHeapTable(Enviro, "Test1", 100000, 0);

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
            DictionaryTable t = Enviro.CreateTable("TEMP", "TEST1", new Schema("KEY1 INT, KEY2 STRING.3"), new Schema("VALUE1 DOUBLE, VALUE2 INT"));

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
