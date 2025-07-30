using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Helpers
{
    public interface IResultToActionResultConverter
    {
        IActionResult Convert<T>(IResult<T> result);
    }

    public class ResultToActionResultConverter : IResultToActionResultConverter
    {
        private readonly IActionResultErrorHandler _actionResultErrorHandler;

        public ResultToActionResultConverter(IActionResultErrorHandler actionResultErrorHandler)
        {
            _actionResultErrorHandler = actionResultErrorHandler;
        }

        public IActionResult Convert<T>(IResult<T> result)
        {
            if (result.IsFailure)
                return (ActionResult)_actionResultErrorHandler.GetActionResultFromError(result.ErrorObject!);

            return new OkObjectResult(result.Value);
        }
    }
}