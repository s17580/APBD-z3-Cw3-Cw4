using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
                using (var com = new SqlCommand())
                {

                    com.Connection = cli;
                    com.CommandText = "SELECT * FROM Student, Enrollment, Studies WHERE Student.IdEnrollment" +
                        " = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy";


                    cli.Open();

                    var dr = com.ExecuteReader();

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

        public IEnumerable<StudyEnrollment> GetStudentEnrollments(string IndexNumber)

        {
            var list = new List<StudyEnrollment>();
            using (var cli = new SqlConnection(SqlConnect))

            {
                using (var com = new SqlCommand())

                {

                    com.Connection = cli;
                    com.CommandText = "SELECT * FROM Student, Enrollment, Studies " +
                        "WHERE Student.IdEnrollment = Enrollment.IdEnrollment " +
                        "AND Enrollment.IdStudy = Studies.IdStudy AND Student.IndexNumber = @indexNumber";
                    com.Parameters.AddWithValue("indexNumber", IndexNumber);


                    cli.Open();

                    var dr = com.ExecuteReader();

                    while (dr.Read())

                    {


                        list.Add(new StudyEnrollment

                        { 

                        IdEnrollment = int.Parse(dr["IdEnrollment"].ToString()),
                        Semester = int.Parse(dr["Semester"].ToString()),
                        NameOfStudy = dr["Name"].ToString(),
                        StartDate = dr["StartDate"].ToString(),
                        
                        
                    });
                    
                }
                
            }
            
        }

         return list;   

            }
        }
}