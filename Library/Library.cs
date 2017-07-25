using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.ScalarExpressions;
using Pulse.ActionExpressions;
using Pulse.Data;

namespace Pulse.Libraries
{

    public class Library
    {

        private IScalarExpressionLookup _Functions;
        private IActionExpressionLookup _Actions;
        private Heap<Cell> _Values;
        private Heap<CellMatrix> _Matrixes;
        private string _Name;

        public Library(string Name, IScalarExpressionLookup Functions, IActionExpressionLookup Actions)
        {
            this._Functions = Functions;
            this._Actions = Actions;
            this._Values = new Heap<Cell>();
            this._Matrixes = new Heap<CellMatrix>();
            this._Name = Name;
        }

        /// <summary>
        /// Represents a collection of values
        /// </summary>
        public Heap<Cell> Values
        {
            get { return this._Values; }
        }

        /// <summary>
        /// Represents a collection of matrixes
        /// </summary>
        public Heap<CellMatrix> Matrixes
        {
            get { return this._Matrixes; }
        }

        /// <summary>
        /// The name of the library
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// Inner function set
        /// </summary>
        public IScalarExpressionLookup FunctionSet
        {
            get { return this._Functions; }
        }

        /// <summary>
        /// Inner action set
        /// </summary>
        public IActionExpressionLookup ActionSet
        {
            get { return this._Actions; }
        }

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool FunctionExists(string Name)
        {
            return this._Functions.Exists(Name);
        }

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ScalarExpressionFunction LookupFunction(string Name)
        {
            return this._Functions.Lookup(Name);
        }

        /// <summary>
        /// Checks if an action exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool ActionExists(string Name)
        {
            return this._Actions.Exists(Name);
        }

        /// <summary>
        /// Gets an action
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public ActionExpression LookupAction(string Name)
        {
            return this._Actions.Lookup(Name);
        }

    }

}
