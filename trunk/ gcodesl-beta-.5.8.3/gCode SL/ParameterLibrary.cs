using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gCodeSL
{
    public class IParameterLibrary
    {
        private Dictionary<string, IParameter> m_params = new Dictionary<string, IParameter>();
        private string m_name;

        /// <summary>
        /// Creates a library of scripting functions
        /// </summary>
        /// <param name="collectionName">Name of the library</param>
        public IParameterLibrary(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException();
            m_name = collectionName;
        }

        /// <summary>
        /// Adds support for the given parameter
        /// </summary>
        /// <param name="parameter">IParameter to add support for</param>
        public void AddSupportForIParameter(IParameter parameter)
        {
            if (m_params.ContainsValue(parameter))
                throw new Exception("FunctionLibrary Library '" + m_name + "' already supports the parameter '" + parameter.Type + "'");
            if (m_params.ContainsKey(parameter.Type))
                throw new Exception("FunctionLibrary Library '" + m_name + "' already has the parameter name '" + parameter.Type + "' assigned.");
            m_params.Add(parameter.Type.ToLower(), parameter);
        }

        /// <summary>
        /// Finds out if the FunctionLibrary supports the given parameter
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Returns true if the parameter is supported and false otherwise</returns>
        public bool SupportsIParameter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException();
            return m_params.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Gets the parameter of the given name from the IParameterLibrary
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Returns the parameter</returns>
        public IParameter GetIParameter(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException();
            if (!SupportsIParameter(name))
                throw new Exception("FunctionLibrary Library '" + m_name + "' doesn't support the parameter '" + name + "'");
            return m_params[name.ToLower()];
        }

        /// <summary>
        /// Gets the name of the IParameterLibrary
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the dictionary of parameter names and parameters
        /// </summary>
        public Dictionary<string, IParameter> Parameters
        {
            get { return m_params; }
        }
    }
}
