using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using MyMood.Web.Models;

namespace MyMood.LoadTest
{
    public class TestAgent
    {


        public static AggregateTestResult Run(string serverBaseUrl, string eventName, string testType, string authCode, int numberOfAgents, int targetRequestRate, int durationInSeconds)
        {
            var testUrl = string.Format("{0}/App/{1}/{2}/{3}", serverBaseUrl, eventName, string.Compare(testType, "ResponseOnly", true) == 0 ? "SubmitMoodResponse" : "SyncData", authCode);
            var serializer = new Newtonsoft.Json.JsonSerializer();

            Func<string, object> generateRequestBodyPayload = string.Compare(testType, "ResponseOnly", true) == 0 ? 
                new Func<string, object>(GenerateSubmitMoodResponseRequestBody) : 
                new Func<string, object>(GenerateSyncDataRequestBody);

            var terminateAt = DateTime.Now.AddSeconds(durationInSeconds);
            var targetMsBetweenRequests = targetRequestRate > 0 ? 1000 / targetRequestRate : 0;
            var agentThreads = new List<Thread>();

            var results = new AggregateTestResult() { StartTime = DateTime.Now };

            var finished = new CountdownEvent(1);

            for (var i = 0; i < numberOfAgents; i++)
            {
                var agentThread = new Thread(arg =>
                {
                    var simulatedRespondentId = Guid.NewGuid().ToString();

                    while (DateTime.Now < terminateAt)
                    {
                        var testResult = new TestResult() { RequestSentAt = DateTime.Now };

                        try
                        {
                            System.Diagnostics.Debug.WriteLine("Thread {0} sending request to {1}", Thread.CurrentThread.ManagedThreadId, testUrl);
                            var httpRequest = System.Net.HttpWebRequest.Create(testUrl);

                            httpRequest.Method = "POST";
                            httpRequest.ContentType = "application/json; charset=utf-8";

                            var request = generateRequestBodyPayload(simulatedRespondentId);

                            using (var requestStream = new System.IO.StreamWriter(httpRequest.GetRequestStream()))
                            {
                                serializer.Serialize(requestStream, request);
                            }

                            using(var httpResponse = httpRequest.GetResponse())
                            {
                                //UpdateAppFromServiceModel response = null;

                                using(var responseStream = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                                {
                                    responseStream.ReadToEnd();
                                    //response = serializer.Deserialize<UpdateAppFromServiceModel>(responseStream.ReadToEnd());
                                }

                                //if(response.
                            }

                            testResult.ResponseReceivedAt = DateTime.Now;
                            testResult.Successful = true;
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            testResult.Successful = false;
                        }

                        results.AddResult(testResult);

                        var nextRequestDelay = testResult.RequestSentAt.AddMilliseconds(targetMsBetweenRequests).Subtract(DateTime.Now);

                        if (nextRequestDelay.TotalMilliseconds > 0)
                        {
                            Thread.Sleep(nextRequestDelay);
                        }
                    }

                    finished.Signal();
                }, 250);

                agentThreads.Add(agentThread);

                finished.AddCount();

                Thread.Sleep(targetMsBetweenRequests > 0 ? targetMsBetweenRequests / numberOfAgents : 20);

                agentThread.Start();
            }

            finished.Signal();
            finished.Wait();

            results.EndTime = DateTime.Now;

            return results;
        }

        private static UpdateServiceFromAppModel GenerateSyncDataRequestBody(string simulatedRespondentId)
        {
            var responseCount = rng.Next(1, 6);

            return new UpdateServiceFromAppModel
            {
                rid = simulatedRespondentId,
                apn = simulatedRespondentId,
                reg = simulatedRespondentId,
                Responses = Enumerable.Range(1, responseCount).Select(n => GenerateMoodResponse()).ToArray(),
                ResTotal = responseCount,
                LastUpdate = null
            };
        }

        private static SubmitResponseModel GenerateSubmitMoodResponseRequestBody(string simulatedRespondentId)
        {
            return new SubmitResponseModel
            {
                rid = simulatedRespondentId,
                apn = simulatedRespondentId,
                reg = simulatedRespondentId,
                r = GenerateMoodResponse()
            };
        }

        private static MoodResponseUpdateModel GenerateMoodResponse()
        {
            return new MoodResponseUpdateModel
            {
                i = Guid.NewGuid().ToString(), // response id
                m = MoodNames[rng.Next(0, MoodNames.Length - 1)], // mood name
                p = null, // mood prompt id
                t = DateTime.UtcNow
            };
        }

        private static string[] MoodNames = { "Passionate", "Excited", "Proud", "Engaged", "Optimistic", "Frustrated", "Worried", "Bored", "Deflated", "Disengaged" };
        private static Random rng = new Random();
    }

    public class AggregateTestResult
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public TimeSpan Duration { get { return EndTime - StartTime; } }

        public IEnumerable<TestResult> Tests { get; protected set; }

        public AggregateTestResult()
        {
            Tests = new ConcurrentQueue<TestResult>();
        }

        public void AddResult(TestResult result)
        {
            ((ConcurrentQueue<TestResult>)Tests).Enqueue(result);
        }
    }

    public class TestResult
    {
        public DateTime RequestSentAt { get; set; }

        public DateTime? ResponseReceivedAt { get; set; }

        public TimeSpan? Duration { get { return ResponseReceivedAt.HasValue ? ResponseReceivedAt.Value - RequestSentAt : (TimeSpan?)null; } }

        public bool Successful { get; set; }
    }
}
