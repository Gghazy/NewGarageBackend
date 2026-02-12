using Garage.Contracts.Auth;
using Garage.Application.Common;
using MediatR;
namespace Garage.Application.Auth.Queries.Login;
public record LoginQuery(LoginRequest Request) : IRequest<Result<LoginResponse>>;

