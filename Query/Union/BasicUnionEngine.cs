using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Union
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class BasicUnionEngine : UnionEngine
    {

        public override void Render(Host Host, WriteStream Output, IEnumerable<Table> Tables, UnionMetaData MetaData)
        {

            foreach (Table t in Tables)
            {

                ReadStream rs = t.OpenReader();
                Output.Consume(rs);

                MetaData.ReadCount += t.RecordCount;
                MetaData.WriteCount += t.RecordCount;
                MetaData.TableCount++;

            }

        }

    }

}
