using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gCodeSL
{
    /// <summary>
    /// Creates a library of scripting functions
    /// </summary>
    public sealed class FunctionLibrary
    {
        private Dictionary<string, Method> m_functions = new Dictionary<string, Method>();
        private string m_name;

        /// <summary>
        /// Creates a library of scripting functions
        /// </summary>
        /// <param name="collectionName">Name of the library</param>
        public FunctionLibrary(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException();
            m_name = collectionName;
        }

        /// <summary>
        /// Adds support for the given method
        /// </summary>
        /// <param name="method">Method to add support for</param>
        public void AddSupportForMethod(Method method)
        {
            if (m_functions.ContainsValue(method))
                throw new Exception("FunctionLibrary Library '" + m_name + "' already supports the method '" + method.Name + "'");
            if (m_functions.ContainsKey(method.Name))
                throw new Exception("FunctionLibrary Library '" + m_name + "' already has the method name '" + method.Name + "' assigned.");
            m_functions.Add(method.Name.ToLower(), method);
        }

        /// <summary>
        /// Finds out if the FunctionLibrary supports the given method
        /// </summary>
        /// <param name="name">Name of the method</param>
        /// <returns>Returns true if the method is supported and false otherwise</returns>
        public bool SupportsMethod(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException();
            return m_functions.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Gets the method of the given name from the FunctionLibrary
        /// </summary>
        /// <param name="name">Name of the method</param>
        /// <returns>Returns the method</returns>
        public Method GetMethod(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException();
            if (!SupportsMethod(name))
                throw new Exception("FunctionLibrary Library '" + m_name + "' doesn't support the method '" + name + "'");
            return m_functions[name.ToLower()];
        }

        /// <summary>
        /// Gets the name of the FunctionLibrary
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the dictionary of method names and methods
        /// </summary>
        public Dictionary<string, Method> Methods
        {
            get { return m_functions; }
        }
    }
}
