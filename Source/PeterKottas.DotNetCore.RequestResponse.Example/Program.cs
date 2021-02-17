using PeterKottas.DotNetCore.RequestResponse.Example.DTO;
using System;
using System.Threading;
using PeterKottas.DotNetCore.RequestResponse.Extensions.SerializedJourney;

namespace PeterKottas.DotNetCore.RequestResponse.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var req = new IsUsernameAvailableWebRequestDTO
            {
                Username = "Peter"
            };

            req.LogJourney();
            Console.WriteLine("Created first request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                req.OperationId,
                req.EntryTimestamp,
                req.Depth,
                req.TimeTakenGlobal,
                req.TimeTakenLocal,
                string.Join("\n", req.GetJourney()),
                req.Username);

            Thread.Sleep(50);

            var plugReq = req.GetRequest<IsUsernameAvailableRequestDTO>(operationInner =>
            {
                operationInner.Username = req.Username;

                return operationInner;
            });

            Console.WriteLine("Created second request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                plugReq.OperationId,
                plugReq.EntryTimestamp,
                plugReq.Depth,
                plugReq.TimeTakenGlobal,
                plugReq.TimeTakenLocal,
                string.Join("\n", plugReq.GetJourney()),
                plugReq.Username);

            Thread.Sleep(50);

            var plugResp = plugReq.GetResponse<IsUsernameAvailableResponseDTO>(operationInner =>
            {
                operationInner.IsAvailable = true;
                return operationInner;
            });

            Console.WriteLine("Created first response\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                plugResp.OperationId,
                plugResp.EntryTimestamp,
                plugResp.Depth,
                plugResp.TimeTakenGlobal,
                plugResp.TimeTakenLocal,
                string.Join("\n", plugResp.GetJourney()),
                plugResp.IsAvailable
                );

            Thread.Sleep(50);

            var webResp = plugResp.GetResponse<IsUsernameAvailableWebResponseDTO>(operationInner =>
            {
                operationInner.IsAvailable = plugResp.IsAvailable;

                return operationInner;
            });

            Console.WriteLine("Created second response\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                webResp.OperationId,
                webResp.EntryTimestamp,
                webResp.Depth,
                webResp.TimeTakenGlobal,
                webResp.TimeTakenLocal,
                string.Join("\n", webResp.GetJourneyJsonFlat()),
                webResp.IsAvailable
                );

            Console.ReadKey();
        }
    }
}
