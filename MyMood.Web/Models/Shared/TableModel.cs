using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMood.Web.Models
{
    public class ReportRow
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Cell> Cells { get; set; }
    }

    public class Cell
    {
        public string ColName { get; set; }
        public Guid? ColId { get; set; }
        public double Value { get; set; }
        public string ValueString { get; set; }
    }

}
