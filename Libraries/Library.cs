using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.ActionExpressions;
using Pulse.Expressions;
using Pulse.Tables;
using Pulse.Elements;
using Pulse.Elements.Structures;

namespace Pulse.Libraries
{

    /// <summary>
    /// Represents a base class for all libraries
    /// </summary>
    public abstract class Library 
    {

        protected Host _Host;
        protected string _Name;

        public Library(Host Host, string Name)
        {
            this._Host = Host;
            this._Name = Name;
        }

        /// <summary>
        /// The name of the library
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// The library's host
        /// </summary>
        public Host Host
        {
            get { return this._Host; }
        }

        /// <summary>
        /// Shuts down the library
        /// </summary>
        public virtual void ShutDown()
        {
            // do something
        }

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ScalarFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ScalarExpressionFunction ScalarFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool MatrixFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract MatrixExpressionFunction MatrixFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool RecordFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract RecordExpressionFunction RecordFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool TableFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract TableExpressionFunction TableFunctionLookup(string Name);

        /// <summary>
        /// Checks if an action exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ActionExists(string Name);

        /// <summary>
        /// Gets an action
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ActionExpressionParameterized ActionLookup(string Name);

        /// <summary>
        /// Represents the base library
        /// </summary>
        //public sealed class BaseLibrary : Library
        //{

        //    private ScalarExpressionFunction.BaseLibraryFunctions _x;

        //    public BaseLibrary(Host Host)
        //        :base(Host, Host.GLOBAL)
        //    {
        //        this._x = new ScalarExpressionFunction.BaseLibraryFunctions(Host);
        //    }

        //    public override bool ActionExists(string Name)
        //    {
        //        return false;
        //    }

        //    public override ActionExpressionParameterized ActionLookup(string Name)
        //    {
        //        return null;
        //    }

        //    public override bool ScalarFunctionExists(string Name)
        //    {
        //        return this._x.Exists(Name);
        //    }

        //    public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        //    {
        //        return this._x.Lookup(Name);
        //    }

        //}

    }


}
