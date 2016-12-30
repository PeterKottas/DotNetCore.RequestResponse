using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.RequestResponse.Example.DTO
{
    public class CustomBaseOperationDTO : BaseOperationDTO<CustomBaseRequestDTO, CustomBaseResponseDTO, CustomBaseOperationDTO>
    {
        public int Counter { get; set; }

        public CustomBaseOperationDTO()
        {
            Counter = 1;
        }

        protected override sealed BASE_CLASS GetOperationCustom<BASE_CLASS>(BASE_CLASS operation)
        {
            operation.Counter = this.Counter + 1;
            return operation;
        }
    }
}
