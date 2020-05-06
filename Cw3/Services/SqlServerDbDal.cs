using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Req;
using Cw3.DTOs.Resp;
using Microsoft.AspNetCore.Http;

namespace Cw3.Services
{
    public class SqlServerDbDal : IStudentsDal
    {

        private string SqlConnect = "Data Source=db-mssql;Initial" +
            " Catalog = s17580; Integrated Security = True";

        public IEnumerable<Student> GetStudents()
        {

            var list = new List<Student>();
            using (var cli = new SqlConnection(SqlConnect))

            {
                using (var comm = new SqlCommand())
                {

                    comm.Connection = cli;
                    comm.CommandText = "SELECT * FROM Student, Enrollment, Studies WHERE Student.IdEnrollment" +
                        " = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy";


                    cli.Open();

                    var dr = comm.ExecuteReader();

                    while (dr.Read())
                    {

                        list.Add(new Student

                        {


                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            BirthDate = dr["BirthDate"].ToString(),
                            NameOfStudy = dr["Name"].ToString(),
                            Semester = int.Parse(dr["Semester"].ToString()),

                        });
                    }

                }
            }

            return list;
        }

        public StudyEnrollment GetStudentEnrollments(string IndexNumber)

        {
            var list = new List<StudyEnrollment>();
            using (var cli = new SqlConnection(SqlConnect))

            {
                using (var comm = new SqlCommand())

                {

                    comm.Connection = cli;
                    comm.CommandText = "SELECT * FROM Student, Enrollment, Studies " +
                        "WHERE Student.IdEnrollment = Enrollment.IdEnrollment " +
                        "AND Enrollment.IdStudy = Studies.IdStudy AND Student.IndexNumber = @indexNumber";
                    comm.Parameters.AddWithValue("indexNumber", IndexNumber);


                    cli.Open();

                    var dr = comm.ExecuteReader();

                    while (dr.Read())

                    {


                        list.Add(new StudyEnrollment

                        { 

                        IdEnrollment = int.Parse(dr["IdEnrollment"].ToString()),
                        Semester = int.Parse(dr["Semester"].ToString()),
                        IdStudy = int.Parse(dr["IdStudy"].ToString()),
                      //NameOfStudy = dr["Name"].ToString(),
                        StartDate = dr["StartDate"].ToString(),
                    });
                    
                }
                
            }
            
        }

         return list.First();   

            }
        public EnrollmentStudentResp EnrollmentStudent(EnrollmentStudentReq req)
        {
            var stud = new Student();
            stud.FirstName = req.FirstName;

            using (var connect = new SqlConnection(SqlConnect))
            using (var command = new SqlCommand())
            {
                command.Connection = connect;

                connect.Open();
                var transac = connect.BeginTransaction();

                try
                {
                    command.Transaction = transac;
                    command.CommandText = "SELECT IdStudy FROM studies WHERE name = @name";
                    command.Parameters.AddWithValue("name", req.Studies);

                    var dr = command.ExecuteReader();
                    if (!dr.Read())
                    {
                        return new EnrollmentStudentResp
                        {
                            success = false,
                            errMessage = "Nie ma takich studiow",
                            statCode = StatusCodes.Status400BadRequest
                        };
                    }
                    int idstudy = (int)dr["IdStudy"];

                    command.CommandText = "SELECT TOP 1 * FROM enrollment WHERE IdStudy = @idstudy AND Semester = 1 ORDER BY StartDate DESC";
                    command.Parameters.AddWithValue("idstudy", idstudy);

                    int enrollmentId;
                    dr.Close();
                    dr = command.ExecuteReader();
                    if (!dr.Read())
                    {
                        command.CommandText = "SELECT MAX(IdEnrollment) AS id FROM Enrollment";
                        dr = command.ExecuteReader();
                        int nextId = ((int)dr["id"]) + 1;

                        command.CommandText = "INSERT INTO Enrollment VALUES(@nextId, 1, @idstudy, GETDATE())";
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows == 1)
                        {
                            enrollmentId = nextId;
                        }
                        else
                        {
                            transac.Rollback();

                            return new EnrollmentStudentResp
                            {
                                success = false,
                                errMessage = "Cos poszlo nie tak",
                                statCode = StatusCodes.Status500InternalServerError
                            };
                        }
                    }
                    else
                    {
                        enrollmentId = (int)dr["IdEnrollment"];
                    }

                    dr.Close();

                    command.CommandText = "SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
                    command.Parameters.AddWithValue("IndexNumber", req.IndexNumber);
                    dr = command.ExecuteReader();

                    if (dr.Read())
                    {
                        dr.Close();
                        transac.Rollback();

                        return new EnrollmentStudentResp
                        {
                            success = false,
                            errMessage = "Student o tym numerze indeksu juz istnieje",
                            statCode = StatusCodes.Status400BadRequest
                        };
                    }

                    dr.Close();

                    // Dodawanie studenta
                    command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@Index, @Fname, @LName, @Bdate, @IdEnrollment)";
                    command.Parameters.AddWithValue("Index", req.IndexNumber);
                    command.Parameters.AddWithValue("Fname", req.FirstName);
                    command.Parameters.AddWithValue("Lname", req.LastName);
                    command.Parameters.AddWithValue("Bdate", req.BirthDate);
                    command.Parameters.AddWithValue("IdEnrollment", enrollmentId);
                    command.ExecuteNonQuery();

                    transac.Commit();


                    command.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment = @enrollmentId";
                    command.Parameters.AddWithValue("enrollmentId", enrollmentId);
                    dr = command.ExecuteReader();

                    if (dr.Read())
                    {
                        return new EnrollmentStudentResp
                        {
                            success = true,
                            statCode = StatusCodes.Status201Created,
                            IdEnrollment = (int)dr["IdEnrollment"],
                            IdStudy = (int)dr["IdStudy"],
                            Semester = (int)dr["Semester"],
                            StartDate = (DateTime)dr["StartDate"]
                        };
                    }
                    else
                    {
                        return new EnrollmentStudentResp
                        {
                            success = false,
                            errMessage = "Cos poszlo nie tak",
                            statCode = StatusCodes.Status500InternalServerError
                        };
                    }
                }
                catch (SqlException exc)
                {
                    transac.Rollback();

                    return new EnrollmentStudentResp
                    {
                        success = false,
                        errMessage = exc.Message,
                        statCode = StatusCodes.Status500InternalServerError
                    };
                }
            }

        }

        public void PromotionStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }
    }
}