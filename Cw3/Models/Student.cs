using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public string BirthDate { get; set; }
        public StudyEnrollment Enrollment { get; set; }
        //Zestaw 7
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; }

    }

    }

