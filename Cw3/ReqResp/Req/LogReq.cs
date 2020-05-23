using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;




namespace Cw3.ReqResp.Req
{
    //Zestaw 7
    public class LogReq
    {
        [Required(ErrorMessage = "Wymagane jest podanie twojego numeru indexu")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie twojego hasła")]
        public string Password { get; set; }
    }
}
