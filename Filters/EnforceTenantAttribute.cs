// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using VidizmoBackend.Data;
// using VidizmoBackend.Models;

// namespace VidizmoBackend.Filters
// {
//     public class EnforceTenantAttribute : Attribute, IAsyncActionFilter
//     {
//         private readonly string[] _routeParams;
//         private readonly Type[] _entityTypes;

//         public EnforceTenantAttribute(string[] routeParams, Type[] entityTypes)
//         {
//             if (routeParams.Length != entityTypes.Length)
//                 throw new ArgumentException("Route params and entity types count must match");

//             _routeParams = routeParams;
//             _entityTypes = entityTypes;
//         }

//         public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//         {
//             var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
//             var tenantId = context.HttpContext.User.FindFirst("OrganizationId")?.Value;

//             if (tenantId == null)
//             {
//                 context.Result = new UnauthorizedResult();
//                 return;
//             }

//             for (int i = 0; i < _routeParams.Length; i++)
//             {
//                 var paramName = _routeParams[i];
//                 var entityType = _entityTypes[i];

//                 if (!context.ActionArguments.TryGetValue(paramName, out var idObj) || idObj is not int id)
//                 {
//                     context.Result = new BadRequestObjectResult($"Invalid route parameter '{paramName}'");
//                     return;
//                 }

//                 object? entity = entityType.Name switch
//                 {
//                     nameof(User) => await dbContext.Users.FindAsync(id),
//                     nameof(Role) => await dbContext.Roles.FindAsync(id),
//                     nameof(Organization) => await dbContext.Organizations.FindAsync(id),
//                     nameof(Video) => await dbContext.Videos.FindAsync(id),
//                     _ => null
//                 };

//                 if (entity == null || GetEntityTenantId(entity) != tenantId)
//                 {
//                     context.Result = new ForbidResult(); // Access denied
//                     return;
//                 }
//             }

//             await next();
//         }

//         private string? GetEntityTenantId(object entity)
//         {
//             return entity switch
//             {
//                 User u => u.OrganizationId.ToString(),

//                 Video v => v.OrganizationId.ToString(),

//                 Group g => g.OrganizationId.ToString(),

//                 Role r => r.CreatedByUser?.OrganizationId.ToString(), // indirect mapping

//                 _ => null
//             };
//         }

// }

// }