using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLanguage
{
    /// <summary>
    /// Defines a generic class for methods in ScriptingLanguage
    /// </summary>
    public abstract class Method
    {
        protected string m_name;
        protected List<IParameter> m_param;
        protected List<IParameter> m_paramOpt;

        /// <summary>
        /// Creates a new method
        /// </summary>
        public Method()
        {
            m_param = new List<IParameter>();
            m_paramOpt = new List<IParameter>();
        }

        /// <summary>
        /// Name of the method
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Calls the specified method
        /// </summary>
        /// <param name="param">Parameters of the method</param>
        /// <returns>Returns required value, if any, and null otherwise</returns>
        public virtual IParameter Call(params IParameter[] param)
        {

            if (param.Length < m_param.Count)
                throw new Exception(m_name + " must be called with at least " + m_param.Count.ToString() + " parameters.");
            if (param.Length > m_param.Count + m_paramOpt.Count)
                throw new Exception(m_name + " must be called with no more than " + (m_param.Count + m_paramOpt.Count).ToString() + " parameters.");
            if (param.Length == 0)
                return null;

            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] == null)
                    throw new ArgumentException();

                if (i >= m_param.Count)
                {
                    if (param[i].Type != m_paramOpt[i - m_param.Count].Type)
                        throw new Exception("Argument " + i.ToString() + " of method call must be a " + param[i].Type);
                }
                else if (param[i].Type != m_param[i].Type)
                    throw new Exception("Argument " + i.ToString() + " of method call must be a " + param[i].Type);
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all required parameters
        /// </summary>
        public List<IParameter> RequiredParameters
        {
            get { return m_param; }
        }

        /// <summary>
        /// Gets a list of all optional parameters
        /// </summary>
        public List<IParameter> OptionalParameters
        {
            get { return m_param; }
        }
    }
}
