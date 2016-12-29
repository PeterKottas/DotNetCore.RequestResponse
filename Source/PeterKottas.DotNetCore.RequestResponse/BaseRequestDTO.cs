using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.RequestResponse
{
    public abstract class BaseRequestDTO<BASE_REQUEST, BASE_RESPONSE> : BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE> 
        where BASE_REQUEST: BaseRequestDTO<BASE_REQUEST, BASE_RESPONSE> 
        where BASE_RESPONSE : BaseResponseDTO<BASE_REQUEST, BASE_RESPONSE>
    {
        protected override REQUEST_CONCRETE GetRequestInner<REQUEST_CONCRETE>()
        {
            var request = new REQUEST_CONCRETE();
            request.Depth = this.Depth + 1;
            return request;
        }

        protected override RESPONSE_CONCRETE GetResponseInner<RESPONSE_CONCRETE>()
        {
            var request = new RESPONSE_CONCRETE();
            request.Depth = this.Depth;
            return request;
        }
    }
}
