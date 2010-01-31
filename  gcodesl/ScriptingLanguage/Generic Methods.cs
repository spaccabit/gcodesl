using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using gInclude;
using gInclude.Utility;

namespace ScriptingLanguage
{
    /// <summary>
    /// A generic method for randomizing integers
    /// </summary>
    public sealed class mGeneric_RandomInt : Method
    {
        /// <summary>
        /// A generic method for randomizing integers
        /// </summary>
        public mGeneric_RandomInt() : base()
        {
            m_name = "Generic.Random";

            m_param.Add(new pInt(0));
        }

        /// <summary>
        /// Generic.Random(int max)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 parameter and 0 optional parameters.
        /// [0] = pInt //maximum number to randomize</param>
        /// <returns>Returns a pInt that contains the number randomized</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pInt((int)Randomization.TrueRandom((int)param[0].Data));
        }
    }

    /// <summary>
    /// A generic method for randomizing doubles
    /// </summary>
    public sealed class mGeneric_RandomDouble : Method
    {
        /// <summary>
        /// A generic method for randomizing doubles
        /// </summary>
        public mGeneric_RandomDouble() : base()
        {
            m_name = "Generic.RandomDouble";

            m_param.Add(new pDouble(0));
        }

        /// <summary>
        /// Generic.Random(double max)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 parameter and 0 optional parameters.
        /// [0] = pDouble //maximum number to randomize</param>
        /// <returns>Returns a pDouble that contains the number randomized</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pDouble((double)Randomization.TrueRandom((float)param[0].Data));
        }
    }

    /// <summary>
    /// A generic method for showing a MessageBox
    /// </summary>
    public sealed class mGeneric_MessageBox : Method
    {
        /// <summary>
        /// A generic method for showing a MessageBox
        /// </summary>
        public mGeneric_MessageBox() : base()
        {
            m_name = "Generic.MessageBox";

            pString str = new pString("");
            m_param.Add(str);
            //title of msgbox
            m_paramOpt.Add(str);
        }

        /// <summary>
        /// Generic.MessageBox(string text [, string title])
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 1 optional parameter.
        /// param[0] = pString //Text that the MessageBox displays
        /// param[1] = pString //Title of the MessageBox; optional</param>
        /// <returns>Returns null</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param.Length == 1)
                MessageBox.Show(param[0].Data.ToString());
            else if (param.Length == 2)
                MessageBox.Show(param[0].Data.ToString(), param[1].Data.ToString());

            return null;
        }
    }

    /// <summary>
    /// A generic method for converting parameters to strings
    /// </summary>
    public sealed class mConvert_ToString : Method
    {
        /// <summary>
        /// A generic method for converting parameters to strings
        /// </summary>
        public mConvert_ToString() : base()
        {
            m_name = "Convert.ToString";

            m_param.Add(new IParameter(null));
        }

        /// <summary>
        /// Convert.ToString(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = IParameter //Parameter to convert</param>
        /// <returns>Returns a pString that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pString(param[0].Data.ToString());
        }
    }

    /// <summary>
    /// A generic method for converting parameters to integers
    /// </summary>
    public sealed class mConvert_ToInt : Method
    {
        /// <summary>
        /// A generic method for converting parameters to integers
        /// </summary>
        public mConvert_ToInt() : base()
        {
            m_name = "Convert.ToInt";

            m_param.Add(new IParameter(null));
        }

        /// <summary>
        /// Convert.ToInt(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = IParameter //Parameter to convert</param>
        /// <returns>Returns a pInt that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param[0].Type == new pBool(false).Type)
            {
                if (param[0].Data == (object)true)
                    return new pInt(1);
                else
                    return new pInt(0);
            }
            return new pInt(Parse.String2Int(param[0].Data.ToString()));
        }
    }

    /// <summary>
    /// A generic method for converting parameters to doubles
    /// </summary>
    public sealed class mConvert_ToDouble : Method
    {
        /// <summary>
        /// A generic method for converting parameters to doubles
        /// </summary>
        public mConvert_ToDouble() : base()
        {
            m_name = "Convert.ToDouble";

            m_param.Add(new IParameter(null));
        }

        /// <summary>
        /// Convert.ToDouble(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = IParameter //Parameter to convert</param>
        /// <returns>Returns a pDouble that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param[0].Type == new pBool(false).Type)
            {
                if (param[0].Data == (object)true)
                    return new pDouble(1);
                else
                    return new pDouble(0);
            }
            return new pDouble(Parse.String2Double(param[0].Data.ToString()));
        }
    }

    /// <summary>
    /// A generic method for converting parameters to booleans
    /// </summary>
    public sealed class mConvert_ToBool : Method
    {
        /// <summary>
        /// A generic method for converting parameters to booleans
        /// </summary>
        public mConvert_ToBool() : base()
        {
            m_name = "Convert.ToBool";

            m_param.Add(new IParameter(null));
        }

        /// <summary>
        /// Convert.ToDouble(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = IParameter //Parameter to convert</param>
        /// <returns>Returns a pBool that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param[0].Type == new pInt(0).Type || param[0].Type == new pDouble(0).Type)
            {
                if ((int)param[0].Data < 1)
                    return new pBool(false);
                else
                    return new pBool(true);
            }
            return new pBool(Parse.String2Bool(param[0].Data.ToString()));
        }
    }
}
