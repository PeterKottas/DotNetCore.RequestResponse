using PeterKottas.DotNetCore.RequestResponse.Base;

namespace PeterKottas.DotNetCore.RequestResponse.Example.DTO
{
    public class CustomBaseOperationDTO : BaseOperationDTO<CustomBaseRequestDTO, CustomBaseResponseDTO, CustomBaseOperationDTO>
    {
        public int Counter { get; set; }

        public CustomBaseOperationDTO()
        {
            Counter = 1;
        }

        protected sealed override TBaseClass GetOperationCustom<TBaseClass>(TBaseClass operation)
        {
            operation.Counter = Counter + 1;

            return operation;
        }
    }
}
