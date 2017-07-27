using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Select
{


    /// <summary>
    /// 
    /// </summary>
    public sealed class BasicSelectEngine : SelectEngine
    {

        public override void Render(Host Host, RecordWriter Output, Table Data, FieldResolver Variants, int RecordRef, ScalarExpressionCollection Fields, Filter Where, long Limit, SelectMetaData MetaData)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            long Ticker = 0;

            RecordReader rs = Data.OpenReader();

            while (rs.CanAdvance)
            {

                if (Ticker > Limit && Limit > 0)
                    break;

                MetaData.ReadCount++;

                Variants.SetValue(RecordRef, rs.ReadNext());

                if (Where.Evaluate(Variants))
                {

                    Output.Insert(Fields.Evaluate(Variants));

                    MetaData.WriteCount++;

                    Ticker++;

                }

            }

            sw.Stop();

            MetaData.RunTime = sw.Elapsed;

        }

    }


}
