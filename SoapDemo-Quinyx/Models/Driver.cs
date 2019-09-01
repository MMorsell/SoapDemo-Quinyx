using System;
using System.Collections.Generic;
using System.Text;

namespace SoapDemo_Quinyx.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int BadgeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string CategoryName { get; set; }
    }
}
