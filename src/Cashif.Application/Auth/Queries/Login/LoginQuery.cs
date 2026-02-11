using Cashif.Contracts.Auth;
using Cashif.Application.Common;
using MediatR;
namespace Cashif.Application.Auth.Queries.Login;
public record LoginQuery(LoginRequest Request) : IRequest<Result<LoginResponse>>;
