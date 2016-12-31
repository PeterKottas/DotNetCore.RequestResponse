using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PeterKottas.DotNetCore.RequestResponse;
using PeterKottas.DotNetCore.RequestResponse.Base;

namespace PeterKottas.DotNetCore.RequestResponse.Extensions.SerializedJourney
{
    public static class JourneyExtensions
    {
        public static List<string> GetJourneyJson<BASE_REQUEST, BASE_RESPONSE, BASE>(this BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE> operation)
            where BASE_REQUEST : BASE
            where BASE_RESPONSE : BASE
            where BASE : BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>, new()
        {
            return operation.GetJourney().Select(operationInList => Newtonsoft.Json.JsonConvert.SerializeObject(operationInList)).ToList();
        }

        public static string GetJourneyJsonFlat<BASE_REQUEST, BASE_RESPONSE, BASE>(this BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE> operation)
            where BASE_REQUEST : BASE
            where BASE_RESPONSE : BASE
            where BASE : BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>, new()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(operation.GetJourney());
        }
    }
}
