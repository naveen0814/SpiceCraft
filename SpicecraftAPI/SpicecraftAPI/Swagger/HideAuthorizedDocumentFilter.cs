using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace SpicecraftAPI.Swagger
{
    public class HideAuthorizedDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathsToRemove = new List<string>();

            foreach (var apiDescription in context.ApiDescriptions)
            {
                var actionAttributes = apiDescription.ActionDescriptor.EndpointMetadata;
                var hasAuthorize = actionAttributes.OfType<AuthorizeAttribute>().Any();

                if (!hasAuthorize && apiDescription.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    var controllerHasAuthorize = controllerAction.ControllerTypeInfo
                        .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                        .Any();

                    hasAuthorize = controllerHasAuthorize;
                }

                if (hasAuthorize)
                {
                    var path = "/" + apiDescription.RelativePath.TrimEnd('/');
                    path = "/" + path.TrimStart('/'); // Normalize slashes
                    pathsToRemove.Add(path);
                }
            }

            foreach (var path in pathsToRemove.Distinct())
            {
                swaggerDoc.Paths.Remove(path);
            }
        }
    }
}
