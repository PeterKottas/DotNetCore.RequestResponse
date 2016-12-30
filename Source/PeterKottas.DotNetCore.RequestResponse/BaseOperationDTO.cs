using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.RequestResponse
{
    public abstract class BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>
        where BASE_REQUEST : BASE
        where BASE_RESPONSE : BASE
        where BASE : BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>, new()
    {
        public Guid OperationId { get; set; }
        public long EnterTimestamp { get; set; }
        public int Depth { get; set; }
        private List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>> wholeJourney;
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
            wholeJourney = new List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>>();
            TimeTakenGlobal = 0;
            TimeTakenLocal = 0;
        }

        protected virtual RESPONSE_CONCRETE GetResponseCustom<RESPONSE_CONCRETE>(RESPONSE_CONCRETE response) where RESPONSE_CONCRETE : BASE_RESPONSE, new()
        {
            return response;
        }

        protected virtual REQUEST_CONCRETE GetRequestCustom<REQUEST_CONCRETE>(REQUEST_CONCRETE request) where REQUEST_CONCRETE : BASE_REQUEST, new()
        {
            return request;
        }

        protected virtual BASE_CLASS GetOperationCustom<BASE_CLASS>(BASE_CLASS operation)
            where BASE_CLASS : BASE
        {
            return operation;
        }

        private BASE_CLASS GetOperation<BASE_CLASS>(BASE_CLASS operation)
            where BASE_CLASS : BASE
        {
            operation.EnterTimestamp = this.EnterTimestamp;
            operation.logJourney = this.logJourney;
            operation.wholeJourney = this.wholeJourney;
            operation.OperationId = this.OperationId;
            var currentTimestamp = GetCurrentUnixTimestampMillis();
            operation.TimeTakenGlobal = currentTimestamp - this.EnterTimestamp;
            operation.TimeTakenLocal = currentTimestamp - (this.EnterTimestamp + this.TimeTakenGlobal);
            operation = GetOperationCustom(operation);
            return operation;
        }

        public REQUEST GetRequest<REQUEST>(Func<REQUEST, REQUEST> initFunction = null) where REQUEST : BASE_REQUEST, new()
        {
            var request = new REQUEST();
            if (this is BASE_REQUEST)
            {
                request.Depth = this.Depth + 1;
            }
            else
            {
                request.Depth = this.Depth;
            }
            request = GetOperation(request);
            request = GetRequestCustom(request);
            request = initFunction(request);
            if (logJourney)
            {
                wholeJourney.Add(this);
            }
            return request;
        }

        public RESPONSE GetResponse<RESPONSE>(Func<RESPONSE, RESPONSE> initFunction = null) where RESPONSE : BASE_RESPONSE, new()
        {
            var response = new RESPONSE();
            if (this is BASE_RESPONSE)
            {
                response.Depth = this.Depth - 1;
            }
            else
            {
                response.Depth = this.Depth;
            }
            response = GetOperation(response);
            response = GetResponseCustom(response);
            response = initFunction(response);
            if (logJourney)
            {
                wholeJourney.Add(this);
            }
            return response;
        }

        public void LogJourney(bool log = true)
        {
            logJourney = log;
            if (logJourney)
            {
                wholeJourney.Add(this);
            }
        }

        public List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>> GetJourney()
        {
            return wholeJourney;
        }
    }
}
