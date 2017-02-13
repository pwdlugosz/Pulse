using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse
{

    class Program
    {

        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();


            Host Enviro = new Host();
            FieldResolver variables = new FieldResolver(Enviro);

            Expression A = Expression.Value(0);
            Expression B = Expression.Value(1);
            Expression C = Expression.NEQ(A, B);

            Console.WriteLine("A : {0}", A.Evaluate(variables));
            Console.WriteLine("B : {0}", B.Evaluate(variables));
            Console.WriteLine("C : {0}", C.Evaluate(variables));

            //Schema s = new Schema("Key int, Value double, xyz int");
            //HeapDreamTable x = Enviro.Dream("Test", s);
            //HeapScribeTable x = Enviro.CreateTable("TEMP", "Test", s);
            //x.CreateIndex("IDX1", new Key(0));
            //Index y = Index.CreateExternalIndex(x, new Key(0));
            //RandomCell rng = new RandomCell(127);

            //WriteStream writer = x.OpenWriter();
            //for (int i = 0; i < 200000; i++)
            //{

            //    Record r = Record.Stitch(rng.NextLong(0, 100), rng.NextDoubleGauss(), new Cell(i));
            //    x.Insert(r);
            //    y.Insert(r, x.LastInsertKey);

            //}
            //writer.Close();
            //Console.WriteLine("Table Build : {0}", sw.Elapsed);

            //Index idx = x.GetIndex("IDX1");

            //idx.Tree.Dump(Host.TestDir + "NC_Index_Dump.txt", true);
            //Console.WriteLine("Index Dump : {0}", sw.Elapsed);

            //Table.Dump(Host.TestDir + "Test_Data_Dump_NCIndex_WHERE.txt", idx.OpenReader(Record.Stitch(new Cell(0))));
            //Console.WriteLine("Index Dump - WHERE: {0}", sw.Elapsed);

            //ReadStream stream = idx.OpenReader();
            //Table.Dump(Host.TestDir + "Test_Data_Dump_NCIndex.txt", stream);
            //Console.WriteLine("Table Dump - Index: {0}", sw.Elapsed);

            //stream = x.OpenReader();
            //Table.Dump(Host.TestDir + "Test_Data_Dump_Heap.txt", stream);
            //Console.WriteLine("Table Dump - Heap: {0}", sw.Elapsed);

            //stream = y.OpenReader();
            //Table.Dump(Host.TestDir + "Test_External_Dump_Heap_NCIndex.txt", stream);
            //Console.WriteLine("Table Dump - External NC Meta");

            //stream = y.OpenReader(Record.Stitch(new Cell(2)), Record.Stitch(new Cell(4)));
            //Table.Dump(Host.TestDir + "Test_External_Dump_Heap_NCIndex_WHERE.txt", stream);
            //Console.WriteLine("Table Dump - External NC Meta");

            //Console.WriteLine(x.MetaData());

            Enviro.ShutDown();

            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();


        }

    }

}
