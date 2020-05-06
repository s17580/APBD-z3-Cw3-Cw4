using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.Services;
using Cw3.DTOs.Req;
using Cw3.DTOs.Resp;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class StudyEnrollmentsController : ControllerBase
    {
        private IStudentsDal _dbService;

        public StudyEnrollmentsController(IStudentsDal dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult EnrollmentStudent(EnrollmentStudentReq req)
        {
            var resp = _dbService.EnrollmentStudent(req);
            if (resp.success)
            {
                var enroll = new StudyEnrollment
                {
                    IdEnrollment = resp.IdEnrollment,
                    Semester = resp.Semester,
                    IdStudy = resp.IdStudy,
                    StartDate = resp.StartDate.ToShortDateString()
                };
                return Ok(enroll);
            }
            else
            {
                return StatusCode(resp.statCode, resp.errMessage);
            }
        }
    }
}