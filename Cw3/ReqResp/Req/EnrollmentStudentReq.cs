﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.ReqResp.Req
{
    public class EnrollmentStudentReq
    {
        [Required(ErrorMessage = "Wymagane jest podanie numeru indeksu")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie imienia")]
        [MaxLength(15)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie nazwiska")]
        [MaxLength(15)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie daty urodzenia")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie kierunku studiów")]
        public string Studies { get; set; }
    }
}

