using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.RequestResponse.Base
{
    /// <summary>
    /// Base class for both request and response. Shared functionality of both resides here
    /// </summary>
    /// <typeparam name="BASE_REQUEST"></typeparam>
    /// <typeparam name="BASE_RESPONSE"></typeparam>
    /// <typeparam name="BASE"></typeparam>
    public abstract class BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>
        where BASE_REQUEST : BASE
        where BASE_RESPONSE : BASE
        where BASE : BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>
    {
        private List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>> wholeJourney;
        private bool logJourney;

        /// <summary>
        /// Operation id, identifier that uniquely defines whole chain of requests/responses
        /// </summary>
        public Guid OperationId { get; private set; }

        /// <summary>
        /// Timestamp of when the first request/response was created
        /// </summary>
        public long EntryTimestamp { get; private set; }

        /// <summary>
        /// Depth of the chain
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Time taken from first request/response in chain in ms
        /// </summary>
        public long TimeTakenGlobal { get; private set; }

        /// <summary>
        /// Time taken since the last request/response in chain in ms
        /// </summary>
        public long TimeTakenLocal { get; private set; }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long GetCurrentUnixTimestampMillis()
        {
            DateTime localDateTime, univDateTime;
            localDateTime = DateTime.Now;
            univDateTime = localDateTime.ToUniversalTime();
            return (long)(univDateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Constructor that populates all the necesary data for the first time
        /// </summary>
        /// <param name="logJourney"></param>
        public BaseOperationDTO(bool logJourney = false)
        {
            EntryTimestamp = GetCurrentUnixTimestampMillis();
            Depth = 1;
            OperationId = Guid.NewGuid();
            wholeJourney = new List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>>();
            TimeTakenGlobal = 0;
            TimeTakenLocal = 0;
        }

        /// <summary>
        /// Override this method in request/response DTO to provide custom functionality
        /// </summary>
        /// <typeparam name="RESPONSE_CONCRETE"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual RESPONSE_CONCRETE GetResponseCustom<RESPONSE_CONCRETE>(RESPONSE_CONCRETE response) where RESPONSE_CONCRETE : BASE_RESPONSE, new()
        {
            return response;
        }

        /// <summary>
        /// Override this method in request/response DTO to provide custom functionality
        /// </summary>
        /// <typeparam name="REQUEST_CONCRETE"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual REQUEST_CONCRETE GetRequestCustom<REQUEST_CONCRETE>(REQUEST_CONCRETE request) where REQUEST_CONCRETE : BASE_REQUEST, new()
        {
            return request;
        }

        /// <summary>
        /// Override this method in your custom operation DTO to provide custom functionality when creating new requests ad responses
        /// </summary>
        /// <typeparam name="BASE_CLASS"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected virtual BASE_CLASS GetOperationCustom<BASE_CLASS>(BASE_CLASS operation)
            where BASE_CLASS : BASE
        {
            return operation;
        }

        private BASE_CLASS GetOperation<BASE_CLASS>(BASE_CLASS operation)
            where BASE_CLASS : BASE
        {
            operation.EntryTimestamp = this.EntryTimestamp;
            operation.logJourney = this.logJourney;
            operation.wholeJourney = this.wholeJourney;
            operation.OperationId = this.OperationId;
            var currentTimestamp = GetCurrentUnixTimestampMillis();
            operation.TimeTakenGlobal = currentTimestamp - this.EntryTimestamp;
            operation.TimeTakenLocal = currentTimestamp - (this.EntryTimestamp + this.TimeTakenGlobal);
            operation = GetOperationCustom(operation);
            return operation;
        }

        /// <summary>
        /// Creates request of a given type
        /// </summary>
        /// <typeparam name="REQUEST"></typeparam>
        /// <param name="initFunction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates response of a given type
        /// </summary>
        /// <typeparam name="RESPONSE"></typeparam>
        /// <param name="initFunction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Enables/disables logging of the whole journey
        /// </summary>
        /// <param name="log"></param>
        public void LogJourney(bool log = true)
        {
            logJourney = log;
            if (logJourney)
            {
                wholeJourney.Add(this);
            }
        }

        /// <summary>
        /// Returns the whole journey (chain of requests and responses)
        /// </summary>
        /// <returns></returns>
        public List<BaseOperationDTO<BASE_REQUEST, BASE_RESPONSE, BASE>> GetJourney()
        {
            return wholeJourney;
        }
    }
}
