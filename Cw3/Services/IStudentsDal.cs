using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.ReqResp.Req;


namespace Cw3.Services
{
    public interface IStudentsDal
    {
        public IEnumerable<Student> GetStudents();
        public StudyEnrollment GetStudentEnrollments(string IndexNumber);
        public StudyEnrollment EnrollmentStudent(EnrollmentStudentReq req);
        public StudyEnrollment PromotionStudents(PromotionStudentsReq req);
        public bool IsStudentExist(string indexNumber);
        public Student GetStudentByIndexNumber(string indexNumber);
        public Student GetStudentByRefreshToken(string refTok);
        public int SetStudentRefreshToken(SetStudRefTokReq req);
    }
        //public Student CreateStudent();
        //public Student UpdateStudentById();
        //public Student DeleteStudentById();
}
