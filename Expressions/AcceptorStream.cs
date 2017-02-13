using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Expressions
{

    /// <summary>
    /// Acceptors are similar to 
    /// </summary>
    public abstract class AcceptorStream
    {

        public AcceptorStream()
        {
        }

        public abstract ExpressionCollection OutputValues;

        public abstract void Accept(FieldResolver Variants);

        public abstract long WriteCount();

        public abstract void Close();

    }

}

