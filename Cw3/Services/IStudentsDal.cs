using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Req;
using Cw3.DTOs.Resp;

namespace Cw3.Services
{
    public interface IStudentsDal
    {
        public IEnumerable<Student> GetStudents();

        public StudyEnrollment GetStudentEnrollments(string IndexNumber);
            public EnrollmentStudentResp EnrollmentStudent(EnrollmentStudentReq req);
            public void PromotionStudents(int semester, string studies);
        }
        //public Student CreateStudent();
        //public Student UpdateStudentById();
        //public Student DeleteStudentById();
}
