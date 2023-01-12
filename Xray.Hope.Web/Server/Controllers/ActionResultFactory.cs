using Microsoft.AspNetCore.Mvc;
using Xray.Hope.Web.Shared;

namespace Xray.Hope.Web.Server.Controllers
{
    public static class ActionResultFactory
    {
        public static ActionResult Create<TOut>(Result result, Func<Result, TOut> successFunc)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(Result<TOut>.Create(successFunc(result), result));
            }

            return new BadRequestObjectResult(result);
        }

        public static ActionResult Create<TValue, TOut>(Result<TValue> result, Func<Result<TValue>, TOut> successFunc = null)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(Result<TOut>.Create(successFunc(result), result));
            }

            return new BadRequestObjectResult(result);
        }
    }
}