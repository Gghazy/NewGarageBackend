using Garage.Application.Abstractions;
using Garage.Domain.Clients.Entities;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Garage.Domain.Clients.ValueObjects;
using Garage.Domain.Shared.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Clients.Commands.Update;



public sealed class UpdateClientCommandHandler(
    IRepository<Client> clientRepo,
    UserManager<AppUser> userManager,
    IUnitOfWork uow)
    : IRequestHandler<UpdateClientCommand, Guid>
{
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Guid> Handle(UpdateClientCommand command, CancellationToken ct)
    {
        var r = command.Request;

        var client = await _clientRepo.QueryTracking()
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (client is null)
            throw new Exception("Client not found");

        var user = await _userManager.FindByIdAsync(client.UserId.ToString());
        if (user is null)
            throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(r.Email) &&
            !string.Equals(user.Email, r.Email, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _userManager.Users.AnyAsync(x => x.Email == r.Email, ct);
            if (exists)
                throw new Exception("Email already exists");

            user.Email = r.Email;
            user.UserName = r.Email;
            user.NormalizedEmail = r.Email.ToUpperInvariant();
            user.NormalizedUserName = r.Email.ToUpperInvariant();
            user.EmailConfirmed = true;
        }

        if (!string.IsNullOrWhiteSpace(r.PhoneNumber))
            user.PhoneNumber = r.PhoneNumber;

        var updateUserRes = await _userManager.UpdateAsync(user);
        if (!updateUserRes.Succeeded)
            throw new Exception(string.Join(", ", updateUserRes.Errors.Select(e => e.Description)));

        client.UpdateBaseData(r.NameAr, r.NameEn, r.PhoneNumber); 

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

        switch (client)
        {
            case CompanyClient company:
                {
                    var identity = new CompanyIdentity(
                        commercialRegister: r.CommercialRegister!,
                        taxNumber: r.TaxNumber!
                    );
                    company.UpdateCompanyInfo(identity, address);
                    break;
                }

            case IndividualClient individual:
                {
                    individual.UpdatePersonalInfo(address);
                    break;
                }

            default:
                throw new Exception("Invalid client type");
        }

        await _uow.SaveChangesAsync(ct);
        return client.Id;
    }
}

