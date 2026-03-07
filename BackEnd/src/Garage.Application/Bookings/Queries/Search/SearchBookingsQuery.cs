using Garage.Contracts.Bookings;
using Garage.Contracts.Common;
using MediatR;

namespace Garage.Application.Bookings.Queries.Search;

public record SearchBookingsQuery(SearchCriteria Search) : IRequest<QueryResult<BookingDto>>;
