using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.ReqResp.Resp
{
    public class Respons
    {
        public bool success { get; set; }
        public string errMessage { get; set; }
        public int statCode { get; set; }
    }
}
