using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Scripting
{
    
    public static class ScriptingHelper
    {

        public static int GetTypeSize(PulseParser.TypeContext context)
        {
            if (context.LITERAL_INT() == null)
                return 8;
            return int.Parse(context.LITERAL_INT().GetText());
        }

        public static CellAffinity GetTypeAffinity(PulseParser.TypeContext context)
        {

            if (context.T_BOOL() != null)
                return CellAffinity.BOOL;
            else if (context.T_INT() != null)
                return CellAffinity.INT;
            else if (context.T_DATE() != null)
                return CellAffinity.DATE_TIME;
            else if (context.T_DOUBLE() != null)
                return CellAffinity.DOUBLE;
            else if (context.T_STRING() != null)
                return CellAffinity.STRING;
            else if (context.T_BLOB() != null)
                return CellAffinity.BLOB;

            throw new Exception(string.Format("Invalid type '{0}'", context.GetText()));

        }

        public static string GetLibName(PulseParser.Var_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetVarName(PulseParser.Var_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static ActionExpressions.Assignment GetAssignment(PulseParser.AssignmentContext context)
        {

            if (context.PLUS() != null)
                return ActionExpressions.Assignment.PlusEquals;
            else if (context.MINUS() != null)
                return ActionExpressions.Assignment.MinusEquals;
            else if (context.MUL() != null)
                return ActionExpressions.Assignment.ProductEquals;
            else if (context.DIV() != null)
                return ActionExpressions.Assignment.DivideEquals;
            else if (context.DIV2() != null)
                return ActionExpressions.Assignment.CheckDivideEquals;
            else if (context.MOD() != null)
                return ActionExpressions.Assignment.ModEquals;
            else
                return ActionExpressions.Assignment.Equals;

        }

    }

}
