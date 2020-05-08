using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Models
{
    public class StudyEnrollment

    {
        public int IdEnrollment { get; set; }
        public int Semester     { get; set; }
        public Stud Study { get; set; }
        public string StartDate { get; set; }


    }
}
