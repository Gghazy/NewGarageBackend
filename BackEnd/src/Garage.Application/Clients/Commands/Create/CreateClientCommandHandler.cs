using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Clients;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Clients.Enums;
using Garage.Domain.Clients.ValueObjects;
using Garage.Domain.Shared.ValueObjects;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Clients.Commands.Create;

public sealed class CreateClientCommandHandler : BaseCommandHandler<CreateClientCommand, Guid>
{
    private readonly IRepository<Client> _clientRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClientCommandHandler(
        IRepository<Client> clientRepository,
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _clientRepository = clientRepository;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CreateClientCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // Check if email already exists
        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var emailExists = await _userManager.Users.AnyAsync(x => x.Email == req.Email, ct);
            if (emailExists)
                return Fail("Email already exists");
        }

        // Create user
        var user = new AppUser
        {
            UserName = req.Email,
            Email = req.Email,
            EmailConfirmed = true,
            PhoneNumber = req.PhoneNumber
        };

        var createUserResult = await _userManager.CreateAsync(user, "123456");
        if (!createUserResult.Succeeded)
            return Fail(string.Join(", ", createUserResult.Errors.Select(e => e.Description)));

        try
        {
            // Create client based on type
            var clientType = ClientType.FromName(req.Type);
            
            var client = clientType switch
            {
                var t when t == ClientType.Company => CreateCompany(user.Id, req),
                var t when t == ClientType.Individual => CreateIndividual(user.Id, req),
                _ => throw new InvalidOperationException("Invalid client type")
            };

            // Persist to database
            await _clientRepository.AddAsync(client, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Ok(client.Id);
        }
        catch (Exception ex)
        {
            // If client creation fails, delete the user
            await _userManager.DeleteAsync(user);
            return Fail($"Failed to create client: {ex.Message}");
        }
    }

    private static Client CreateIndividual(Guid userId, CreateClientRequest request)
    {

        return new IndividualClient(
            userId: userId,
            nameAr: request.NameAr,
            nameEn: request.NameEn,
            phoneNumber: request.PhoneNumber,
            resourceId: request.ResourceId,
            address: request.Address
        );
    }

    private static Client CreateCompany(Guid userId, CreateClientRequest request)
    {
        var identity = new CompanyIdentity(
            commercialRegister: request.CommercialRegister!,
            taxNumber: request.TaxNumber!
        );

        var address = new Address(
            streetName: request.StreetName!,
            additionalStreetName: request.AdditionalStreetName,
            cityName: request.CityName!,
            postalZone: request.PostalZone!,
            countrySubentity: request.CountrySubentity,
            countryCode: request.CountryCode!,
            buildingNumber: request.BuildingNumber!,
            citySubdivisionName: request.CitySubdivisionName
        );

        return new CompanyClient(
            userId: userId,
            nameAr: request.NameAr,
            nameEn: request.NameEn,
            phoneNumber: request.PhoneNumber,
            identity: identity,
            address: address,
            resourceId: request.ResourceId
        );
    }
}

