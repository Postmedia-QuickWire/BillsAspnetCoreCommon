using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Common.Classes
{

	// need to lock down the swagger controller
	//https://github.com/domaindrivendev/Swashbuckle/issues/384
	public static class SwaggerAuthorizeExtensions
	{
        // the redirectUrl WORKS for a refresh
        // BUT the swagger page doesn't redirect when an api is selected from the dropdown
        // so a big blank 401 page will do.
        // this shows how to send parameters to the middleware though
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder) //, string redirectUrl)
		{
			return builder.UseMiddleware<SwaggerAuthorizedMiddleware>(); //redirectUrl);
		}
	}
	public class SwaggerAuthorizedMiddleware
	{
		private readonly RequestDelegate _next;
        //private readonly string _redirectUrl = "/login";
		public SwaggerAuthorizedMiddleware(RequestDelegate next) //, string redirectUrl)
		{
			_next = next;
            //_redirectUrl = redirectUrl;
        }

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Path.StartsWithSegments("/swagger"))
			{
				context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
				context.Response.Headers[HeaderNames.Expires] = "0";
				context.Response.Headers[HeaderNames.Pragma] = "no-cache";
				if (!context.User.Identity.IsAuthenticated)
				{
                    //context.Response.Redirect(_redirectUrl);
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					return;
				}
			}

			await _next.Invoke(context);
		}
	}
}
