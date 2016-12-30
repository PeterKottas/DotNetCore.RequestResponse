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
        protected virtual REQUEST_CONCRETE GetRequestOuter<REQUEST_CONCRETE>() where REQUEST_CONCRETE : BASE_REQUEST, new()
        {
            var request = new REQUEST_CONCRETE();
            return request;
        }

        protected sealed override REQUEST_CONCRETE GetRequestInner<REQUEST_CONCRETE>()
        {
            var request = GetRequestOuter<REQUEST_CONCRETE>();
            request.Depth = this.Depth + 1;
            return request;
        }

        protected virtual RESPONSE_CONCRETE GetResponseOuter<RESPONSE_CONCRETE>() where RESPONSE_CONCRETE : BASE_RESPONSE, new()
        {
            var request = new RESPONSE_CONCRETE();
            return request;
        }

        protected sealed override RESPONSE_CONCRETE GetResponseInner<RESPONSE_CONCRETE>()
        {
            var request = GetResponseOuter<RESPONSE_CONCRETE>();
            request.Depth = this.Depth;
            return request;
        }
    }
}
