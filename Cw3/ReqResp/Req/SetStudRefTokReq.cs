using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cw3.ReqResp.Req
{
    public class SetStudRefTokReq
    {
        [Required(ErrorMessage = "Musisz podac numer indeksu")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
        public string RefreshToken { get; set; }
    }
}
