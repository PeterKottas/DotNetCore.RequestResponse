using PeterKottas.DotNetCore.RequestResponse.Base;

namespace PeterKottas.DotNetCore.RequestResponse
{
    /// <summary>
    /// Operation that inherits from this library's base class. This class is ready to be used in your project. Optionally, 
    /// you can implement you own base class instead. Check out tutorial on how to do that on 
    /// https://github.com/PeterKottas/DotNetCore.WindowsService
    /// </summary>
    public abstract class OperationDTO : BaseOperationDTO<RequestDTO, ResponseDTO, OperationDTO>
    {
    }
}
