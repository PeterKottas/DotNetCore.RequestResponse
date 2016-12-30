using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.RequestResponse.Example.DTO
{
    public class IsUsernameAvaliableWebRequestDTO : CustomBaseRequestDTO
    {
        public string Username { get; set; }
    }
}
