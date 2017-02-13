using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    public sealed class Filter 
    {

        public Filter(Expression Node)
        {
            this.InnerNode = Node;
        }

        public bool IsInverted
        {
            get;
            set;
        }

        public Expression InnerNode
        {
            get;
            set;
        }

        public Filter CloneOfMe()
        {
            return new Filter(this.InnerNode.CloneOfMe());
        }

        public bool Evaluate(FieldResolver Resolver)
        {
            bool b = this.InnerNode.Evaluate(Resolver).valueBOOL;
            return this.IsInverted ? !b : b;
        }

        public static Filter TrueForAll
        {
            get
            {
                return new Filter(new ExpressionConstant(null, Cell.TRUE));
            }
        }

        public static Filter FalseForAll
        {
            get
            {
                return new Filter(new ExpressionConstant(null, Cell.FALSE));
            }
        }

    }

}
