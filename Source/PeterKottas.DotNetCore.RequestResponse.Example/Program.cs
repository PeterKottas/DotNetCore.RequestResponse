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
            var req = new IsUsernameAvaliableWebRequestDTO()
            {
                Username = "Peter"
            };
            req.LogJourney();
            Console.WriteLine("Created first request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                req.OperationId,
                req.EnterTimestamp,
                req.Depth,
                req.TimeTakenGlobal,
                req.TimeTakenLocal,
                string.Join("\n", req.GetJourney()),
                req.Username);
            Thread.Sleep(50);
            var plugReq = req.GetRequest<IsUsernameAvaliableRequestDTO>(operationInner =>
            {
                operationInner.Username = req.Username;
                return operationInner;
            });
            Console.WriteLine("Created second request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                plugReq.OperationId,
                plugReq.EnterTimestamp,
                plugReq.Depth,
                plugReq.TimeTakenGlobal,
                plugReq.TimeTakenLocal,
                string.Join("\n", plugReq.GetJourney()),
                plugReq.Username);

            Thread.Sleep(50);
            var plugResp = plugReq.GetResponse<IsUsernameAvaliableResponseDTO>(operationInner =>
            {
                operationInner.IsAvaliable = true;
                return operationInner;
            });
            Console.WriteLine("Created first response\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                plugResp.OperationId,
                plugResp.EnterTimestamp,
                plugResp.Depth,
                plugResp.TimeTakenGlobal,
                plugResp.TimeTakenLocal,
                string.Join("\n", plugResp.GetJourney()),
                plugResp.IsAvaliable
                );
            Thread.Sleep(50);
            var webResp = plugResp.GetResponse<IsUsernameAvaliableWebResponseDTO>(operationInner =>
            {
                operationInner.IsAvaliable = plugResp.IsAvaliable;
                return operationInner;
            });
            Console.WriteLine("Created second response\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                webResp.OperationId,
                webResp.EnterTimestamp,
                webResp.Depth,
                webResp.TimeTakenGlobal,
                webResp.TimeTakenLocal,
                string.Join("\n", webResp.GetJourneyJsonFlat()),
                webResp.IsAvaliable
                );
            Console.ReadKey();
        }
    }
}
