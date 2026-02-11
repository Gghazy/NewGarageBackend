using Cashif.Application.Lookup.Commands.Create;
using Cashif.Application.Lookup.Commands.Update;
using Cashif.Application.Lookup.Queries.GetAll;
using Cashif.Application.Lookup.Queries.GetAllPagination;
using Cashif.Contracts.Common;
using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;
using System.Reflection;

namespace Cashif.Api
{
    public static class LookupHandlersRegistration
    {
        public static IServiceCollection AddLookupHandlersForAllEntities(
            this IServiceCollection services,
            Assembly domainAssemblyWithLookupEntities)
        {
            var lookupEntityTypes = domainAssemblyWithLookupEntities
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(LookupBase).IsAssignableFrom(t))
                .ToList();

            foreach (var entityType in lookupEntityTypes)
            {
                // Pagination
                RegisterClosedHandler(
                    services,
                    requestOpenGeneric: typeof(GetAllPaginationQuery<>),
                    responseType: typeof(QueryResult<LookupDto>),
                    handlerOpenGeneric: typeof(GetAllPaginationHandler<>),
                    entityType: entityType
                );

                // GetAll
                RegisterClosedHandler(
                    services,
                    requestOpenGeneric: typeof(GetAllLookupQuery<>),
                    responseType: typeof(List<LookupDto>),
                    handlerOpenGeneric: typeof(GetAllLookupHandler<>),
                    entityType: entityType
                );

                // Create (عندك بيرجع Guid في الكنترولر)
                RegisterClosedHandler(
                    services,
                    requestOpenGeneric: typeof(CreateLookupCommand<>),
                    responseType: typeof(Guid),
                    handlerOpenGeneric: typeof(CreateLookupHandler<>),
                    entityType: entityType
                );

                // Update (بيرجع bool)
                RegisterClosedHandler(
                    services,
                    requestOpenGeneric: typeof(UpdateLookupCommand<>),
                    responseType: typeof(bool),
                    handlerOpenGeneric: typeof(UpdateLookupHandler<>),
                    entityType: entityType
                );
            }

            return services;
        }

        private static void RegisterClosedHandler(
            IServiceCollection services,
            Type requestOpenGeneric,
            Type responseType,
            Type handlerOpenGeneric,
            Type entityType)
        {
            var requestType = requestOpenGeneric.MakeGenericType(entityType);
            var handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var handlerImpl = handlerOpenGeneric.MakeGenericType(entityType);

            services.AddTransient(handlerInterface, handlerImpl);
        }
    }
}
