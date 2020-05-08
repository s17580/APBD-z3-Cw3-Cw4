using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.Services;
using Cw3.ReqResp.Req;
using Cw3.ApiExceptions;


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
            try
            {
                StudyEnrollment enroll = _dbService.EnrollmentStudent(req);
                String uris = $"/api/enrollments/{enroll.IdEnrollment}";
                return Created(uris, enroll);
            }
            catch (StudiesNotFoundException)
            {
                return BadRequest("Nie można wyszukać podanych studentów");
            }
            catch (StudentAlreadyExistsException)
            {
                return BadRequest("Student o podanym numerze indeksu istnieje");
            }
        }

        [HttpPost("promotions")]
        public IActionResult PromotionStudent(PromotionStudentsReq req)
        {
            try
            {
                StudyEnrollment enroll = _dbService.PromotionStudents(req);
                String uris = $"/api/enrollments/{enroll.IdEnrollment}";
                return Created(uris, enroll);
            }
            catch (StudyEnrollmentNotFoundException)
            {
                return NotFound();
            }
        }
    }
}