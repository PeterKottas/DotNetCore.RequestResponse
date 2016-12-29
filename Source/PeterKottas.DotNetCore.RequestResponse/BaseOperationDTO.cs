using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.RequestResponse
{
    public abstract class BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE> where BASE_REQUEST: BaseRequestDTO<BASE_REQUEST, BASE_RESPONSE> where BASE_RESPONSE: BaseResponseDTO<BASE_REQUEST, BASE_RESPONSE>
    {
        public Guid OperationId { get; set; }
        public long EnterTimestamp { get; set; }
        public int Depth { get; set; }
        private List<string> wholeJourney;
        private bool logJourney;
        public long TimeTakenGlobal;
        public long TimeTakenLocal;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            DateTime localDateTime, univDateTime;
            localDateTime = DateTime.Now;
            univDateTime = localDateTime.ToUniversalTime();
            return (long)(univDateTime - UnixEpoch).TotalMilliseconds;
        }

        public BaseOperationDTO(bool logJourney = false)
        {
            EnterTimestamp = GetCurrentUnixTimestampMillis();
            Depth = 1;
            OperationId = Guid.NewGuid();
            wholeJourney = new List<string>();
            TimeTakenGlobal = 0;
            TimeTakenLocal = 0;
        }

        protected abstract REQUEST_CONCRETE GetRequestInner<REQUEST_CONCRETE>() where REQUEST_CONCRETE : BASE_REQUEST, new();

        protected abstract RESPONSE_CONCRETE GetResponseInner<RESPONSE_CONCRETE>() where RESPONSE_CONCRETE : BASE_RESPONSE, new();

        public REQUEST GetRequest<REQUEST>(Func<REQUEST, REQUEST> initFunction = null) where REQUEST : BASE_REQUEST, new()
        {
            var req = GetRequestInner<REQUEST>();
            req.EnterTimestamp = this.EnterTimestamp;
            req.logJourney = this.logJourney;
            req.wholeJourney = this.wholeJourney;
            req.OperationId = this.OperationId;
            req = initFunction(req);
            var currentTimestamp = GetCurrentUnixTimestampMillis();
            req.TimeTakenGlobal = currentTimestamp - this.EnterTimestamp;
            req.TimeTakenLocal = currentTimestamp - (this.EnterTimestamp + this.TimeTakenGlobal);
            if (logJourney)
            {
                wholeJourney.Add(Newtonsoft.Json.JsonConvert.SerializeObject(req));
            }
            return req;
        }

        public RESPONSE GetResponse<RESPONSE>(Func<RESPONSE, RESPONSE> initFunction = null) where RESPONSE : BASE_RESPONSE, new()
        {
            var req = GetResponseInner<RESPONSE>();
            req.EnterTimestamp = this.EnterTimestamp;
            req.OperationId = this.OperationId;
            req.logJourney = this.logJourney;
            req.wholeJourney = this.wholeJourney;
            req = initFunction(req);
            var currentTimestamp = GetCurrentUnixTimestampMillis();
            req.TimeTakenGlobal = currentTimestamp - this.EnterTimestamp;
            req.TimeTakenLocal = currentTimestamp - (this.EnterTimestamp+this.TimeTakenGlobal);
            if (logJourney)
            {
                wholeJourney.Add(Newtonsoft.Json.JsonConvert.SerializeObject(req));
            }
            return req;
        }

        public void LogJourney(bool log = true)
        {
            logJourney = log;
            if (logJourney)
            {
                wholeJourney.Add(Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
        }

        public List<string> GetJourney()
        {
            return wholeJourney;
        }
    }
}
