using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoTf.Aic.Extensions;

public class LocalAuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!IsLocalDevice(context.HttpContext.Connection.RemoteIpAddress.ToString()))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsLocalDevice(string address)
    {
#if DEBUG
        return true;
#endif
        return address.StartsWith("192.168.0.");
    }
}