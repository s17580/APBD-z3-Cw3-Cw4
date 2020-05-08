using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Cw3.ReqResp.Req;
using Microsoft.AspNetCore.Http;
using Cw3.ApiExceptions;

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

                            IndexNumber = dr["IndexNumber"].ToString(),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            BirthDate = dr["BirthDate"].ToString(),
                            Enrollment = new StudyEnrollment
                            {
                                IdEnrollment = int.Parse(dr["IdEnrollment"].ToString()),
                                Semester = int.Parse(dr["Semester"].ToString()),
                                StartDate = dr["StartDate"].ToString(),
                                Study = new Stud
                                {
                                    IdStudy = int.Parse(dr["IdStudy"].ToString()),
                                    Name = dr["Name"].ToString()
                                }
                            },

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
                            Study = new Stud
                            {
                                IdStudy = int.Parse(dr["IdStudy"].ToString()),
                                Name = dr["StudiesName"].ToString()
                            },
                            StartDate = dr["StartDate"].ToString()
                        });
                    }

                }

            }

            return list.First();

        }
        private void CreateEnrollment(SqlCommand cmd, int idStudy)
        {
            cmd.CommandText = @"INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, 
                              GETDATE()) VALUES ((SELECT 1+MAX(IdEnrollment) FROM Enrollment), @Semester, @IdStudy)";
            cmd.Parameters.AddWithValue("Semester", 1);
            cmd.Parameters.AddWithValue("IdStudy", idStudy);
            cmd.ExecuteNonQuery();
        }

        private StudyEnrollment FindEnrollmentByIdStudies(SqlCommand cmd, int idStudy, int semester)
        {
            cmd.CommandText = @"SELECT * FROM Enrollment e INNER JOIN Studies s ON e.IdStudy = s.IdStudy 
                              WHERE e.Semester = @Semester AND e.IdStudy = @IdStudy";

            cmd.Parameters.AddWithValue("IdStudy", idStudy);
            cmd.Parameters.AddWithValue("Semester", semester);
            var dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new StudyEnrollmentNotFoundException();
            }

            var enroll = new StudyEnrollment
            {
                IdEnrollment = (int)dr["IdEnrollment"],
                Semester = (int)dr["Semester"],
                StartDate = dr["StartDate"].ToString(),
                Study = new Stud
                {
                    IdStudy = idStudy,
                    Name = dr["Name"].ToString()
                },
            };

            dr.Close();
            return enroll;
        }

        public StudyEnrollment EnrollmentStudent(EnrollmentStudentReq req)
        {
            var st = new Student();
            st.FirstName = req.FirstName;

            using (var con = new SqlConnection(SqlConnect))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.Transaction = tran;
                    com.CommandText = "SELECT IdStudy FROM studies WHERE name = @name";
                    com.Parameters.AddWithValue("name", req.Studies);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        throw new StudiesNotFoundException();
                    }

                    int idstudy = (int)dr["IdStudy"];
                    dr.Close();

                    StudyEnrollment enroll;

                    try
                    {
                        enroll = FindEnrollmentByIdStudies(com, idstudy, 1);
                    }
                    catch (StudiesNotFoundException)
                    {
                        CreateEnrollment(com, idstudy);
                        enroll = FindEnrollmentByIdStudies(com, idstudy, 1);
                    }

                    com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", req.IndexNumber);
                    dr = com.ExecuteReader();

                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        throw new StudentAlreadyExistsException();
                    }

                    dr.Close();

                    // Dodanie nowego studenta
                    com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) " +
                                        "VALUES (@Index, @Fname, @LName, @Bdate, @IdEnrollment)";
                    com.Parameters.AddWithValue("Index", req.IndexNumber);
                    com.Parameters.AddWithValue("Fname", req.FirstName);
                    com.Parameters.AddWithValue("Lname", req.LastName);
                    com.Parameters.AddWithValue("Bdate", req.BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", enroll.IdEnrollment);
                    com.ExecuteNonQuery();

                    tran.Commit();

                    return enroll;
                }
                catch (SqlException)
                {
                    tran.Rollback();
                    throw new Exception("Coś poszło nie tak");
                }
            }
        }

        private StudyEnrollment FindEnrollmentBySemesterAndStudies(SqlCommand cmd, string studyName, int semester)
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"SELECT * FROM Enrollment E RIGHT JOIN Studies S ON (S.IdStudy = E.IdStudy AND S.Name = @Name) WHERE Semester = @Semester";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Name", studyName);
            cmd.Parameters.AddWithValue("Semester", semester);
            var dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new StudiesNotFoundException();
            }

            var enroll = new StudyEnrollment
            {
                IdEnrollment = (int)dr["IdEnrollment"],
                Semester = (int)dr["Semester"],
                StartDate = dr["StartDate"].ToString(),
                Study = new Stud
                {
                    IdStudy = (int)dr["IdStudy"],
                    Name = dr["Name"].ToString()
                },
            };

            dr.Close();

            return enroll;
        }

        public StudyEnrollment PromotionStudents(PromotionStudentsReq req)
        {
            using (var conn = new SqlConnection(SqlConnect))
            using (var cmd = new SqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                FindEnrollmentBySemesterAndStudies(cmd, req.Studies, req.Semester);

                cmd.CommandText = @"PromotionStudents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Studies", req.Studies);
                cmd.Parameters.AddWithValue("@Semester", req.Semester);
                cmd.Parameters.AddWithValue("@NewIdEnrollment", 0);
                cmd.ExecuteNonQuery();

                int newSemester = req.Semester + 1;

                return FindEnrollmentBySemesterAndStudies(cmd, req.Studies, newSemester);
            }
        }

        public bool IsStudentExist(string indexNumber)
        {
            var sql = @"SELECT 1 FROM Student WHERE IndexNumber = @IndexNumber";
            using (var con = new SqlConnection(SqlConnect))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("IndexNumber", indexNumber);
                var dr = cmd.ExecuteReader();
                return dr.Read();

            }

        }
    }
}
