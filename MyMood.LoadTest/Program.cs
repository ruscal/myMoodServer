using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace MyMood.LoadTest
{
    class Program
    {
        public class Options
        {
            [Option('s', "server", Required = true, HelpText = "The base URL of the myMood server to load test")]
            public string ServerBaseUrl { get; set; }

            [Option('e', "event", Required = true, HelpText = "The event to generate responses against")]
            public string EventName { get; set; }

            [Option('p', "passcode", Required = true, HelpText = "The event passcode for submitting app-related data")]
            public string PassCode { get; set; }

            [Option('t', "testtype", Required = true, HelpText = "The type of message/payload to send to the server under test (ResponseOnly | FullSync)")]
            public string TestType { get; set; }

            [Option('a', "agents", Required = false, DefaultValue = 100, HelpText = "The number of (simulated respondent) agents to instantate for the load test")]
            public int AgentCount { get; set; }

            [Option('r', "rate", Required = false, DefaultValue = 0, HelpText = "The number of times per second that each agent should attempt to communicate with the server")]
            public int RequestRate { get; set; }

            [Option('d', "duration", Required = false, DefaultValue = 60, HelpText = "The total number of seconds to conduct the load test for")]
            public int Duration { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.ReadKey(true);
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            System.Net.ServicePointManager.DefaultConnectionLimit = options.AgentCount;

            Console.Write(String.Format("Executing {0} test agents for {1} seconds on {2} ...", options.AgentCount, options.Duration, options.ServerBaseUrl));
            Console.Write("Press enter to start ...");
            Console.ReadLine();


            var results = TestAgent.Run(options.ServerBaseUrl, options.EventName, options.TestType, options.PassCode, options.AgentCount, options.RequestRate, options.Duration);

            Console.WriteLine();
            Console.WriteLine("---------------------------------");
            Console.WriteLine(String.Format("Total test duration: {0:c}", results.Duration));
            Console.WriteLine(String.Format("Total requests: {0}", results.Tests.Count()));
            
            if (results.Tests.Any())
            {
                Console.WriteLine(String.Format("Successful requests: {0} ({1} %)", results.Tests.Count(t => t.Successful), ((decimal)results.Tests.Count(t => t.Successful) / (decimal)results.Tests.Count()) * 100M));
                Console.WriteLine(String.Format("Failed requests: {0} ({1} %)", results.Tests.Count(t => !t.Successful), ((decimal)results.Tests.Count(t => !t.Successful) / (decimal)results.Tests.Count()) * 100M));

                Console.WriteLine();
                Console.WriteLine(String.Format("Total requests per second: {0}", results.Tests.Count() / results.Duration.TotalSeconds));
                Console.WriteLine(String.Format("Successful requests per second: {0}", results.Tests.Count(t => t.Successful) / results.Duration.TotalSeconds));

                var testsWithResponse = results.Tests.Where(t => t.ResponseReceivedAt.HasValue);
                if (testsWithResponse.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine(String.Format("Min response time: {0:0}ms", testsWithResponse.Select(t => t.Duration.Value.TotalMilliseconds).Min()));
                    Console.WriteLine(String.Format("Avg response time: {0:0}ms", testsWithResponse.Select(t => t.Duration.Value.TotalMilliseconds).Average())); 
                    Console.WriteLine(String.Format("Max response time: {0:0}ms", testsWithResponse.Select(t => t.Duration.Value.TotalMilliseconds).Max()));
                }
            }

            Console.WriteLine("---------------------------------");
            Console.ReadKey(true);
        }
    }
}
