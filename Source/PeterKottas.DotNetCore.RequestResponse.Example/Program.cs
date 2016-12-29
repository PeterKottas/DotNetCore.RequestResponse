using PeterKottas.DotNetCore.RequestResponse.Example.DTO;
using System;
using System.Threading;

namespace PeterKottas.DotNetCore.RequestResponse.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var req = new WebRequestDTO()
            {
                Username = "Peter"
            };
            //req.LogJourney();
            Console.WriteLine("Created first request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n", 
                req.OperationId, 
                req.EnterTimestamp, 
                req.Depth, 
                req.TimeTakenGlobal,
                req.TimeTakenLocal,
                string.Join("\n", req.GetJourney()),
                req.Username);
            Thread.Sleep(50);
            var plugReq = req.GetRequest<PluginRequestDTO>(operationInner =>
            {
                operationInner.UserName = req.Username;
                return operationInner;
            });
            Console.WriteLine("Created second request\nId:{0}\nTimestamp:{1}\nDepth:{2}\nTimeTakenGlobal:{3}\nTimeTakenLocal:{4}\nJourney:{5}\nValue:{6}\n\n",
                plugReq.OperationId,
                plugReq.EnterTimestamp,
                plugReq.Depth,
                plugReq.TimeTakenGlobal,
                plugReq.TimeTakenLocal,
                string.Join("\n", plugReq.GetJourney()),
                plugReq.UserName);

            Thread.Sleep(50);
            var plugResp = plugReq.GetResponse<PluginResponseDTO>(operationInner =>
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
                plugResp.Counter);
            Thread.Sleep(50);
            var webResp = plugResp.GetResponse<WebResponseDTO>(operationInner =>
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
                string.Join("\n", webResp.GetJourney()),
                webResp.Counter);
            Console.ReadKey();
        }
    }
}
