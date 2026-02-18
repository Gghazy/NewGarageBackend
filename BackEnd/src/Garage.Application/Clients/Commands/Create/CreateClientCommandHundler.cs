using Garage.Application.Abstractions;
using Garage.Contracts.Clients;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Clients.Enums;
using Garage.Domain.Clients.ValueObjects;
using Garage.Domain.Shared.ValueObjects;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Clients.Commands.Create;
public sealed class CreateClientCommandHandler(
    IRepository<Client> clientRepo,
    UserManager<AppUser> userManager,
    IUnitOfWork uow)
    : IRequestHandler<CreateClientCommand, Guid>
{
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Guid> Handle(CreateClientCommand command, CancellationToken ct)
    {
        var r = command.Request;

        if (!string.IsNullOrWhiteSpace(r.Email))
        {
            var exists = await _userManager.Users.AnyAsync(x => x.Email == r.Email, ct);
            if (exists)
                throw new Exception("Email already exists");
        }

        var user = new AppUser
        {
            UserName = r.Email,
            Email = r.Email,
            EmailConfirmed = true,
            PhoneNumber = r.PhoneNumber
        };

        var createUserRes = await _userManager.CreateAsync(user, "123456");
        if (!createUserRes.Succeeded)
            throw new Exception(string.Join(", ", createUserRes.Errors.Select(e => e.Description)));

        var type = ClientType.FromValue(r.Type);

        var client = type switch
        {
            var t when t == ClientType.Company => CreateCompany(user.Id, r),
            var t when t == ClientType.Individual => CreateIndividual(user.Id, r),
            _ => throw new Exception("Invalid client type")
        };

        await _clientRepo.AddAsync(client, ct);
        await _uow.SaveChangesAsync(ct);

        return client.Id;
    }

    private static Client CreateIndividual(Guid userId, CreateClientRequest r)
    {
        var address = new Address(
            streetName: r.StreetName!,             
            additionalStreetName: r.AdditionalStreetName,
            cityName: r.CityName!,
            postalZone: r.PostalZone!,
            countrySubentity: r.CountrySubentity,
            countryCode: r.CountryCode!,
            buildingNumber: r.BuildingNumber!,
            citySubdivisionName: r.CitySubdivisionName
        );

        var individual = new IndividualClient(
            userId: userId,
            nameAr: r.NameAr,
            nameEn: r.NameEn,
            phoneNumber: r.PhoneNumber,
            address: address
        );

        return individual;
    }

    private static Client CreateCompany(Guid userId, CreateClientRequest r)
    {
        var identity = new CompanyIdentity(
            commercialRegister: r.CommercialRegister!,
            taxNumber: r.TaxNumber!
        );

        var address = new Address(
            streetName: r.StreetName!,
            additionalStreetName: r.AdditionalStreetName,
            cityName: r.CityName!,
            postalZone: r.PostalZone!,
            countrySubentity: r.CountrySubentity,
            countryCode: r.CountryCode!,
            buildingNumber: r.BuildingNumber!,
            citySubdivisionName: r.CitySubdivisionName
        );

        var company = new CompanyClient(
            userId: userId,
            nameAr: r.NameAr,
            nameEn: r.NameEn,
            phoneNumber: r.PhoneNumber,
            identity: identity,
            address: address
        );

        return company;
    }
}

