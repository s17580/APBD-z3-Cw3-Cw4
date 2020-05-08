using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cw3.ReqResp.Req
{
    public class PromotionStudentsReq
    {
        [Required(ErrorMessage = "Wymagane jest podanie nazwy studiów")]
        public string Studies { get; set; }

        [Required(ErrorMessage = "Wymagane jest podanie semestru")]
        public int Semester { get; set; }
    }
}
