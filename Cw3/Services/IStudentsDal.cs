using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentsDal
    {
        public IEnumerable<Student> GetStudents();

        public IEnumerable<StudyEnrollment> GetStudentEnrollments(string IndexNumber);
        //public Student CreateStudent();
        //public Student UpdateStudentById();
        //public Student DeleteStudentById();
    }
}
