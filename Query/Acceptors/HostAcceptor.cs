using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Query.Acceptors
{
    
    /// <summary>
    /// Represents an acceptor that writes data to the host's IO communicator
    /// </summary>
    public sealed class HostAcceptor : AcceptorStream
    {

        private Host _Host;
        private ExpressionCollection _Expressions;
        private long _Ticks = 0;

        public HostAcceptor(Host Host, ExpressionCollection Fields)
            : base()
        {
            this._Host = Host;
            this._Expressions = Fields;
        }

        public override void Accept(FieldResolver Variants)
        {
            this._Host.IO.WriteLine(this._Expressions.Evaluate(Variants).ToString());
        }

        public override void Close()
        {
            // Do nothing
        }

        public override long WriteCount()
        {
            return this._Ticks++;
        }

    }

}
