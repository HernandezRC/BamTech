using Microsoft.AspNetCore.Mvc;

namespace StargateAPI.Controllers
{
    public static class ControllerBaseExtensions
    {

        public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response)
        {
            var httpResponse = new ObjectResult(response)
            {
                //Response can absolutely be null here despite Visual Studio saying otherwise.
                //Case in point, the unit test that was failing because it would break here because response was null.
                StatusCode = response?.ResponseCode
            };
            return httpResponse;
        }
    }
}