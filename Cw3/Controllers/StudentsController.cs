﻿using System;
using System.Linq;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{

    
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private IStudentsDal _dbService;
        public StudentsController(IStudentsDal dbService)

        {

            _dbService = dbService;
        }
        
        [HttpGet]
        public IActionResult GetStudents([FromServices]IStudentsDal serv, [FromQuery]string orderBy)


        {

   
                if (orderBy == "lastname")
                {
                   
                    return Ok(_dbService.GetStudents().OrderBy(stu =>stu.LastName));

                }
            return Ok(_dbService.GetStudents());
        }


        [HttpGet("{id}")]

        public IActionResult GetStudent([FromRoute]int id)

        {
            if (id == 1)

            {

                return Ok("Jarosław");
            }

            return NotFound("Nie znaleziono studenta");

        }

        [HttpPost]
        public IActionResult CreateStudent([FromBody] Student student)
            {

            //    student.IndexNumber = $"s{new Random().Next(1, 20000)}";

            return Ok(student);
                

            }
            //Cw3

        [HttpPut("{id}")]
        public IActionResult UpdateStudentById([FromRoute]int id)
            {
            return Ok("Aktualizacja ukończona");
            }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudentById([FromRoute]int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpGet("{IndexNumber}/enrollments")]
        public IActionResult GetStudentEnrollments([FromServices]IStudentsDal serv, [FromRoute]string IndexNumber)
        {
            return Ok(_dbService.GetStudentEnrollments(IndexNumber));
        }

            }
    }
