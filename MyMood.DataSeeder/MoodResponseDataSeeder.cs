using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyMood.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MyMood.DataSeeder
{
    class MoodResponseDataSeeder
    {
        string requestUrl = "http://www.learning-performance.com/MyMood/App/SyncData/";
        string appId = "ABBA6130-9663-4FA4-D1D4-08CF435A7DE9";
        string[] moods = new string[]{
            "Excited",
            "Optimistic",
            "Engaged",
            "Proud",
            "Passionate",
            "Frustrated",
            "Disengaged",
            "Deflated",
            "Bored",
            "Worried"
        };

        public void SeedData(DateTime from, DateTime to, int stepMins, int responsCount)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            var responseIds = new List<Guid>();
            for (int i = 0; i < responsCount; i++)
            {
                responseIds.Add(System.Guid.NewGuid());
            }

            DateTime currentTime = from;

            while (currentTime < to)
            {
                foreach (var responseId in responseIds)
                {
                    SubmitResponse(responseId, new MoodResponseUpdateModel()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        MyMood =  new MoodModel(){
                            Name = moods[rand.Next(0, moods.Length-1)]
                        },
                        TimeStamp = currentTime
                    });
                }
                currentTime = currentTime.AddMinutes(stepMins);
            }

        }

        private void SubmitResponse(Guid responseId, MoodResponseUpdateModel response)
        {
            var model = new UpdateServiceFromAppModel()
            {
                AppId = appId,
                LastSuccessfulServiceUpdate = DateTime.UtcNow,
                ResponderId = responseId.ToString(),
                Responses = new List<MoodResponseUpdateModel>() { response }
            };

            JObject value = null;

            try
            {
                Console.WriteLine("Submitting request: " + requestUrl + " :: " + model.ResponderId);
                var req = HttpWebRequest.Create(requestUrl + appId);
                req.ContentType = "application/json";
                req.Method = "POST";

                JObject postJson = JObject.FromObject(model);

                string postJsonString = postJson.ToString();

                byte[] data = new ASCIIEncoding().GetBytes(postJsonString);
                req.ContentLength = data.Length;

                Stream dataStream = req.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        value = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    }
                }
                Console.WriteLine("Request completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed request:");
                Console.WriteLine(ex.ToString());
            }

        }
    }



}
