using System;
using System.Linq;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;
using Cw3.ReqResp.Req;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{

    
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private IStudentsDal _dbService;
        private IConfiguration _configuration;
        public StudentsController(IStudentsDal dbService, IConfiguration configuration)

        {

            _dbService = dbService;
            _configuration = configuration;
        }
        //Zestaw 7
        private string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string HashPasswordWithSalt(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Encoding.UTF8.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                );
            return Convert.ToBase64String(valueBytes);
        }

        private bool ValidatePassword(string password, string salt, string hash)
            => HashPasswordWithSalt(password, salt) == hash;


        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LogReq req)
        {
            var stud = _dbService.GetStudentByIndexNumber(req.IndexNumber);
            if (stud == null)
            {
                return NotFound("Nie znaleziono studenta");
            }

            if (stud.Salt == "")
            {
                if (stud.Password != req.Password)
                {
                    return StatusCode(401, "Nieprawidłowe hasło");
                }
            }
            else
            {
                if (!ValidatePassword(req.Password, stud.Salt, stud.Password))
                {
                    return StatusCode(401, "Nieprawidłowe hasło (posolone)");
                }
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, req.IndexNumber),
                new Claim(ClaimTypes.Role, stud.Role),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            var refreshToken = Guid.NewGuid();

            _dbService.SetStudentRefreshToken(new SetStudRefTokReq
            {
                IndexNumber = req.IndexNumber,
                RefreshToken = refreshToken.ToString()
            });

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public IActionResult RefreshTocken(RefTokReq req)
        {
            var stud = _dbService.GetStudentByRefreshToken(req.RefreshToken);
            if (stud == null)
            {
                return StatusCode(401, "Nieprawidłowy token odświeżania");
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, stud.IndexNumber),
                new Claim(ClaimTypes.Role, stud.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            var newRefreshToken = Guid.NewGuid();
            var updated = _dbService.SetStudentRefreshToken(new SetStudRefTokReq
            {
                IndexNumber = stud.IndexNumber,
                RefreshToken = newRefreshToken.ToString()
            });

            if (updated != 1)
            {
                return StatusCode(500, "Coś poszło nie tak");
            }

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = newRefreshToken
            });
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
        public IActionResult CreateStudent([FromBody] Student stud)
            {

            //    student.IndexNumber = $"s{new Random().Next(1, 20000)}";

            return Ok(stud);
                

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
