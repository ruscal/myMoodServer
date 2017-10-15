using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Discover.Common;

namespace Discover.Logic
{
    /// <summary>
    /// Passes a logical statement and returns a result based on the predicate result
    /// i.e (param1 == 2 || param2 == 'hello') && param3 != param4 ? 'Success result is this' : 'Failed result is this'
    /// i.e (param1 == 2 || param2 == 'hello') && param3 != param4 ? 'Success result is this'; param4 == 'boo' ? 'return this' : 'return that' 
    /// </summary>
    public class LogicEngine
    {
        public LogicEngine()
        {
        }


        public LogicEngine(string logicStatement, Dictionary<string, string> paramVals)
        {
            LogicStatement = logicStatement;
            _originalLogicStatement = logicStatement;
            Params = paramVals;

        }

        const string REGEX_COMMAND = @"(?<command>SUM)\((?<param>[^)]*)\)";
        const string REGEX_LITERALS = @"'(?<LIT>[^']*)'";
        const string REGEX_LIT_PARAMS = @"(?<PARAM>{[\w]+})";
        protected string _logicStatement = "";
        protected string _originalLogicStatement = "";
        protected Dictionary<string, string> _params = new Dictionary<string, string>();
        protected LogicTest[] _tests = new LogicTest[0];
        protected string _literalParamNameFormat = "@@lit{0}";
        protected int _literalCount = 0;

        public string OriginalLogicStatement
        {
            get
            {
                return _originalLogicStatement;
            }
        }

        public string LogicStatement
        {
            get
            {
                return _logicStatement;
            }
            set
            {
                _logicStatement = value;
                _originalLogicStatement = value;
            }
        }

        public Dictionary<string, string> Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        

        public virtual string GetResult()
        {
            PrepLogic();
            foreach (LogicTest t in _tests)
            {
                if (t.Evaluate(Params))
                {
                    return t.Result;
                }
            }
            return "";
        }

        protected virtual string ReplaceLiteralParams(string literal)
        {
            Regex regex = new Regex(REGEX_LIT_PARAMS);
            MatchEvaluator me = new MatchEvaluator(ReplaceLiteralParam);
            literal = regex.Replace(literal, me);
            return literal;
        }

        protected string ReplaceLiteralParam(Match m)
        {
            string hit = m.Groups["PARAM"].Value;
            string paramName = hit.Substring(0, hit.Length - 2);
            if (Params.ContainsKey(paramName))
            {
                string val = Params[paramName].ToString();
                return val;
            }
            else
            {
                return hit;
            }
        }

        protected virtual void PrepLogic()
        {
            //take out literals and store in params so can format logic
            Regex regex = new Regex(REGEX_LITERALS);
            MatchEvaluator me = new MatchEvaluator(ReplaceLiteral);
            _logicStatement = regex.Replace(_logicStatement, me);

            //replace any keyword implementations
            regex = new Regex(REGEX_COMMAND);
            me = new MatchEvaluator(ReplaceCommand);
            _logicStatement = regex.Replace(_logicStatement, me);

            string[] testArr = _logicStatement.Split(';');
            _tests = new LogicTest[testArr.Length];
            for (int i = 0; i < testArr.Length; i++)
            {
                _tests[i] = new LogicTest(testArr[i]);
            }
        }

        protected string ReplaceLiteral(Match m)
        {
            string paramId = string.Format(_literalParamNameFormat, _literalCount);

            while (Params.ContainsKey(paramId))
            {
                _literalCount++;
                paramId = string.Format(_literalParamNameFormat, _literalCount);
            }
            _literalCount++;
            Params[paramId] = ReplaceLiteralParams(m.Groups["LIT"].Value);
            return paramId;
        }

        protected string ReplaceCommand(Match m)
        {
            switch (m.Groups["command"].Value)
            {
                case "SUM": return Sum(m.Groups["param"].Value);

            }
            return m.ToString();
        }

        protected string Sum(string statement)
        {
            if (!string.IsNullOrEmpty(statement))
            {
                //string arrStr = statement.Substring(1, statement.IndexOf("]"));
                string[] arr = statement.Split(',');
                double total = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    string val = arr[i];
                    double value = 0;
                    if (StringHelper.IsNumber(val))
                    {
                        double.TryParse(arr[i], out value);
                    }
                    else
                    {
                        if (Params.ContainsKey(val))
                        {
                            double.TryParse(Sum(Params[val]), out value);
                        }
                    }
                    total += value;
                }
                return total.ToString();
            }

            return "0";
        }

    }
}
