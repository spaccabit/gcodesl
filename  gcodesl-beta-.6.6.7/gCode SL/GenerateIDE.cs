using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace gCodeSL
{
    /// <summary>
    /// XML Interaction with a scripting language 
    /// </summary>
    public static class XMLInteract
    {
        /// <summary>
        /// Genereates a XML file for the IDE to use
        /// </summary>
        /// <param name="language">Language used to populate file</param>
        /// <param name="filepath">Path of the XML file</param>
        public static void GenerateIDE(ScriptingLanguage language, string filepath)
        {
            if (language == null)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException();
            if (File.Exists(filepath))
                File.Delete(filepath);

            var fLib = language.FunctionLibrary.Methods;
            var pLib = language.ParameterLibrary.Parameters;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            var writer = XmlWriter.Create(filepath, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("IDE");

            writer.WriteStartElement("name");
            writer.WriteString(language.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("linepreceders");
            writer.WriteAttributeString("amount", "2");
            writer.WriteStartElement("preceder");
            writer.WriteString(ScriptingLanguage.ValidPreceder.Declare.ToString().ToLower());
            writer.WriteEndElement();
            writer.WriteStartElement("preceder");
            writer.WriteString(ScriptingLanguage.ValidPreceder.Execute.ToString().ToLower());
            writer.WriteEndElement();
            writer.WriteStartElement("preceder");
            writer.WriteString(ScriptingLanguage.ValidPreceder.While.ToString().ToLower());
            writer.WriteEndElement();
            writer.WriteStartElement("preceder");
            writer.WriteString(ScriptingLanguage.ValidPreceder.Break.ToString().ToLower());
            writer.WriteEndElement();

            writer.WriteStartElement("variables");
            writer.WriteAttributeString("amount", pLib.Count.ToString());
            foreach (var parameter in pLib)
            {
                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", parameter.Value.Type);
                writer.WriteAttributeString("description", parameter.Value.Description);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("functions");
            writer.WriteAttributeString("amount", fLib.Count.ToString());
            foreach (var method in fLib)
            {
                writer.WriteStartElement("method");
                writer.WriteAttributeString("name", method.Value.Name);
                writer.WriteAttributeString("description", method.Value.Description);
                foreach (var param in method.Value.RequiredParameters)
                {
                    writer.WriteStartElement("req-param");
                    writer.WriteString(param.Type);
                    writer.WriteEndElement();
                }
                foreach (var param in method.Value.OptionalParameters)
                {
                    writer.WriteStartElement("opt-param");
                    writer.WriteString(param.Type);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Gets the name of the given language
        /// </summary>
        /// <param name="filepath">Path of the XML IDE file</param>
        /// <returns>Returns the name of the language</returns>
        public static string NameOfScriptingLanguage(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException();
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            string toRet = null;
            XmlReader reader = XmlReader.Create(filepath);
            while (toRet == null && reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "name")
                    toRet = reader.ReadString();
            }
            reader.Close();

            return toRet;
        }

        /// <summary>
        /// Gets a list of all supported parameters for the given language
        /// XML IDE file
        /// </summary>
        /// <param name="filepath">Path of the XML IDE file</param>
        /// <returns>Returns all supported parameters and descriptions</returns>
        public static List<IDEParameter> SupportedParameters(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException();
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            List<IDEParameter> toRet = new List<IDEParameter>();
            XmlReader reader = XmlReader.Create(filepath);
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "parameter")
                    toRet.Add(new IDEParameter(reader.GetAttribute("name"), reader.GetAttribute("description")));
            }

            reader.Close();

            return toRet;
        }

        /// <summary>
        /// Gets a list of all supported methods for the given language
        /// XML IDE file
        /// </summary>
        /// <param name="filepath">Path of the XML IDE file</param>
        /// <returns>Returns all supported methods and descriptions</returns>
        public static List<IDEMethod> SupportedMethods(string filepath, List<IDEParameter> supportedParameters)
        {
            if (string.IsNullOrEmpty(filepath) || supportedParameters == null || supportedParameters.Contains(null))
                throw new ArgumentNullException();
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            List<IDEMethod> toRet = new List<IDEMethod>();
            Dictionary<string, IDEParameter> index = new Dictionary<string, IDEParameter>();
            foreach (var param in supportedParameters)
                index.Add(param.Name, param);

            XmlReader reader = XmlReader.Create(filepath);
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "method")
                {
                    if (!reader.HasAttributes)
                        throw new Exception("Methods must have attributes 'name' and 'description'");
                    string name = reader.GetAttribute("name", null), desc = reader.GetAttribute("description", null);
                    if (name == null || desc == null)
                        throw new Exception("Methods must have attributes 'name' and 'description'");

                    List<IDEParameter> reqParam = new List<IDEParameter>(), optParam = new List<IDEParameter>();

                    reader.Read();
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "req-param")
                    {
                        string pName = reader.ReadString();
                        if (!index.ContainsKey(pName))
                            throw new Exception("Method " + name + " cannot require a unsupported parameter " + pName);
                        reqParam.Add(index[pName]);
                        reader.Read();
                    }
                    while ( reader.MoveToContent() == XmlNodeType.Element && reader.Name == "opt-param")
                    {
                        string pName = reader.ReadString();
                        if (!index.ContainsKey(pName))
                            throw new Exception("Method " + name + " cannot allow a unsupported parameter " + pName);
                        optParam.Add(index[pName]);
                        reader.Read();
                    }

                    toRet.Add(new IDEMethod(name, reqParam, optParam, desc));
                }
            }

            reader.Close();

            return toRet;
        }
        private static IDEParameter IDEParamsContains(List<IDEParameter> param, string name)
        {
            name = name.ToLower();

            foreach (var val in param)
            {
                if (val.Name.ToLower() == name)
                    return val;
            }
            return null;
        }

        /// <summary>
        /// Gets a array of all supported line preceders for the given
        /// language XML IDE file
        /// </summary>
        /// <param name="filepath">Path of the XML IDE file</param>
        /// <returns>Returns all supported line preceders</returns>
        public static string[] SupportedPreceders(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException();
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            List<string> toRet = new List<string>();
            XmlReader reader = XmlReader.Create(filepath);
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "preceder")
                    toRet.Add(reader.ReadString());
            }

            reader.Close();

            return toRet.ToArray();
        }
    }

    /// <summary>
    /// Creates a Parameter with IDE relevant information
    /// </summary>
    public sealed class IDEParameter
    {
        string m_name, m_desc;

        /// <summary>
        /// Creates a Parameter with IDE relevant information
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="description">Description of the parameter</param>
        public IDEParameter(string name, string description)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                throw new ArgumentNullException();
            m_name = name;
            m_desc = description;
        }

        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the parameter
        /// </summary>
        public string Description
        {
            get { return m_desc; }
        }
    }

    /// <summary>
    /// Creates a Method with IDE relevant information
    /// </summary>
    public sealed class IDEMethod
    {
        List<IDEParameter> m_param = new List<IDEParameter>(), m_paramOpt = new List<IDEParameter>();
        string m_desc, m_name;

        /// <summary>
        /// Creates a Method with IDE relevant information
        /// </summary>
        /// <param name="name">Name of the method</param>
        /// <param name="param">Required parameters of the method</param>
        /// <param name="optionalParam">Optional parameters of the method</param>
        /// <param name="description">Description of the method</param>
        public IDEMethod(string name, List<IDEParameter> param, List<IDEParameter> optionalParam, string description)
        {
            if (param == null || optionalParam == null || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(name))
                throw new ArgumentNullException();

            foreach (var p in param)
            {
                if (p == null)
                    throw new ArgumentNullException();
            }
            foreach (var p in optionalParam)
            {
                if (p == null)
                    throw new ArgumentNullException();
            }

            m_name = name;
            m_param = param;
            m_paramOpt = optionalParam;
            m_desc = description;
        }

        /// <summary>
        /// Gets the name of the method
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets the description of the method
        /// </summary>
        public string Description
        {
            get { return m_desc; }
        }

        /// <summary>
        /// Gets the required parameters of the method
        /// </summary>
        public List<IDEParameter> RequiredParameters
        {
            get { return m_param; }
        }

        /// <summary>
        /// Gets the optional parameters of the method
        /// </summary>
        public List<IDEParameter> OptionalParameters
        {
            get { return m_paramOpt; }
        }
    }
}
