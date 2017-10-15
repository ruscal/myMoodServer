using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Discover.Common;

namespace Discover.Logic
{
    public class LogicParser
    {
        public LogicParser()
        {
        }

        public LogicParser(string logic, Dictionary<string, string> paramVals)
        {
            _originalLogic = logic;
            _logic = logic;
            Params = paramVals;
        }

        public LogicParser(string logic, Dictionary<string, string> paramVals, bool ignoreTrailingText)
        {
            _originalLogic = logic;
            _logic = logic;
            _ignoreTrailingText = ignoreTrailingText;
            Params = paramVals;
        }

        const string REGEX_PREDICATE = @"(?<PRED>(?<LEFT>[a-zA-Z0-9@\[\]\.\-']+)(?<OPERATOR>(==)|(!=)|(>)|(<)|(>=)|(<=))(?<RIGHT>[a-zA-Z0-9@\[\]\.\-']+))";
        const string REGEX_ANDCLAUSE = @"(?<AND>(?<LEFT>1|0)&&(?<RIGHT>1|0))";
        const string REGEX_LITERALS = @"'(?<LIT>[^']*)'";
        protected string _originalLogic = "";
        protected string _logic = "";
        protected Dictionary<string, string> _params = new Dictionary<string, string>();
        protected bool _ignoreTrailingText = false;
        protected string _literalParamNameFormat = "@@lit{0}";
        protected int _literalCount = 0;

        public string Logic
        {
            get
            {
                return _logic;
            }
            set
            {
                _originalLogic = value;
                _logic = value;
            }
        }

        public string OriginalLogic
        {
            get
            {
                return _originalLogic;
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

        

        public bool Parse()
        {
            int endIndex = 0;
            return Parse(ref endIndex);
        }

        public bool Parse(string logic, Dictionary<string, string> paramVals)
        {
            Logic = logic;
            Params = paramVals;
            return Parse();
        }

        public bool Parse(ref int endIndex)
        {
            PrepLogic();
            // param1='3123'&param2='eerewer'&(Q3[2]==''||Q11!='123')
            if (Logic == "") throw new ArgumentException("Cannot parse logic - no logic provided.");
            // get rid of white space
            string logic = Logic;
            int eindex = 0;
            LogicParser lp;
            bool r;
            if (logic.StartsWith("("))
            {
                //need to calculate nested logic or find end bracket
                char c = logic[1];
                int index = 1;
                while (c != ')')
                {
                    if (c == '(')
                    {
                        //nested logic
                        lp = new LogicParser(logic.Substring(index), Params, true);
                        r = lp.Parse(ref eindex);
                        string result = (Convert.ToInt32(r)).ToString();
                        eindex = index + eindex;
                        string nlogic = logic.Substring(0, index) + result + ((logic.Length > eindex + 1) ? logic.Substring(eindex + 1) : "");
                        logic = nlogic;
                        index = index + (result.Length - 1);
                    }
                    index++;
                    c = logic[index];
                    if (index >= logic.Length) throw new ArgumentException("Could not find closing bracket.");
                }
                eindex = index;
                lp = new LogicParser(logic.Substring(1, (eindex - 1)), Params);
                r = lp.Parse();
                endIndex = Logic.Length - (logic.Length - eindex);
                if (logic.Length > (endIndex + 1) && !_ignoreTrailingText)
                {
                    //has trailing text
                    string result = (Convert.ToInt32(r)).ToString();
                    string nlogic = result + logic.Substring(eindex + 1);
                    logic = nlogic;
                    lp = new LogicParser(logic, Params);
                    return lp.Parse();
                }
                else
                {
                    return r;
                }
            }
            else
            {
                int index = logic.IndexOf("(");
                while (index > 0)
                {
                    lp = new LogicParser(logic.Substring(index), Params, true);
                    r = lp.Parse(ref eindex);
                    string result = (Convert.ToInt32(r)).ToString();
                    eindex = eindex + index;
                    string nlogic = logic.Substring(0, index) + result + ((logic.Length > eindex + 1) ? logic.Substring(eindex + 1) : "");
                    logic = nlogic;
                    index = logic.IndexOf("(");
                }


                //evaluate logic
                //this should be bracket free!
                //e.g. param1=='1234'||param2!=param3||param5<=9&&param6>5

                Regex regex = new Regex(REGEX_PREDICATE);
                MatchEvaluator me = new MatchEvaluator(ReplacePredicate);
                logic = regex.Replace(logic, me);
                //e.g. now should be   1||0||1&&1
                regex = new Regex(REGEX_ANDCLAUSE);
                me = new MatchEvaluator(ReplaceAndClause);
                while (logic.IndexOf('&') > 0)
                {
                    logic = regex.Replace(logic, me);
                }
                if (logic.IndexOf("||") > 0)
                {
                    logic = logic.Replace("||", "|");
                    string[] orArr = logic.Split('|');
                    foreach (string s in orArr)
                    {
                        if (s == "1") return true;
                    }
                    return false;
                }
                else
                {
                    return (logic == "1");
                }

            }

        }

        protected void PrepLogic()
        {
            //take out literals and store in params so can format logic
            Regex regex = new Regex(REGEX_LITERALS);
            MatchEvaluator me = new MatchEvaluator(ReplaceLiteral);
            _logic = regex.Replace(_logic, me);
            //remove space;
            _logic = _logic.Replace(" ", "");
        }

        protected string ReplaceLiteral(Match m)
        {
            string paramId = string.Format(_literalParamNameFormat, _literalCount);

            while (Params[paramId] != null)
            {
                _literalCount++;
                paramId = string.Format(_literalParamNameFormat, _literalCount);
            }
            _literalCount++;
            Params[paramId] = m.Groups["LIT"].Value;
            return paramId;
        }


        protected string ReplacePredicate(Match m)
        {
            string left = GetValue(m.Groups["LEFT"].Value);
            string right = GetValue(m.Groups["RIGHT"].Value);
            string op = m.Groups["OPERATOR"].Value;
            bool result = false;
            switch (op)
            {
                case "==":
                    result = (left == right);
                    break;
                case "!=":
                    result = (left != right);
                    break;
                case ">":
                    result = (double.Parse(left) > double.Parse(right));
                    break;
                case "<":
                    result = (double.Parse(left) < double.Parse(right));
                    break;
                case ">=":
                    result = (double.Parse(left) >= double.Parse(right));
                    break;
                case "<=":
                    result = (double.Parse(left) <= double.Parse(right));
                    break;
            }
            return (Convert.ToInt32(result)).ToString();
        }

        protected string ReplaceAndClause(Match m)
        {
            bool left = Convert.ToBoolean(int.Parse(m.Groups["LEFT"].Value));
            bool right = Convert.ToBoolean(int.Parse(m.Groups["RIGHT"].Value));
            return (Convert.ToInt32(left && right)).ToString();
        }

        protected string GetValue(string clause)
        {
            if (clause == "''") return "";
            if (clause.StartsWith("'"))
            {
                if (clause.EndsWith("'"))
                {

                    return clause.Substring(1, clause.Length - 2);
                }
                throw new ArgumentException("Invalid apostrophe in clause!");
            }
            if (StringHelper.IsNumber(clause))
            {
                return clause;
            }
            if (!Params.ContainsKey(clause)) throw new ArgumentException(string.Format("Reference string not found in supplied dictionary [{0}]", clause));
            return Params[clause];
        }
    }
}
