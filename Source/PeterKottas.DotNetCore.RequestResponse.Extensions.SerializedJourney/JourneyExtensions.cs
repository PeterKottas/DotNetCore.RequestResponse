using System.Collections.Generic;
using System.Linq;
using PeterKottas.DotNetCore.RequestResponse.Base;

namespace PeterKottas.DotNetCore.RequestResponse.Extensions.SerializedJourney
{
    public static class JourneyExtensions
    {
        public static List<string> GetJourneyJson<TBaseRequest, TBaseResponse, TBase>(this BaseOperationDTO<TBaseRequest, TBaseResponse, TBase> operation)
            where TBaseRequest : TBase
            where TBaseResponse : TBase
            where TBase : BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>, new()
        {
            return operation.GetJourney().Select(Newtonsoft.Json.JsonConvert.SerializeObject).ToList();
        }

        public static string GetJourneyJsonFlat<TBaseRequest, TBaseResponse, TBase>(this BaseOperationDTO<TBaseRequest, TBaseResponse, TBase> operation)
            where TBaseRequest : TBase
            where TBaseResponse : TBase
            where TBase : BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>, new()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(operation.GetJourney());
        }
    }
}
