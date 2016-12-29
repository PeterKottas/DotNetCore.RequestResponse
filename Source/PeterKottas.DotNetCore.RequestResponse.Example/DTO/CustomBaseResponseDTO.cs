using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.RequestResponse.Example.DTO
{
    public class CustomBaseResponseDTO : BaseResponseDTO<CustomBaseRequestDTO, CustomBaseResponseDTO>
    {
        public int Counter = 0;

        protected override RESPONSE_CONCRETE GetResponseOuter<RESPONSE_CONCRETE>()
        {
            var resp = new RESPONSE_CONCRETE();
            resp.Counter = this.Counter + 1;
            return resp;
        }
    }
}
