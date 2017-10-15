using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Common;
using System.Text.RegularExpressions;

namespace Discover.Logic
{
    /// <summary>
    /// Passes a logical statement and returns a result based on the predicate result
    /// i.e (param1 == 2 || param2 == 'hello') && param3 != param4 ? 'Success result is this' : 'Failed result is this'
    /// or (for multiple checks) => ((boo > 1) || param2 == 'hello') && param3 != param4 ? 'Success result is this' : 'Failed result is this';(param1 == 2 || param2 == 'hello') && param3 != param4 ? 'Success result is this' : 'Failed result is this'

    /// </summary>
    public class LogicEvaluator
    {
        public LogicEvaluator(Dictionary<string, string> paramVals, string logicStatement)
        {
            this.BooleanStringOutputType = BooleanStringOutput.Default;
            this.FailToFindParam = FailToFindParamBehaviour.InvalidateClause;
            this.Params = paramVals;
            this.Raw = RemoveWhiteSpace(logicStatement);
            this.Statements = new List<Statement>();
            this.BuildStatements();
        }

        public string Raw { get; protected set; }
        public FailToFindParamBehaviour FailToFindParam { get; set; }
        public BooleanStringOutput BooleanStringOutputType { get; set; }

        public List<Statement> Statements { get; set; }

        public Dictionary<string, string> Params { get; set; }

        protected void BuildStatements()
        {
            //split statements
            bool inLiteral = false;
            List<string> statements = new List<string>();
            int lastIndex = 0;
            for (int i = 0; i < this.Raw.Length; i++)
            {
                if (this.Raw[i] == '\'')
                {
                    inLiteral = !inLiteral;
                }
                else if (this.Raw[i] == ';')
                {
                    statements.Add(this.Raw.Substring(lastIndex, i));
                    lastIndex = i + 1;
                }
            }
            if (lastIndex < this.Raw.Length) statements.Add(this.Raw.Substring(lastIndex, this.Raw.Length - lastIndex));
            foreach (var s in statements)
            {
                Statement evalStatement = new Statement(this, s);
                this.Statements.Add(evalStatement);
            }
        }

        protected string RemoveWhiteSpace(string text)
        {
            return text.Trim()
                .Replace(" == ", "==")
                .Replace(" != ", "!=")
                .Replace(" >= ", ">=")
                .Replace(" <= ", "<=")
                .Replace(" > ", ">")
                .Replace(" < ", "<")
                .Replace(" && ", "&&")
                .Replace(" || ", "||");
        }

        public string Resolve()
        {
            return Resolve(null);
        }

        public string Resolve(IEnumerable<string> paramsToPreserve)
        {
            foreach (var s in this.Statements)
            {
                Reference result = s.Resolve();
                if (result != null && !string.IsNullOrWhiteSpace(result.Value)) return result.Value;
            }
            return null;
        }

        public bool Evaluate()
        {
            return Evaluate(null);
        }

        public bool Evaluate(IEnumerable<string> paramsToPreserve)
        {
            if (this.Statements.Count() == 0) throw new ArgumentException("Cannot evaluate statements - no statements to evaluate");
            if (this.Statements.Count() > 1) throw new ArgumentException("Cannot evaluate multiple statements");
            return this.Statements[0].Evaluate();
        }

        public string Compress()
        {
            return this.Compress(null);
        }

        public string Compress(IEnumerable<string> paramsToPreserve)
        {
            if (this.Statements.Count() == 0) throw new ArgumentException("Cannot compress statements - no statements to compress");
            if (this.Statements.Count() > 1) throw new ArgumentException("Cannot compress multiple statements");
            return this.Statements[0].Compress(paramsToPreserve);
        }

        public bool CanResolve()
        {
            return CanResolve(null);
        }

        public bool CanResolve(IEnumerable<string> paramsToPreserve)
        {
            return !(this.Statements.Any(s => s.CanEvaluate(paramsToPreserve) == false));
        }

        public bool TryGetParamValue(string paramName, out string value)
        {
            if (!Params.ContainsKey(paramName))
            {
                switch (FailToFindParam)
                {
                    case FailToFindParamBehaviour.TreatAsLiteral:
                        value = paramName;
                        break;
                    default:
                    case FailToFindParamBehaviour.ReplaceWithEmptyString:
                        value = "";
                        break;
                }
                return false;
            }

            value = Params[paramName] ?? string.Empty;
            return true;
        }

        public string GetParamValue(string paramName)
        {
            string value;
            if (TryGetParamValue(paramName, out value)) return value;
            switch (FailToFindParam)
            {
                case FailToFindParamBehaviour.ReplaceWithEmptyString:
                    return "";
                case FailToFindParamBehaviour.TreatAsLiteral:
                    return paramName;
                default:
                    throw new ArgumentException(string.Format("Could not find param value - {0}", paramName));
            }
        }

        internal string BooleanToString(bool val)
        {
            switch (this.BooleanStringOutputType)
            {

                case BooleanStringOutput.CamelCase:
                    return val ? "True" : "False";
                case BooleanStringOutput.Lowercase:
                    return val.ToString().ToLower();
                case BooleanStringOutput.NumericalZeroOne:
                    return val ? "1" : "0";
                default:
                case BooleanStringOutput.Default:
                    return val.ToString();
            }
        }

    }

    public enum FailToFindParamBehaviour
    {
        ThrowException,
        ReplaceWithEmptyString,
        TreatAsLiteral,
        InvalidateClause
    }

    public abstract class LogicBase
    {
        public LogicBase(LogicEvaluator evaluator)
        {
            this.Evaluator = evaluator;
        }

        protected LogicEvaluator Evaluator { get; set; }
        public bool Valid { get; protected set; }
    }

    public class Reference : LogicBase
    {
        public Reference(LogicEvaluator evaluator, string raw)
            : base(evaluator)
        {
            this.Raw = raw;
            this.Valid = true;
            if (!raw.StartsWith("'"))
            {
                if (StringHelper.IsNumber(raw))
                {
                    this.ReferenceType = Logic.ReferenceType.Number;
                    this.NumericalValue = decimal.Parse(raw);
                    this.Value = raw;
                }
                else
                {
                    this.ReferenceType = Logic.ReferenceType.Parameter;
                    this.ParamName = raw;
                    string val;
                    if (this.Evaluator.TryGetParamValue(raw, out val))
                    {
                        if (StringHelper.IsNumber(val))
                        {
                            NumericalValue = decimal.Parse(val);
                        }
                    }
                    else
                    {
                        switch (this.Evaluator.FailToFindParam)
                        {
                            case FailToFindParamBehaviour.ThrowException:
                                throw new ArgumentException(string.Format("Could not find parameter value in dictionary [{0}]", raw));
                            case FailToFindParamBehaviour.InvalidateClause:
                                this.Valid = false;
                                break;
                        }
                    }

                    Value = val;
                }
            }
            else
            {
                this.ReferenceType = Logic.ReferenceType.Literal;
                var sub = this.Raw.Substring(1);
                this.Value = this.Raw.Substring(1, this.Raw.Substring(1).LastIndexOf("'"));
                if (StringHelper.IsNumber(this.Value))
                {
                    NumericalValue = decimal.Parse(this.Value);
                }
            }
        }

        public string Raw { get; set; }
        public string Value { get; set; }
        public decimal? NumericalValue { get; set; }
        public string ParamName { get; set; }
        public ReferenceType ReferenceType { get; set; }


        public bool IsEqualTo(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue == reference.NumericalValue);
            if (this.Value == null) return reference.Value == null;
            return this.Value.Equals(reference.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsNotEqualTo(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue != reference.NumericalValue);
            if (this.Value == null) return reference.Value == null;
            return !this.Value.Equals(reference.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsLessThan(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue < reference.NumericalValue);
            throw new ArgumentException("Cannot compare references as numbers - numerical values not set");
        }

        public bool IsGreaterThan(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue > reference.NumericalValue);
            throw new ArgumentException("Cannot compare references as numbers - numerical values not set");
        }

        public bool IsLessThanOrEqualTo(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue <= reference.NumericalValue);
            throw new ArgumentException("Cannot compare references as numbers - numerical values not set");
        }

        public bool IsGreaterThanOrEqualTo(Reference reference)
        {
            if (this.NumericalValue != null && reference.NumericalValue != null) return (this.NumericalValue >= reference.NumericalValue);
            throw new ArgumentException("Cannot compare references as numbers - numerical values not set");
        }

        public string ToString()
        {
            return ToString(null);
        }

        public string ToString(IEnumerable<string> paramsToPreserve)
        {
            if (this.ReferenceType == Logic.ReferenceType.Parameter)
            {
                if (!this.Valid || (paramsToPreserve != null && paramsToPreserve.Any(p => p.Equals(this.ParamName, StringComparison.InvariantCultureIgnoreCase))))
                {
                    return ToRawString();
                }
            }
            return Value;
        }

        public string ToRawString()
        {
            switch (ReferenceType)
            {
                case Logic.ReferenceType.Parameter:
                    return ParamName;
                case Logic.ReferenceType.Number:
                    return Value;
                default:
                case Logic.ReferenceType.Literal:
                    return string.Format("'{0}'", Value);
            }
        }
    }

    public class Clause : LogicItem
    {
        public Clause(LogicEvaluator evaluator, string clause)
            : base(evaluator)
        {
            this.Raw = clause;
            this.BuildClause();
        }

        const string REGEX_PREDICATE = @"^(?<PRED>(?<LEFT>([a-zA-Z0-9@\[\]\.\-]+)|('.*'))(?<OPERATOR>(==)|(!=)|(>)|(<)|(>=)|(<=))(?<RIGHT>([a-zA-Z0-9@\[\]\.\-]+)|('.*')))$";
        public string Raw { get; set; }
        public Reference Left { get; set; }
        public CompareOperator Operator { get; set; }
        public Reference Right { get; set; }

        public bool? Result { get; set; }

        public override bool Evaluate(IEnumerable<string> paramsToPreserve)
        {
            if (!CanEvaluate(paramsToPreserve)) return false;
            switch (Operator)
            {
                case CompareOperator.GreaterThan:
                    return Left.IsGreaterThan(this.Right);
                case CompareOperator.GreaterThanOrEqualTo:
                    return Left.IsGreaterThanOrEqualTo(this.Right);
                case CompareOperator.LessThan:
                    return Left.IsLessThan(this.Right);
                case CompareOperator.LessThanOrEqualTo:
                    return Left.IsLessThanOrEqualTo(this.Right);
                case CompareOperator.NotEqualTo:
                    return Left.IsNotEqualTo(this.Right);
                default:
                case CompareOperator.EqualTo:
                    return Left.IsEqualTo(this.Right);
            }
        }

        private void BuildClause()
        {
            Regex regex = new Regex(REGEX_PREDICATE);
            Match m = regex.Match(this.Raw);
            if (!m.Success) throw new ArgumentException(string.Format("Badly formed clause [{0}]", this.Raw));
            this.Left = new Reference(this.Evaluator, m.Groups["LEFT"].Value);
            this.Right = new Reference(this.Evaluator, m.Groups["RIGHT"].Value);
            this.Operator = GetOperator(m.Groups["OPERATOR"].Value);
            this.Valid = this.Left.Valid && this.Right.Valid;
        }


        public string ToRawString()
        {
            return string.Concat(Left.ToRawString(), OperatorToString(), Right.ToRawString());
        }

        public override bool CanEvaluate(IEnumerable<string> paramsToPreserve)
        {
            return paramsToPreserve == null ? this.Valid
                : this.Valid &&
                !(paramsToPreserve.Any(p =>
                (this.Left.ReferenceType == ReferenceType.Parameter && this.Left.ParamName.Equals(p, StringComparison.InvariantCultureIgnoreCase))
                || (this.Right.ReferenceType == ReferenceType.Parameter && this.Right.ParamName.Equals(p, StringComparison.InvariantCultureIgnoreCase))));
        }

        public override string Compress(IEnumerable<string> paramsToPreserve)
        {
            if (CanEvaluate(paramsToPreserve))
            {
                return this.Evaluator.BooleanToString(this.Evaluate());
            }
            else
            {
                return string.Concat(this.Left.ToString(paramsToPreserve), " ", OperatorToString(), " ", this.Right.ToString(paramsToPreserve));
            }
        }

        public string OperatorToString()
        {
            switch (Operator)
            {

                case CompareOperator.GreaterThan:
                    return ">";
                case CompareOperator.GreaterThanOrEqualTo:
                    return ">=";
                case CompareOperator.LessThan:
                    return "<";
                case CompareOperator.LessThanOrEqualTo:
                    return "<=";
                case CompareOperator.NotEqualTo:
                    return "!=";
                default:
                case CompareOperator.EqualTo:
                    return "==";
            }
        }

        public CompareOperator GetOperator(string op)
        {
            switch (op)
            {
                case ">": return CompareOperator.GreaterThan;
                case "<": return CompareOperator.LessThan;
                case ">=": return CompareOperator.GreaterThanOrEqualTo;
                case "<=": return CompareOperator.LessThanOrEqualTo;
                case "!=": return CompareOperator.NotEqualTo;
                default:
                case "==": return CompareOperator.EqualTo;
            }
        }
    }

    public class Combination : LogicItem
    {
        public Combination(LogicEvaluator evaluator, string statement)
            : base(evaluator)
        {
            this.Items = new List<LogicItem>();
            this.Raw = statement;
            this.BuildCombination();
        }



        public List<LogicItem> Items { get; protected set; }
        public CombineOperator Operator { get; protected set; }
        public string Raw { get; set; }

        public override bool Evaluate(IEnumerable<string> paramsToPreserve)
        {
            if (!CanEvaluate(paramsToPreserve)) return false;
            switch (Operator)
            {

                case CombineOperator.Or:
                    return EvaluateForOr();
                default:
                case CombineOperator.And:
                    return EvaluateForAnd();
            }
        }

        private bool EvaluateForAnd()
        {
            foreach (var c in this.Items)
            {
                if (!c.Evaluate()) return false;
            }
            return true;
        }

        private bool EvaluateForOr()
        {
            foreach (var c in this.Items)
            {
                if (c.Evaluate()) return true;
            }
            return false;
        }

        private void BuildCombination()
        {
            int iteration = 0;
            List<string> combos = new List<string>();
            int lastComboIndex = 0;
            int i = 0;
            string combo;
            while (i < this.Raw.Length)
            {
                if (this.Raw[i] == '(')
                {
                    combo = this.Raw.Substring(lastComboIndex, i - lastComboIndex);
                    if(!string.IsNullOrWhiteSpace(combo))combos.Add(combo);
                    iteration++;
                    lastComboIndex = i + 1;
                }
                else if (this.Raw[i] == ')')
                {
                    iteration--;
                    if (iteration == 0)
                    {
                        combo = this.Raw.Substring(lastComboIndex, i - lastComboIndex);
                        if (!string.IsNullOrWhiteSpace(combo)) combos.Add(string.Concat("^", combo));
                        lastComboIndex = i + 1;
                    }

                }
                else if (iteration == 0 && this.Raw[i] == '&')
                {
                    combo = this.Raw.Substring(lastComboIndex, i - lastComboIndex);
                    if(!string.IsNullOrWhiteSpace(combo)) combos.Add(combo);
                    lastComboIndex = i + 2;
                    this.Operator = CombineOperator.And;
                    i = i + 1;
                }
                else if (iteration == 0 && this.Raw[i] == '|')
                {
                    combo = this.Raw.Substring(lastComboIndex, i - lastComboIndex);
                    if (!string.IsNullOrWhiteSpace(combo)) combos.Add(combo);
                    lastComboIndex = i + 2;
                    this.Operator = CombineOperator.Or;
                    i = i + 1;
                }

                i++;

            }

            var last = this.Raw.Substring(lastComboIndex);
            if (!string.IsNullOrWhiteSpace(last)) combos.Add(last);

            foreach (var s in combos)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (s[0] == '^')
                    {
                        this.Items.Add(new Combination(this.Evaluator, s.Substring(1)));
                    }
                    else
                    {
                        this.Items.Add(new Clause(this.Evaluator, s));
                    }
                }
            }

            this.Valid = true;
            foreach (var item in this.Items)
            {
                if (!item.Valid) this.Valid = false;
            }

        }

        public override bool CanEvaluate(IEnumerable<string> paramsToPreserve)
        {
            return this.Valid && !(this.Items.Any(i => i.CanEvaluate(paramsToPreserve) == false));
        }

        public override string Compress(IEnumerable<string> paramsToPreserve)
        {
            if (this.CanEvaluate(paramsToPreserve))
            {
                return this.Evaluator.BooleanToString(this.Evaluate());
            }
            else
            {
                StringBuilder output = new StringBuilder();
                foreach (var item in this.Items)
                {
                    if (output.Length > 0) output.AppendLine(string.Concat(" ", OperatorToString(), " "));
                    output.AppendLine(item.Compress());
                }
                return string.Format("({0})", output.ToString());
            }
        }

        public string OperatorToString()
        {
            switch (this.Operator)
            {
                case CombineOperator.Or:
                    return "||";
                default:
                case CombineOperator.And:
                    return "&&";

            }
        }
    }

    public abstract class LogicItem : LogicBase
    {
        public LogicItem(LogicEvaluator evaluator)
            : base(evaluator)
        {

        }

        public abstract bool Evaluate(IEnumerable<string> paramsToPreserve);
        public abstract string Compress(IEnumerable<string> paramsToPreserve);
        public abstract bool CanEvaluate(IEnumerable<string> paramsToPreserve);

        public string Compress()
        {
            return Compress(null);
        }

        public bool Evaluate()
        {
            return Evaluate(null);
        }
    }

    public class Statement : LogicBase
    {
        public Statement(LogicEvaluator evaluator, string statement)
            : base(evaluator)
        {
            this.Raw = statement;
            this.BuildStatement();
        }

        public string Raw { get; set; }
        public Reference Success { get; set; }
        public Reference Fail { get; set; }
        public LogicItem Test { get; set; }

        private void BuildStatement()
        {
            int endIndex = this.Raw.IndexOf("?");
            var test = this.Raw;
            if (endIndex >= 0)
            {
                var result = this.Raw.Substring(endIndex + 1).Split(':');
                this.Success = new Reference(this.Evaluator, result[0]);
                this.Fail = result.Length > 1 ? new Reference(this.Evaluator, result[1]) : null;
                test = Raw.Substring(0, endIndex);
            }
            else
            {
                this.Success = new Reference(this.Evaluator, string.Concat("'", this.Evaluator.BooleanToString(true), "'"));
                this.Fail = new Reference(this.Evaluator, string.Concat("'", this.Evaluator.BooleanToString(false), "'"));
            }
            if (test.Contains("&&") || test.Contains("||"))
            {
                //add combination
                this.Test = new Combination(this.Evaluator, test);
            }
            else
            {
                this.Test = new Clause(this.Evaluator, test);
            }

            this.Valid = this.Test.Valid;
        }

        public bool Evaluate()
        {
            return Evaluate(null);
        }

        public bool Evaluate(IEnumerable<string> paramsToPreserve)
        {
            return this.Test.Evaluate(paramsToPreserve);
        }

        public bool CanEvaluate()
        {
            return CanEvaluate(null);
        }

        public bool CanEvaluate(IEnumerable<string> paramsToPreserve)
        {
            return this.Test.CanEvaluate(paramsToPreserve);
        }

        public Reference Resolve()
        {
            return Resolve(null);
        }

        public Reference Resolve(IEnumerable<string> paramsToPreserve)
        {
            if (Test.Evaluate(paramsToPreserve)) return Success;
            return Fail;
        }

        public string Compress()
        {
            return Compress(null);
        }

        public string Compress(IEnumerable<string> paramsToPreserve)
        {
            return this.Test.Compress(paramsToPreserve);
        }
    }



    public enum CombineOperator
    {
        And = 1,
        Or = 2
    }

    public enum CompareOperator
    {
        EqualTo = 1,
        NotEqualTo = 2,
        LessThan = 4,
        GreaterThan = 8,
        LessThanOrEqualTo = 16,
        GreaterThanOrEqualTo = 32
    }

    public enum ReferenceType
    {
        Literal,
        Parameter,
        Number
    }

    public enum BooleanStringOutput
    {
        Default,
        NumericalZeroOne,
        Lowercase,
        CamelCase
    }
}
