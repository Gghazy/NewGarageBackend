using Cashif.Contracts.Auth;
using Cashif.Application.Common;
using MediatR;
namespace Cashif.Application.Auth.Commands.RegisterUser;
public record RegisterUserCommand(RegisterUserRequest Request) : IRequest<Result<Guid?>>;
