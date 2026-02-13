using Garage.Contracts.Common;
using Garage.Contracts.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Services.Queries.GetAll;
    public record GetAllServiceQuery() : IRequest<List<ServiceDto>>;


