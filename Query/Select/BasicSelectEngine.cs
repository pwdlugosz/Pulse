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

        public override void Render(Host Host, WriteStream Output, Table Data, ScalarExpressionCollection Fields, Filter Where, long Limit, SelectMetaData MetaData)
        {

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            long Ticker = 0;

            ReadStream rs = Data.OpenReader();

            FieldResolver fr = new FieldResolver(Host);
            fr.AddSchema("T", Data.Columns);

            while (rs.CanAdvance)
            {

                if (Ticker > Limit && Limit > 0)
                    break;

                MetaData.ReadCount++;

                fr.SetValue(0, rs.ReadNext());

                if (Where.Evaluate(fr))
                {

                    Output.Insert(Fields.Evaluate(fr));

                    MetaData.WriteCount++;

                    Ticker++;

                }

            }

            sw.Stop();

            MetaData.RunTime = sw.Elapsed;

        }

    }


}
