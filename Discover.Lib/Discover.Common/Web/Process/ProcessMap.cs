using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Common;

namespace Discover.Web.Process
{
    [Serializable]
    public class ProcessMap
    {
        #region Static Members

        //private static Dictionary<string, ProcessMap> _processes = new Dictionary<string, ProcessMap>(StringComparer.InvariantCultureIgnoreCase);

        //public static ProcessMap Register(string processName)
        //{
        //    return Register(processName, null);
        //}

        //public static ProcessMap Register(string processName, Action<ProcessMap> configure)
        //{
        //    var process = new ProcessMap(processName);
        //    _processes.Add(processName, process);
        //    if (configure != null)
        //    {
        //        configure(process);
        //    }
        //    return process;
        //}

        //public static ProcessMap Find(string processName)
        //{
        //    return _processes.ContainsKey(processName) ? _processes[processName] : null;
        //}

        public static ProcessMap Define(string processName)
        {
            return Define(processName, null);
        }

        public static ProcessMap Define(string processName, Action<ProcessMap> configure)
        {
            var process = new ProcessMap(processName);
            if (configure != null)
            {
                configure(process);
            }
            return process;
        }

        #endregion

        public string Name { get; protected set; }

        public string FriendlyName { get; set; }

        public string StartingUrl { get; set; }

        public string OnCancelledUrl { get; set; }

        public string OnCompletedUrl { get; set; }

        protected List<Step> _steps = new List<Step>();

        public IEnumerable<Step> Steps { get { return _steps; } }

        protected List<StepMapper> _stepMappers = new List<StepMapper>();

        public IEnumerable<StepMapper> StepMappers { get { return _stepMappers; } }

        public ProcessMap() { }

        public ProcessMap(string name)
        {
            Name = name;
        }

        public ProcessMap DefineStep(Step step)
        {
            if (_steps.Any(s => s.Name.Equals(step.Name, StringComparison.InvariantCultureIgnoreCase))) throw new ArgumentException("This process already contains a step with the name \"" + step.Name + "\"", "stepName");
            step.Index = _steps.Count;
            _steps.Add(step);
            return this;
        }

        public ProcessMap DefineStepMapper(StepMapper stepMapper)
        {
            return DefineStepMapper(stepMapper.Name, stepMapper.RoutePath);
        }

        public ProcessMap DefineStepMapper(string mapperName, string path)
        {
            if (_stepMappers.Any(r => r.Name.Equals(mapperName, StringComparison.InvariantCultureIgnoreCase))) throw new ArgumentException("This process already contains a route with the name \"" + mapperName + "\"", "routeName");
            _stepMappers.Add(new StepMapper(mapperName, path));
            return this;
        }

        public Step FindStep(string stepName)
        {
            return FindStepByName(stepName);
            
        }

        public Step FindStep(string stepName, Dictionary<string, string> parameters)
        {
            Step step = FindStepByName(stepName);

            //if (step == null)
            //    step = FindStepByBaseName(stepName);

            if (step != null) return step;
            StepMapper route = FindRouteByName(stepName);

            if (route == null) return null;
            stepName = StringHelper.ReplaceReferences(route.RoutePath, parameters);
            return FindStepByName(stepName);
        }

        public Step FindStepBefore(string stepName)
        {
            return _steps.TakeWhile(s => !s.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase)).LastOrDefault();
        }

        public Step FindStepAfter(string stepName)
        {
            return _steps.SkipWhile(s => !s.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase)).Skip(1).FirstOrDefault();
        }

        private StepMapper FindRouteByName(string routeName)
        {
            return _stepMappers.FirstOrDefault(r => r.Name.Equals(routeName, StringComparison.InvariantCultureIgnoreCase));
        }

        private Step FindStepByName(string stepName)
        {
            return _steps.FirstOrDefault(s => s.Name.Equals(stepName, StringComparison.InvariantCultureIgnoreCase));
        }

        //private Step FindStepByBaseName(string stepName)
        //{
        //    return _steps.FirstOrDefault(s => s.BaseName.Equals(stepName, StringComparison.InvariantCultureIgnoreCase));
        //}
    }
}
