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
            m_name = "Generic.RandomInt";

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

    #region ConvertMethods
    /// <summary>
    /// A method for converting booleans to strings
    /// </summary>
    public sealed class mConvert_BoolToString : Method
    {
        /// <summary>
        /// A method for converting booleans to strings
        /// </summary>
        public mConvert_BoolToString() : base()
        {
            m_name = "Convert.BoolToString";

            m_param.Add(new pBool(false));
        }

        /// <summary>
        /// Convert.BoolToString(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pBool //Parameter to convert</param>
        /// <returns>Returns a pString that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pString(param[0].Data.ToString());
        }
    }

    /// <summary>
    /// A method for converting integers to strings
    /// </summary>
    public sealed class mConvert_IntToString : Method
    {
        /// <summary>
        /// A method for converting integers to strings
        /// </summary>
        public mConvert_IntToString() : base()
        {
            m_name = "Convert.IntToString";

            m_param.Add(new pInt(0));
        }

        /// <summary>
        /// Convert.IntToString(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pInt //Parameter to convert</param>
        /// <returns>Returns a pString that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pString(param[0].Data.ToString());
        }
    }

    /// <summary>
    /// A method for converting doubles to strings
    /// </summary>
    public sealed class mConvert_DoubleToString : Method
    {
        /// <summary>
        /// A method for converting doubles to strings
        /// </summary>
        public mConvert_DoubleToString() : base()
        {
            m_name = "Convert.DoubleToString";

            m_param.Add(new pDouble(0));
        }

        /// <summary>
        /// Convert.DoubleToString(IParameter param)
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
    /// A method for converting booleans to integers
    /// </summary>
    public sealed class mConvert_BoolToInt : Method
    {
        /// <summary>
        /// A method for converting booleans to integers
        /// </summary>
        public mConvert_BoolToInt() : base()
        {
            m_name = "Convert.BoolToInt";

            m_param.Add(new pBool(false));
        }

        /// <summary>
        /// Convert.BoolToInt(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pBool //Parameter to convert</param>
        /// <returns>Returns a pInt that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param[0].Data == (object)true)
                return new pInt(1);
            else
                return new pInt(0);
        }
    }

    /// <summary>
    /// A method for converting Strings to integers
    /// </summary>
    public sealed class mConvert_StringToInt : Method
    {
        /// <summary>
        /// A method for converting strings to integers
        /// </summary>
        public mConvert_StringToInt() : base()
        {
            m_name = "Convert.DoubleToInt";

            m_param.Add(new pString(null));
        }

        /// <summary>
        /// Convert.StringToInt(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pString //Parameter to convert</param>
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
    /// A method for converting doubles to integers
    /// </summary>
    public sealed class mConvert_DoubleToInt : Method
    {
        /// <summary>
        /// A method for converting doubles to integers
        /// </summary>
        public mConvert_DoubleToInt() : base()
        {
            m_name = "Convert.StringToInt";

            m_param.Add(new pDouble(0));
        }

        /// <summary>
        /// Convert.DoubleToInt(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pDouble //Parameter to convert</param>
        /// <returns>Returns a pInt that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pInt((int)(double)param[0].Data);
        }
    }

    /// <summary>
    /// A method for converting integers to doubles
    /// </summary>
    public sealed class mConvert_IntToDouble : Method
    {
        /// <summary>
        /// A method for converting integers to doubles
        /// </summary>
        public mConvert_IntToDouble() : base()
        {
            m_name = "Convert.IntToDouble";

            m_param.Add(new pInt(0));
        }

        /// <summary>
        /// Convert.IntToDouble(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pInt //Parameter to convert</param>
        /// <returns>Returns a pDouble that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pDouble(Parse.String2Double(param[0].Data.ToString()));
        }
    }

    /// <summary>
    /// A method for converting booleans to doubles
    /// </summary>
    public sealed class mConvert_BoolToDouble : Method
    {
        /// <summary>
        /// A method for converting booleans to doubles
        /// </summary>
        public mConvert_BoolToDouble() : base()
        {
            m_name = "Convert.BoolToDouble";

            m_param.Add(new pBool(false));
        }

        /// <summary>
        /// Convert.BoolToDouble(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pBool //Parameter to convert</param>
        /// <returns>Returns a pDouble that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if (param[0].Data == (object)true)
                return new pDouble(1);
            else
                return new pDouble(0);
        }
    }

    /// <summary>
    /// A method for converting strings to doubles
    /// </summary>
    public sealed class mConvert_StringToDouble : Method
    {
        /// <summary>
        /// A method for converting strings to doubles
        /// </summary>
        public mConvert_StringToDouble() : base()
        {
            m_name = "Convert.StringToDouble";

            m_param.Add(new pString(""));
        }

        /// <summary>
        /// Convert.StringToDouble(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pString //Parameter to convert</param>
        /// <returns>Returns a pDouble that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pDouble(Parse.String2Double(param[0].Data.ToString()));
        }
    }

    /// <summary>
    /// A method for converting strings to booleans
    /// </summary>
    public sealed class mConvert_StringToBool : Method
    {
        /// <summary>
        /// A method for converting strings to booleans
        /// </summary>
        public mConvert_StringToBool() : base()
        {
            m_name = "Convert.StringToBool";

            m_param.Add(new pString(""));
        }

        /// <summary>
        /// Convert.StringToBool(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pString //Parameter to convert</param>
        /// <returns>Returns a pBool that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            return new pBool(Parse.String2Bool(param[0].Data.ToString()));
        }
    }

    /// <summary>
    /// A method for converting doubles to booleans
    /// </summary>
    public sealed class mConvert_DoubleToBool : Method
    {
        /// <summary>
        /// A method for converting doubles to booleans
        /// </summary>
        public mConvert_DoubleToBool() : base()
        {
            m_name = "Convert.DoubleToBool";

            m_param.Add(new pDouble(0));
        }

        /// <summary>
        /// Convert.DoubleToBool(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pDouble //Parameter to convert</param>
        /// <returns>Returns a pBool that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if ((double)param[0].Data < 1)
                return new pBool(false);
            else
                return new pBool(true);
        }
    }

    /// <summary>
    /// A method for converting integers to booleans
    /// </summary>
    public sealed class mConvert_IntToBool : Method
    {
        /// <summary>
        /// A method for converting integers to booleans
        /// </summary>
        public mConvert_IntToBool() : base()
        {
            m_name = "Convert.IntToBool";

            m_param.Add(new pInt(0));
        }

        /// <summary>
        /// Convert.IntToBool(IParameter param)
        /// </summary>
        /// <param name="param">Parameters to pass to call the function.
        /// In this case, there is 1 required parameter and 0 optional parameters.
        /// param[0] = pInt //Parameter to convert</param>
        /// <returns>Returns a pBool that contains the converted parameter</returns>
        public override IParameter Call(params IParameter[] param)
        {
            base.Call(param);

            if ((int)param[0].Data < 1)
                return new pBool(false);
            else
                return new pBool(true);
        }
    }
    #endregion
}
