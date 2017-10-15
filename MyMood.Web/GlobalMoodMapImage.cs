using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using MyMood.Domain;


namespace MyMood.Web
{
    public class GlobalMoodMapImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime ReportStart { get; set; }
        public DateTime ReportEnd { get; set; }
        public float Tension { get; set; }
        public bool ShowDataPoints { get; set; }

        int _dataPointWidth = 10;

        List<MoodDataSet> _moods = new List<MoodDataSet>();

        public void AddSnapshot(DateTime timeStamp, IEnumerable<MoodMapItem> moods)
        {
            var totalResponses = moods.Select(m => m.ResponseCount).Sum();
            decimal cumulativePercentage = 0;
           

            foreach(var mood in moods)
            {
               
                var responseCount = mood.ResponseCount;
                //decimal responsePercentage = responseCount / totalResponses * 100M;
                decimal responsePercentage = mood.ResponsePercentage;

                var moodData = _moods.FirstOrDefault(m => m.Mood.Name.Equals(mood.Name, StringComparison.InvariantCultureIgnoreCase));
                if (moodData == null)
                {
                    moodData = new MoodDataSet()
                    {
                        Mood = new MoodItem()
                        {
                            DisplayColor = mood.DisplayColor,
                            DisplayIndex = mood.DisplayIndex,
                            MoodType = mood.MoodType,
                            Name = mood.Name
                        },
                        DataPoints = new List<DataPoint>()
                    };
                    _moods.Add(moodData);
                }
                moodData.DataPoints.Add(new DataPoint() { TimeStamp = timeStamp, ResponseCount = responseCount, ResponsePercentage = responsePercentage, CumulativePercentage = cumulativePercentage });

                cumulativePercentage += responsePercentage;
            }
        }


        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);
           
            var positiveMoods = from m in _moods where m.Mood.MoodType == MoodType.Positive select m;
            var neutralMoods = from m in _moods where m.Mood.MoodType == MoodType.Neutral select m;
            var negativeMoods = from m in _moods where m.Mood.MoodType == MoodType.Negative select m;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (MoodDataSet mood in positiveMoods)
                {
                    AddMoodLayer(g, mood, ShowDataPoints);
                }

                foreach (MoodDataSet mood in neutralMoods)
                {
                    AddMoodLayer(g, mood, ShowDataPoints);
                }
                

                foreach (MoodDataSet mood in negativeMoods)
                {
                    AddMoodLayer(g, mood, ShowDataPoints);
                }

                if (neutralMoods.Count() > 0)
                {
                    AddSeperator(g, neutralMoods.First());
                }

                if (negativeMoods.Count() > 0)
                {
                    AddSeperator(g, negativeMoods.First());
                }
            }
            return bmp;
        }

        private void AddSeperator(Graphics g, MoodDataSet mood)
        {
            Pen pen = new Pen(Color.White, 5);
            GraphicsPath path = new GraphicsPath();
            var dataPoints = mood.DataPoints.Select(dp => DataPointToPoint(dp)).ToList();
            path.AddCurve(dataPoints.ToArray(), this.Tension);
            g.DrawPath(pen, path);
        }

        private void AddMoodLayer(Graphics g, MoodDataSet mood, bool showDataPoints)
        {
            PointF bottomRight = new PointF(this.Width, this.Height);
            PointF bottomLeft = new PointF(0, this.Height);
            var color = mood.Mood.DisplayColor;
            Pen pen = new Pen(color, 2);
            var dataPoints = mood.DataPoints.Select(dp => DataPointToPoint(dp)).ToList();
            var last = dataPoints[dataPoints.Count - 1];

            GraphicsPath path = new GraphicsPath();
            path.AddCurve(dataPoints.ToArray(), this.Tension);
            last.X = bottomRight.X;
            path.AddLine(last, bottomRight);
            path.AddLine(bottomRight, bottomLeft);
            path.AddLine(bottomLeft, dataPoints.First());
            g.FillPath(new SolidBrush(color), path);

            if (ShowDataPoints)
            {
                var dpW = _dataPointWidth / 2;
                foreach (var dp in dataPoints)
                {
                    g.FillEllipse(new SolidBrush(Color.Black), dp.X - dpW, dp.Y - dpW, _dataPointWidth, _dataPointWidth);
                }
            }
        }

        public class MoodMapItem : MoodItem
        {
            public int ResponseCount { get; set; }
            public decimal ResponsePercentage { get; set; }
        }

        public class MoodItem
        {
            public string Name { get; set; }
            public int DisplayIndex { get; set; }
            public Color DisplayColor { get; set; }
            public MoodType MoodType { get; set; }
        }

        private PointF DataPointToPoint(DataPoint dp)
        {
            var x = (decimal)dp.TimeStamp.Subtract(this.ReportStart).TotalMilliseconds / (decimal)this.ReportEnd.Subtract(this.ReportStart).TotalMilliseconds * (decimal)this.Width;
            var y = (decimal)dp.CumulativePercentage / 100M * (decimal)this.Height;
            return new PointF((float)x, (float)y);
        }

        private class MoodDataSet 
        {
            public MoodItem Mood { get; set; }
            public List<DataPoint> DataPoints { get; set; }
        }

        private class DataPoint
        {
            public DateTime TimeStamp { get; set; }
            public int ResponseCount { get; set; }
            public decimal ResponsePercentage { get; set; }
            public decimal CumulativePercentage { get; set; }

        }
    }


}