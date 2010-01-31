using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage
{
    /// <summary>
    /// Defines a generic parameter for ScriptingLanguage
    /// </summary>
    public class IParameter
    {
        protected object m_data;

        /// <summary>
        /// Creates a new generic parameter for ScriptingLanguage
        /// </summary>
        /// <param name="data">Data of the parameter</param>
        public IParameter(object data)
        {
            m_data = data;
        }

        /// <summary>
        /// Gets or sets the data of the Parameter
        /// </summary>
        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        /// <summary>
        /// Gets the type of the Parameter
        /// </summary>
        public virtual string Type
        {
            get { throw new NotSupportedException("This should never be called on IParameter()"); }
        }

        /// <summary>
        /// Creates a new parameter that contains the specified data
        /// </summary>
        /// <param name="data">Data of the new parameter</param>
        /// <returns>Returns new parameter</returns>
        public virtual IParameter Create(object data)
        {
            throw new NotSupportedException();
        }
    }

    #region GenericParameters
    /// <summary>
    /// Creates a new string Parameter for ScriptingLanguage
    /// </summary>
    public sealed class pString : IParameter
    {
        /// <summary>
        /// Creates a new string Parameter for ScriptingLanguage
        /// </summary>
        /// <param name="data">Data of the parameter</param>
        public pString(string data) : base(data) { }

        /// <summary>
        /// Gets the type of the parameter
        /// </summary>
        public override string Type
        {
            get { return "String"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter
        /// </summary>
        /// <param name="data">Data of the instance</param>
        /// <returns>Returns new parameter</returns>
        public override IParameter Create(object data)
        {
            return new pString((string)data);
        }
    }

    /// <summary>
    /// Creates a new int Parameter for ScriptingLanguage
    /// </summary>
    public sealed class pInt : IParameter
    {
        /// <summary>
        /// Creates a new int Parameter for ScriptingLanguage
        /// </summary>
        /// <param name="data">Data of the parameter</param>
        public pInt(int data) : base(data) { }

        /// <summary>
        /// Gets the type of the parameter
        /// </summary>
        public override string Type
        {
            get { return "Int"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter
        /// </summary>
        /// <param name="data">Data of the instance</param>
        /// <returns>Returns new parameter</returns>
        public override IParameter Create(object data)
        {
            return new pInt((int)data);
        }
    }

    /// <summary>
    /// Creates a new int Parameter for ScriptingLanguage
    /// </summary>
    public sealed class pDouble : IParameter
    {
        /// <summary>
        /// Creates a new int Parameter for ScriptingLanguage
        /// </summary>
        /// <param name="data">Data of the parameter</param>
        public pDouble(double data) : base(data) { }

        /// <summary>
        /// Gets the type of the parameter
        /// </summary>
        public override string Type
        {
            get { return "Double"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter
        /// </summary>
        /// <param name="data">Data of the instance</param>
        /// <returns>Returns new parameter</returns>
        public override IParameter Create(object data)
        {
            return new pDouble((double)data);
        }
    }

    /// <summary>
    /// Creates a new Bool Parameter for ScriptingLanguage
    /// </summary>
    public sealed class pBool : IParameter
    {
        /// <summary>
        /// Creates a new Bool Parameter for ScriptingLanguage
        /// </summary>
        /// <param name="data">Data of the parameter</param>
        public pBool(bool data) : base(data) { }

        /// <summary>
        /// Gets the type of the parameter
        /// </summary>
        public override string Type
        {
            get { return "Bool"; }
        }

        /// <summary>
        /// Creates a new instance of the parameter
        /// </summary>
        /// <param name="data">Data of the instance</param>
        /// <returns>Returns new parameter</returns>
        public override IParameter Create(object data)
        {
            return new pBool((bool)data);
        }
    }
    #endregion
}
