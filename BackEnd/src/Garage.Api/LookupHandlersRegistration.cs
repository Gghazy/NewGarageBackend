using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.Common.Primitives;
using MediatR;
using System.Reflection;

namespace Garage.Api
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

                // Create (???? ????? Guid ?? ?????????)
                RegisterClosedHandler(
                    services,
                    requestOpenGeneric: typeof(CreateLookupCommand<>),
                    responseType: typeof(Guid),
                    handlerOpenGeneric: typeof(CreateLookupHandler<>),
                    entityType: entityType
                );

                // Update (????? bool)
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

