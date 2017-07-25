using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Libraries
{

    public class BaseLibrary : Library
    {

        public BaseLibrary(Host Host)
            : base("BASE", new ScalarExpressions.ScalarExpressionFunction.BaseLibrary(Host), null)
        {
        }

    }

}
