using FluentValidation;
using Garage.Domain.Clients.Enums;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Clients.Commands.Create;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator(UserManager<AppUser> userManager)
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().EmailAddress()
            .MustAsync(async (email, ct) =>
            {
                var u = await userManager.FindByEmailAsync(email);
                return u is null;
            })
            .WithMessage("Email already exists");

        RuleFor(x => x.Request.Type)
            .IsInEnum();

        RuleFor(x => x.Request.NameAr)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Request.NameEn)
            .NotEmpty().MaximumLength(200);

        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty().MaximumLength(20);

        When(x => x.Request.Type == ClientType.Individual.Value, () =>
        {

            RuleFor(x => x.Request.StreetName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.CityName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Request.BuildingNumber).NotEmpty().MaximumLength(20);

        });

        When(x => x.Request.Type == ClientType.Company.Value, () =>
        {
            RuleFor(x => x.Request.CommercialRegister)
                .NotEmpty().MaximumLength(50);

            RuleFor(x => x.Request.TaxNumber)
                .NotEmpty().MaximumLength(50);


            RuleFor(x => x.Request.StreetName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Request.CityName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Request.PostalZone).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Request.CountryCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.Request.BuildingNumber).NotEmpty().MaximumLength(20);

            RuleFor(x => x.Request.AdditionalStreetName).MaximumLength(200);
            RuleFor(x => x.Request.CountrySubentity).MaximumLength(100);
            RuleFor(x => x.Request.CitySubdivisionName).MaximumLength(100);
        });
    }
}
