using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.RequestResponse
{
    public class ResponseDTO : BaseResponseDTO<RequestDTO, ResponseDTO>
    {
        public int Counter; 

        protected override RESPONSE_CONCRETE GetResponseOuter<RESPONSE_CONCRETE>()
        {
            var req = new RESPONSE_CONCRETE();
            req.Counter = this.Counter + 1;
            return req;
        }
    }
}
