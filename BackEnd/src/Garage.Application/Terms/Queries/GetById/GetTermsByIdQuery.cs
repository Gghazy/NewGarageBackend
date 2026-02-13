using Garage.Contracts.Services;
using Garage.Contracts.Terms;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Terms.Queries.GetById
{
    public sealed record GetTermsByIdQuery() : IRequest<TermsDto>;
}
