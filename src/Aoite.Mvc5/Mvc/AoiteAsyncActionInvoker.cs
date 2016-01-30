using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Async;

namespace System.Web.Mvc
{
    class AoiteAsyncActionInvoker : AsyncControllerActionInvoker
    {
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            if(!(actionReturnValue is ActionResult))
            {
                var controller = controllerContext.Controller;
                if(actionReturnValue is Result)
                {
                    if(actionReturnValue == Result.Successfully)
                    {
                        actionReturnValue = controller.Success();
                    }
                    else
                    {
                        var result = actionReturnValue as IValueResult;
                        if(result.IsSucceed)
                        {
                            actionReturnValue = controller.Success(result.GetValue());
                        }
                        else actionReturnValue = controller.Faild(result.Message, result.Status);
                    }
                }
                else
                {
                    actionReturnValue = controller.Success(actionReturnValue);
                }
            }
            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }
    }
}
