using Garage.Contracts.Auth;
using Garage.Application.Common;
using MediatR;
namespace Garage.Application.Auth.Commands.RegisterUser;
public record RegisterUserCommand(RegisterUserRequest Request) : IRequest<Result<Guid?>>;

