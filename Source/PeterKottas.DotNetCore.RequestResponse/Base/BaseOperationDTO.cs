using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.RequestResponse.Base
{
    /// <summary>
    /// Base class for both request and response. Shared functionality of both resides here
    /// </summary>
    /// <typeparam name="TBaseRequest"></typeparam>
    /// <typeparam name="TBaseResponse"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    public abstract class BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>
        where TBaseRequest : TBase
        where TBaseResponse : TBase
        where TBase : BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>
    {
        private List<BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>> _wholeJourney;
        private bool _logJourney;

        /// <summary>
        /// Constructor that populates all the necessary data for the first time
        /// </summary>
        protected BaseOperationDTO()
        {
            EntryTimestamp = GetCurrentUnixTimestampMillis();
            Depth = 1;
            OperationId = Guid.NewGuid();
            _wholeJourney = new List<BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>>();
            TimeTakenGlobal = 0;
            TimeTakenLocal = 0;
        }

        /// <summary>
        /// Operation id, identifier that uniquely defines whole chain of requests/responses
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Timestamp of when the first request/response was created
        /// </summary>
        public long EntryTimestamp { get; set; }

        /// <summary>
        /// Depth of the chain
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Time taken from first request/response in chain in ms
        /// </summary>
        public long TimeTakenGlobal { get; set; }

        /// <summary>
        /// Time taken since the last request/response in chain in ms
        /// </summary>
        public long TimeTakenLocal { get; set; }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long GetCurrentUnixTimestampMillis()
        {
            DateTime localDateTime, univDateTime;
            localDateTime = DateTime.Now;
            univDateTime = localDateTime.ToUniversalTime();

            return (long)(univDateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Override this method in request/response DTO to provide custom functionality
        /// </summary>
        /// <typeparam name="TResponseConcrete"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual TResponseConcrete GetResponseCustom<TResponseConcrete>(TResponseConcrete response)
            where TResponseConcrete : TBaseResponse, new()
        {
            return response;
        }

        /// <summary>
        /// Override this method in request/response DTO to provide custom functionality
        /// </summary>
        /// <typeparam name="TRequestConcrete"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual TRequestConcrete GetRequestCustom<TRequestConcrete>(TRequestConcrete request)
            where TRequestConcrete : TBaseRequest, new()
        {
            return request;
        }

        /// <summary>
        /// Override this method in your custom operation DTO to provide custom functionality when creating new requests ad responses
        /// </summary>
        /// <typeparam name="TBaseClass"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected virtual TBaseClass GetOperationCustom<TBaseClass>(TBaseClass operation)
            where TBaseClass : TBase
        {
            return operation;
        }

        private TBaseClass GetOperation<TBaseClass>(TBaseClass operation)
            where TBaseClass : TBase
        {
            operation.EntryTimestamp = EntryTimestamp;
            operation._logJourney = _logJourney;
            operation._wholeJourney = _wholeJourney;
            operation.OperationId = OperationId;

            var currentTimestamp = GetCurrentUnixTimestampMillis();

            operation.TimeTakenGlobal = currentTimestamp - EntryTimestamp;
            operation.TimeTakenLocal = currentTimestamp - (EntryTimestamp + TimeTakenGlobal);
            operation = GetOperationCustom(operation);

            return operation;
        }

        /// <summary>
        /// Creates request of a given type
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="initFunction"></param>
        /// <returns></returns>
        public TRequest GetRequest<TRequest>(Func<TRequest, TRequest> initFunction = null)
            where TRequest : TBaseRequest, new()
        {
            var request = new TRequest();

            if (this is TBaseRequest)
            {
                request.Depth = Depth + 1;
            }
            else
            {
                request.Depth = Depth;
            }

            request = GetOperation(request);
            request = GetRequestCustom(request);

            if (initFunction != null)
                request = initFunction(request);

            if (_logJourney)
            {
                _wholeJourney.Add(this);
            }

            return request;
        }

        /// <summary>
        /// Creates response of a given type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="initFunction"></param>
        /// <returns></returns>
        public TResponse GetResponse<TResponse>(Func<TResponse, TResponse> initFunction = null)
            where TResponse : TBaseResponse, new()
        {
            var response = new TResponse();

            if (this is TBaseResponse)
            {
                response.Depth = Depth - 1;
            }
            else
            {
                response.Depth = Depth;
            }

            response = GetOperation(response);
            response = GetResponseCustom(response);

            if (initFunction != null)
                response = initFunction(response);

            if (_logJourney)
            {
                _wholeJourney.Add(this);
            }

            return response;
        }

        /// <summary>
        /// Enables/disables logging of the whole journey
        /// </summary>
        /// <param name="log"></param>
        public void LogJourney(bool log = true)
        {
            _logJourney = log;

            if (_logJourney)
            {
                _wholeJourney.Add(this);
            }
        }

        /// <summary>
        /// Returns the whole journey (chain of requests and responses)
        /// </summary>
        /// <returns></returns>
        public List<BaseOperationDTO<TBaseRequest, TBaseResponse, TBase>> GetJourney()
        {
            return _wholeJourney;
        }
    }
}
