using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Logic
{
    public class LogicTest
    {
        public LogicTest(string test)
        {
            if (test != "")
            {
                _test = test;
                int index = test.IndexOf('?');
               
                if (index < 0) throw new ArgumentException("Statement has no '?'");
                _logicStatement = test.Substring(0, index);
                string success = test.Substring(index + 1);
                index = success.IndexOf(':');
                if (index < 0)
                {
                    _success = success.Trim();
                }
                else
                {
                    _success = success.Substring(0, index).Trim();
                    _fail = success.Substring(index + 1).Trim();
                    _hasFailResult = true;
                }
            }
        }

        protected string _test = "";
        protected string _logicStatement = "";
        protected bool _pass = false;
        protected string _result = "";
        protected bool _hasFailResult = false;
        protected string _success = "";
        protected string _fail = "";

        public string Test
        {
            get
            {
                return _test;
            }
            set
            {
                _test = value;
            }
        }

        public bool Passed
        {
            get
            {
                return _pass;
            }
        }

        public string Result
        {
            get
            {
                return _result;
            }
        }

        public string Success
        {
            get
            {
                return _success;
            }
        }

        public string Fail
        {
            get
            {
                return _fail;
            }
        }

        public string LogicStatement
        {
            get
            {
                return _logicStatement;
            }
        }



        public bool Evaluate(Dictionary<string, string> paramVals)
        {
            if (LogicStatement == "") return false;
            LogicParser lp = new LogicParser(LogicStatement, paramVals);
            if (lp.Parse())
            {
                //logic passed 
                _pass = true;
                _result = Success;
                return true;
            }
            else
            {
                if (_hasFailResult)
                {
                    // logic failed but statement passed
                    _pass = true;
                    _result = Fail;
                    return true;
                }
                else
                {
                    //logic failed and statement failed (had no fail result)
                    _pass = false;
                    _result = "";
                    return false;
                }
            }
        }
    }
}
