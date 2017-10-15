using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyMood.Domain;
using Discover.DomainModel;
using Discover.Logging;
using Discover.Mail;
using Discover.HtmlTemplates;
using MyMood.Web.Models;
using System.Drawing;

namespace MyMood.Web.Controllers
{
    public abstract class EventControllerBase : ControllerBase
    {

        protected EventControllerBase()
        {
        }

        public EventControllerBase(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        protected Event GetEvent(Guid id)
        {
            var evnt = this.db.Get<Event>().FirstOrDefault(e => e.Id == id);
            if (evnt == null) throw new ArgumentException("Invalid event");
            PopViewBag(evnt);
            return evnt;
        }

        protected Event GetEvent(string eventName)
        {
            var evnt = this.db.Get<Event>().FirstOrDefault(e => e.Name.Equals(eventName, StringComparison.InvariantCultureIgnoreCase));
            if (evnt == null) throw new ArgumentException("Invalid event");
            PopViewBag(evnt);
            return evnt;
        }

        private void PopViewBag(Event e)
        {
            ViewBag.EventId = e.Id;
            ViewBag.EventName = e.Name;
            ViewBag.EventTitle = e.Title;
        }

        protected Image GetGlobalMoodImage(Event evnt, MoodCategory category, DateTime reportStart, DateTime reportEnd, int moodIsStaleMins, float tension, bool showDataPoints, int width, int height)
        {
            GlobalMoodReportModel report = GetGlobalMoodReport(evnt, category, reportStart, reportEnd, moodIsStaleMins, false);
            GlobalMoodMapImage moodMap = new GlobalMoodMapImage()
            {
                Width = width,
                Height = height,
                ReportStart = reportStart,
                ReportEnd = reportEnd,
                Tension = tension,
                ShowDataPoints = showDataPoints
            };

            bool first = true;
            foreach (var snaphot in report.Snapshots.OrderBy(s => s.t))
            {
                //var moods = snaphot.Moods.OrderBy(m => m.DisplayIndex).Select(m => m.Mood).ToArray();
                //var moodColors = snaphot.Moods.OrderBy(m => m.DisplayIndex).Select(m => ColorTranslator.FromHtml(m.DisplayColor)).ToArray();
                //var responseCounts = snaphot.Moods.OrderBy(m => m.DisplayIndex).Select(m => m.ResponseCount).ToArray();
                //var responsePercentages = snaphot.Moods.OrderBy(m => m.DisplayIndex).Select(m => m.ResponsePercentage).ToArray();

                var moodData = from s in snaphot.d
                               join m in report.Moods on s.i equals m.DisplayIndex
                               select new MyMood.Web.GlobalMoodMapImage.MoodMapItem
                               {
                                   DisplayColor = ColorTranslator.FromHtml(m.DisplayColor),
                                   DisplayIndex = m.DisplayIndex,
                                   MoodType = m.MoodType,
                                   Name = m.Name,
                                   ResponseCount = s.c,
                                   ResponsePercentage = s.p
                               };
                if (first)
                {
                    if (snaphot.t > reportStart) moodMap.AddSnapshot(reportStart, moodData.OrderBy(m => m.DisplayIndex).ToList());
                    first = false;
                }
                moodMap.AddSnapshot(snaphot.t, moodData.OrderBy(m => m.DisplayIndex).ToList());
            }
            return moodMap.ToBitmap();
        }

        protected GlobalMoodReportInfoModel GetGlobalMoodReportInfo(Event evnt, DateTime? lastReportRequest, DateTime? lastUpdate)
        {

            var earliest = this.db.Get<Snapshot>().Where(s => s.Category.Event.Id == evnt.Id).OrderBy(s => s.TimeStamp).FirstOrDefault();
            var hasUpdates = lastReportRequest.HasValue ? this.db.Get<Snapshot>().Any(s => s.LastEditedDate > lastReportRequest.Value) : true;
            DateTime requestDate = DateTime.UtcNow;
            var startDate = evnt.StartDate.HasValue ? evnt.StartDate.Value : earliest == null ? DateTime.UtcNow.Date : earliest.TimeStamp.Date;
            var endDate = evnt.EndDate.HasValue ? evnt.EndDate.Value : DateTime.UtcNow.AddDays(1).Date.AddMinutes(-1);
            var hasAppUpdates = lastUpdate == null || evnt.ApplicationConfig.LastEditedDate > lastUpdate.Value;
            var hasActivityUpdates = lastUpdate == null || this.db.Get<Activity>().Any(a => a.Event.Id == evnt.Id && a.LastEditedDate > lastUpdate.Value);

            return new GlobalMoodReportInfoModel()
                {
                    RequestTimeStamp = requestDate,
                    ReportStartDate = startDate,
                    ReportEndDate = endDate,
                    DaysInReport = (int)Math.Ceiling(endDate.Subtract(startDate).TotalDays),
                    HasNewData = hasUpdates,
                    DayStartsOn = evnt.ApplicationConfig.DayStartsOn ?? new DateTime(2013, 1, 1, 6, 0, 0),
                    DayEndsOn = evnt.ApplicationConfig.DayEndsOn ?? new DateTime(2013, 1, 1, 21, 0, 0),
                    Application = hasAppUpdates ?
                        new ApplicationStateModel()
                        {
                            CurrentVersion = evnt.ApplicationConfig.CurrentVersion,
                            ForceUpdate = evnt.ApplicationConfig.ForceUpdate,
                            GoLiveDate = evnt.ApplicationConfig.GoLiveDate,
                            LANWebServiceUri = evnt.ApplicationConfig.LanServiceUri,
                            SyncDataInterval = evnt.ApplicationConfig.SyncDataInterval,
                            SyncReportInterval = evnt.ApplicationConfig.SyncReportInterval,
                            SyncMode = evnt.ApplicationConfig.SyncMode.ToString(),
                            WANWebServiceUri = evnt.ApplicationConfig.WebServiceUri,
                            WarnSyncFailureAfterMins = evnt.ApplicationConfig.WarnSyncFailureAfterMins,
                            EventTimeZone = evnt.ApplicationConfig.TimeZone,
                            EventTimeOffset = GetEventUtcOffset(evnt),
                            UpdateAppUri = evnt.ApplicationConfig.UpdateAppUri,
                            ConnectionTimeout = evnt.ApplicationConfig.ConnectionTimeout
                        } : null,
                    Prompts = hasActivityUpdates ? this.db.Get<Activity>().Where(a => a.Event.Id == evnt.Id && a.HasPrompt).OrderBy(a => a.TimeStamp).Select(a => new ActivityModel()
                    {
                        Id = a.Id,
                        TimeStamp = a.TimeStamp,
                        Title = a.Title
                    }).ToList() : null,
                    Events = hasActivityUpdates ? this.db.Get<Activity>().Where(a => a.Event.Id == evnt.Id && a.HasPrompt == false).OrderBy(a => a.TimeStamp).Select(a => new ActivityModel()
                    {
                        Id = a.Id,
                        TimeStamp = a.TimeStamp,
                        Title = a.Title
                    }).ToList() : null

                };
        }

        protected GlobalMoodReportModel GetGlobalMoodReport(Event evnt, MoodCategory cat, DateTime reportStart, DateTime reportEnd, int moodIsStaleMins, bool includeActivities)
        {
            return GetGlobalMoodReport(evnt, cat, reportStart, reportEnd, moodIsStaleMins, includeActivities, 0);
        }
        protected GlobalMoodReportModel GetGlobalMoodReport(Event evnt, MoodCategory cat, DateTime reportStart, DateTime reportEnd, int moodIsStaleMins, bool includeActivities, int roundPercentagesToNPlaces)
        {

            var moods = this.db.Get<Mood>().Where(m => m.Category.Id == cat.Id);

            // get last previous snapshot
            var roundedReportEnd = MoodResponse.GetRoundedResponseTime(reportEnd);


            var snapshots = (from s in this.db.Get<Snapshot>()
                             where s.Category.Id == cat.Id
                             && s.Category.Id == cat.Id && s.TimeStamp >= reportStart && s.TimeStamp <= roundedReportEnd
                             group s by s.TimeStamp into sg
                             select new MoodSnaphotReportModel()
                             {
                                 t = sg.Key,
                                 r = sg.Sum(r => r.ResponseCount),
                                 d = sg.Select(r => new MoodSnapshotDataModel()
                                 {
                                     c = r.ResponseCount,
                                     i = r.Mood.DisplayIndex,
                                     p = Math.Round(((decimal)r.ResponseCount / (decimal)sg.Sum(sgr => sgr.ResponseCount)) * 100M, roundPercentagesToNPlaces)
                                 })
                             }).OrderBy(s => s.t).ToList();

            var first = snapshots.FirstOrDefault();
            if (first == null || first.t != reportStart)
            {
                var startSnapTime = (from s in this.db.Get<Snapshot>()
                                     where s.Category.Id == cat.Id
                                     && s.Category.Id == cat.Id && s.TimeStamp < reportStart
                                     orderby s.TimeStamp descending
                                     select s.TimeStamp).FirstOrDefault();
                if (startSnapTime != DateTime.MinValue && startSnapTime >= reportStart.AddMinutes(-(evnt.ApplicationConfig.ReportMoodIsStaleMins)))
                {
                    var startSnap = (from s in this.db.Get<Snapshot>()
                                     where s.Category.Id == cat.Id
                                     && s.Category.Id == cat.Id && s.TimeStamp == startSnapTime
                                     group s by s.TimeStamp into sg
                                     select new MoodSnaphotReportModel()
                                     {
                                         //t = sg.Key,
                                         t = reportStart,
                                         r = sg.Sum(r => r.ResponseCount),
                                         d = sg.Select(r => new MoodSnapshotDataModel()
                                         {
                                             c = r.ResponseCount,
                                             i = r.Mood.DisplayIndex,
                                             p = Math.Round(((decimal)r.ResponseCount / (decimal)sg.Sum(sgr => sgr.ResponseCount)) * 100M, roundPercentagesToNPlaces)
                                         })
                                     }).Single();
                    snapshots.Insert(0, startSnap);
                }
                else
                {
                    var allMoods = this.db.Get<Mood>().Where(m => m.Category.Event.Id == evnt.Id).OrderBy(m => m.DisplayIndex).ToList();
                    snapshots.Insert(0, new MoodSnaphotReportModel()
                    {
                        t = reportStart,
                        r = 0,
                        d = allMoods.Select(m => new MoodSnapshotDataModel()
                        {
                            c = 0,
                            i = m.DisplayIndex,
                            p = Math.Round(100M / (decimal)allMoods.Count(), roundPercentagesToNPlaces)
                        })
                    });
                }



            }

    
            var prompts = this.db.Get<MoodPrompt>().Where(p => p.Event.Id == evnt.Id && p.Activity.TimeStamp >= reportStart && p.Activity.TimeStamp <= reportEnd).OrderBy(p => p.Activity.TimeStamp);


            var activities = from a in this.db.Get<Activity>()
                             join p in prompts on a.Id equals p.Activity.Id into ga
                             from pa in ga.DefaultIfEmpty()
                             select new { Activity = a, HasPrompt = pa != null };



            var model = new GlobalMoodReportModel()
            {
                Snapshots = snapshots,
                Prompts =includeActivities ? prompts.Select(p => new GlobalActivityModel()
                {
                    Id = p.Id,
                    Title = p.Activity.Title,
                    TimeStamp = p.Activity.TimeStamp
                }).ToList() : null,
                Activities = includeActivities ? activities.Where(a => a.HasPrompt == false).Select(e => new GlobalActivityModel()
                {
                    Id = e.Activity.Id,
                    Title = e.Activity.Title,
                    TimeStamp = e.Activity.TimeStamp
                }).ToList() : null,
                Moods = moods.Select(m => new MoodModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    DisplayColor = m.DisplayColor,
                    DisplayIndex = m.DisplayIndex,
                    MoodType = m.MoodType
                }).ToList()
            };

            return model;
        }




        protected IEnumerable<MoodResponse> GetSnapshotResponses(MoodCategory category, DateTime timeOfSnapshot, int moodIsStaleMins)
        {
            var fromDateTime = timeOfSnapshot.AddMinutes(-(moodIsStaleMins));

            if (category != null)
            {
                var responses = (from r in category.Moods
                                     .SelectMany(m =>
                                         m.Responses.Where(r => r.TimeStamp <= timeOfSnapshot && (moodIsStaleMins == 0 || r.TimeStamp > fromDateTime)))
                                 group r by r.Responder.Id into gr
                                 select gr.OrderByDescending(r => r.TimeStamp).ThenByDescending(r => r.CreationDate).FirstOrDefault()
                                ).ToList();
                return responses;
            }

            return new List<MoodResponse>();
        }

        protected IEnumerable<MoodResponse> GetAllResponses(MoodCategory category, DateTime timeOfSnapshot, int moodIsStaleMins)
        {

            var fromDateTime = timeOfSnapshot.AddMinutes(-(moodIsStaleMins));

            if (category != null)
            {
                var responses = category.Moods
                                     .SelectMany(m => m.Responses).Where(r => r.TimeStamp <= timeOfSnapshot && (moodIsStaleMins == 0 || r.TimeStamp > fromDateTime))
                                     .OrderByDescending(x => x.TimeStamp)
                                     .ThenByDescending(x => x.CreationDate).ToList();
                return responses;

            }

            return new List<MoodResponse>();
        }

        protected int GetEventUtcOffset(Event evnt)
        {
            if (string.IsNullOrEmpty(evnt.ApplicationConfig.TimeZone)) return 0;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(evnt.ApplicationConfig.TimeZone);
            return tz.BaseUtcOffset.Hours;
        }

        protected MoodSnaphotReportModel GetTotals(MoodCategory category,  int moodIsStaleMins, bool includeMoodInfo)
        {
            var responses = GetAllResponses(category, DateTime.UtcNow, moodIsStaleMins);
            return BuildMoodSnapshotReportFromResponses(responses, category, DateTime.UtcNow, includeMoodInfo);

            //return GetSnapshot(category, timeOfSnapshot, includeMoodInfo);

        }

        protected IEnumerable<ReportRow> GetTotalsByPrompt(Event evnt)
        {
            var respones = evnt.Responders.SelectMany(x => x.Responses)
                            .OrderBy(x => (x.Prompt != null) ? x.Prompt.ActiveFrom : DateTime.MaxValue)
                            .ThenBy(x => x.Mood.DisplayIndex)
                            .Select(x => new
                            {
                                moodId = x.Mood.Id,
                                promptId = (x.Prompt != null) ? x.Prompt.Id : Guid.Empty,
                                promptName = (x.Prompt != null) ? x.Prompt.Name : "Unprompted"
                            });
            // Total the responses by Mood Prompt
            var moods = (from r in respones
                         group r by new
                         {
                             promptId = r.promptId,
                             promptName = r.promptName,
                             moodId = r.moodId,
                         }
                             into gr
                             select new
                             {
                                 promptId = gr.Key.promptId,
                                 promptName = gr.Key.promptName,
                                 moodId = gr.Key.moodId,
                                 ResponseCount = gr.Count(),
                             });

            var allMoods = evnt.MoodCategories.FirstOrDefault().Moods.Select(x => new { Id = x.Id, Name = x.Name }).ToList();

            //Total report lines - Create a cartesian join - All Moods across the top - all Prompts with responses down the side
            var trl = moods.GroupBy(x => new { promptId = x.promptId, promptName = x.promptName }).Select(y => new ReportRow { Id = y.Key.promptId, Name = y.Key.promptName }).ToList();
            trl.ForEach(x => x.Cells = allMoods.Select(y => new Cell { ColId = y.Id, ColName = y.Name, Value = 0 }).ToList());
            //
            foreach (var mood in moods)
            {
                var row = trl.Where(x => x.Id == mood.promptId).FirstOrDefault();
                row.Cells.Where(x => x.ColId == mood.moodId).FirstOrDefault().Value = mood.ResponseCount;
            }

            return trl;

        }

        protected MoodSnaphotReportModel GetSnapshot(MoodCategory category, DateTime timeOfSnapshot, bool includeMoodInfo)
        {
            return GetSnapshot(category, timeOfSnapshot, includeMoodInfo, 0);
        }
        protected MoodSnaphotReportModel GetSnapshot(MoodCategory category, DateTime timeOfSnapshot, bool includeMoodInfo, int roundPercentagesToNPlaces)
        {
            var moodIsStale = category.Event.ApplicationConfig.ReportMoodIsStaleMins;
            
            timeOfSnapshot = MoodResponse.GetRoundedResponseTime(timeOfSnapshot);
            var staleTime = timeOfSnapshot.AddMinutes(-(moodIsStale));
            var snapshot = this.db.Get<Snapshot>().Where(s => s.Category.Id == category.Id && s.TimeStamp <= timeOfSnapshot && s.TimeStamp >= staleTime).OrderByDescending(s => s.TimeStamp).Select(s => s.TimeStamp).FirstOrDefault();
            if (snapshot == DateTime.MinValue) return GetDefaultSnapshot(category, timeOfSnapshot, includeMoodInfo);
            var snapshotsAt = this.db.Get<Snapshot>().Where(s => s.TimeStamp == snapshot).OrderBy(s => s.Mood.DisplayIndex);
            var allMoods = this.db.Get<Mood>().Where(m => m.Category.Id == category.Id).OrderBy(m => m.DisplayIndex);
            var totalResponses = snapshotsAt.Sum(r => r.ResponseCount);

            return new MoodSnaphotReportModel()
                             {
                                 t = snapshot,
                                 r = totalResponses,
                                 d = snapshotsAt.Select(r => new MoodSnapshotDataModel()
                                 {
                                     c = r.ResponseCount,
                                     i = r.Mood.DisplayIndex,
                                     p = Math.Round(((decimal)r.ResponseCount / (decimal)totalResponses) * 100M, roundPercentagesToNPlaces)
                                 }),
                                 m = includeMoodInfo ?
                                       allMoods.ToList().Select(m => new MoodModel()
                                       {
                                           Id = m.Id,
                                           DisplayIndex = m.DisplayIndex,
                                           DisplayColor = m.DisplayColor,
                                           Name = m.Name,
                                           MoodType = m.MoodType
                                       })
                                       : null
                             };
        }

        protected MoodSnaphotReportModel GetDefaultSnapshot(MoodCategory category, DateTime timeOfSnapshot, bool includeMoodInfo)
        {
            var allMoods = this.db.Get<Mood>().Where(m => m.Category.Id == category.Id).OrderBy(m => m.DisplayIndex);
            return new MoodSnaphotReportModel()
            {
                d = allMoods.Select(m => m.DisplayIndex).ToList().Select(m => new MoodSnapshotDataModel()
                {
                    c = 0,
                    i = m,
                    p = Math.Round(100M / (decimal)allMoods.Count())
                }).ToList(),
                m = includeMoodInfo ?
                    allMoods.ToList().Select(m => new MoodModel()
                    {
                        Id = m.Id,
                        DisplayIndex = m.DisplayIndex,
                        DisplayColor = m.DisplayColor,
                        Name = m.Name,
                        MoodType = m.MoodType
                    })
                    : null,
                r = 0,
                t = timeOfSnapshot
            };
        }

        //protected MoodSnaphotReportModel GetSnapshot(MoodCategory category, DateTime timeOfSnapshot, int moodIsStaleMins, bool includeMoodInfo)
        //{
        //    var responses = GetSnapshotResponses(category, timeOfSnapshot, moodIsStaleMins);
        //    return BuildMoodSnapshotReport(responses, category, timeOfSnapshot, includeMoodInfo);
        //}

        private MoodSnaphotReportModel BuildMoodSnapshotReportFromResponses(IEnumerable<MoodResponse> responses, MoodCategory category, DateTime timeOfSnapshot, bool includeMoodInfo)
        {

            var moods = (from r in responses
                         let rcount = responses.Count()
                         group r by new
                         {
                             Mood = r.Mood,
                             rcount = rcount
                         } into gr
                         select new
                         {
                             Mood = gr.Key.Mood,
                             ResponseCount = gr.Count(),
                             ResponsePercentage = (decimal)gr.Count() / (decimal)gr.Key.rcount * 100M
                         });

            //get stats for all he moods - if no responses at all then equally divide percentages
            var allMoods = (from cm in category.Moods
                            join m in moods on cm equals m.Mood into gm
                            from sub in gm.DefaultIfEmpty()
                            select new MoodSnapshotDataModel()
                            {
                                i = cm.DisplayIndex,
                                c = sub == null ? 0 : sub.ResponseCount,
                                p = sub == null ? (responses.Any() ? 0 : Math.Round((decimal)1 / (decimal)category.Moods.Count() * 100M)) : sub.ResponsePercentage
                            });

            return new MoodSnaphotReportModel()
            {
                d = allMoods.ToList(),
                t = timeOfSnapshot,
                r = moods.Sum(m => m.ResponseCount),
                m = includeMoodInfo ? category.Moods.Select(m => new MoodModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    DisplayColor = m.DisplayColor,
                    DisplayIndex = m.DisplayIndex,
                    MoodType = m.MoodType
                }).ToList() : null
            };

        }
    }

}
