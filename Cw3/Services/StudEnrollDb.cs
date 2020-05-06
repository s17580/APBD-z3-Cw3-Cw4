using Cw3.Models;
using System;
using System.Collections.Generic;
using Cw3.DTOs.Req;
using Cw3.DTOs.Resp;

namespace Cw3.Services
{
    public class StudEnrollDb : IStudentsDal
    {

        private static IEnumerable<Student> _studentsdb = new List<Student>
        {
            new Student{FirstName="Monika", LastName="Dudzik", BirthDate="1980-05-21", NameOfStudy="Kultura Japonii", Semester=2},
            new Student{FirstName="Halina", LastName="Kiepska", BirthDate="1984-09-10", NameOfStudy="Kultura Japonii", Semester=5},
            new Student{FirstName="Krzysztof", LastName="Cieniuch", BirthDate="1997-07-15", NameOfStudy="Sztuka nowych mediów", Semester=3},
            new Student{FirstName="Kamil", LastName="Nosel", BirthDate="1999-06-18", NameOfStudy="Sztuka nowych mediów", Semester=1},
            new Student{FirstName="Wiesław", LastName="Byczek", BirthDate="1991-03-12", NameOfStudy="Sztuka nowych mediów", Semester=4}
        };

        public IEnumerable<Student> GetStudents()
        {
            return _studentsdb;
        }

        public StudyEnrollment GetStudentEnrollments(string IndexNumber)
        {
            return new StudyEnrollment { IdEnrollment = 2, Semester = 2, IdStudy = 1, StartDate = "2020-03-05" };
        }

        public EnrollmentStudentResp EnrollmentStudent(EnrollmentStudentReq req)
        {
            throw new NotImplementedException();
        }

        public void PromotionStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }
    }
    /*
    private static IEnumerable<StudyEnrollment> _enrollmentsdb = new List<StudyEnrollment>
    {
        new StudyEnrollment{IdEnrollment=2, Semester=2, NameOfStudy="Kultura Japonii", StartDate="2019-10-02"},
        new StudyEnrollment{IdEnrollment=1, Semester=5, NameOfStudy="Kultura Japonii", StartDate="2019-03-07"},
        new StudyEnrollment{IdEnrollment=3, Semester=3, NameOfStudy="Sztuka nowych mediów", StartDate="2019-10-02"},
        new StudyEnrollment{IdEnrollment=4, Semester=1, NameOfStudy="Sztuka nowych mediów", StartDate="2020-03-05"},
        new StudyEnrollment{IdEnrollment=5, Semester=4, NameOfStudy="Sztuka nowych mediów", StartDate="2020-03-05"},

    };

    public IEnumerable<Student> GetStudents()
    {
        return _studentsdb;
    }

    public IEnumerable<StudyEnrollment> GetStudentEnrollments(string IndexNumber)
    {
        return _enrollmentsdb;
    }

}

*/

}
