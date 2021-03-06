﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;

namespace Pulse.Scripting
{
    
    public static class ScriptingHelper
    {

        public static int GetTypeSize(PulseParser.TypeContext context)
        {
            if (context.LITERAL_INT() == null)
            {
                CellAffinity a = GetTypeAffinity(context);
                return CellSerializer.DefaultLength(a);
            }
            return int.Parse(context.LITERAL_INT().GetText());
        }

        public static CellAffinity GetTypeAffinity(PulseParser.TypeContext context)
        {

            if (context.T_BOOL() != null)
                return CellAffinity.BOOL;

            else if (context.T_DATE() != null)
                return CellAffinity.DATE_TIME;

            else if (context.T_BYTE() != null)
                return CellAffinity.BYTE;

            else if (context.T_SHORT() != null)
                return CellAffinity.SHORT;

            else if (context.T_INT() != null)
                return CellAffinity.INT;

            else if (context.T_LONG() != null)
                return CellAffinity.LONG;

            else if (context.T_SINGLE() != null)
                return CellAffinity.SINGLE;

            else if (context.T_DOUBLE() != null)
                return CellAffinity.DOUBLE;

            else if (context.T_BINARY() != null)
                return CellAffinity.BINARY;

            else if (context.T_BSTRING() != null)
                return CellAffinity.BSTRING;

            else if (context.T_CSTRING() != null)
                return CellAffinity.CSTRING;

            throw new Exception(string.Format("Invalid type '{0}'", context.GetText()));

        }

        public static string GetLibName(PulseParser.Scalar_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(PulseParser.Matrix_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(PulseParser.Record_nameContext context)
        {
            if (context.lib_name() == null) return Host.GLOBAL;
            return context.lib_name().GetText();
        }

        public static string GetLibName(PulseParser.Table_nameContext context)
        {
            if (context.lib_name() == null) return Host.TEMP;
            return context.lib_name().GetText();
        }

        public static string GetVarName(PulseParser.Scalar_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(PulseParser.Matrix_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(PulseParser.Record_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static string GetVarName(PulseParser.Table_nameContext context)
        {
            return context.IDENTIFIER().GetText();
        }

        public static Expressions.ActionExpressions.Assignment GetAssignment(PulseParser.AssignmentContext context)
        {

            if (context.PLUS() != null)
                return Expressions.ActionExpressions.Assignment.PlusEquals;
            else if (context.MINUS() != null)
                return Expressions.ActionExpressions.Assignment.MinusEquals;
            else if (context.MUL() != null)
                return Expressions.ActionExpressions.Assignment.ProductEquals;
            else if (context.DIV() != null)
                return Expressions.ActionExpressions.Assignment.DivideEquals;
            else if (context.DIV2() != null)
                return Expressions.ActionExpressions.Assignment.CheckDivideEquals;
            else if (context.MOD() != null)
                return Expressions.ActionExpressions.Assignment.ModEquals;
            else
                return Expressions.ActionExpressions.Assignment.Equals;

        }

    }

}
