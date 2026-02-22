using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.ExaminationManagement.Shared;



public sealed class PlateNumber : ValueObject
{
    public string Letters { get; }
    public string Numbers { get; }

    private PlateNumber() { } // EF

    private PlateNumber(string letters, string numbers)
    {
        Letters = letters;
        Numbers = numbers;
    }

    public static PlateNumber Create(string letters, string numbers)
    {
        letters = NormalizeLetters(letters);
        numbers = NormalizeNumbers(numbers);

        Validate(letters, numbers);

        return new PlateNumber(letters, numbers);
    }

    private static string NormalizeLetters(string value)
        => value?.Trim().ToUpperInvariant() ?? "";

    private static string NormalizeNumbers(string value)
        => value?.Trim() ?? "";

    private static void Validate(string letters, string numbers)
    {
        if (string.IsNullOrWhiteSpace(letters))
            throw new DomainException("Plate letters required");

        if (string.IsNullOrWhiteSpace(numbers))
            throw new DomainException("Plate numbers required");

        if (numbers.Any(c => !char.IsDigit(c)))
            throw new DomainException("Plate numbers must be digits");

        if (letters.Length > 3)
            throw new DomainException("Egypt plate letters max 3");

        if (numbers.Length is < 1 or > 4)
            throw new DomainException("Egypt plate numbers 1-4 digits");
    }

    public override string ToString()
        => $"{Letters} {Numbers}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Letters;
        yield return Numbers;
    }
}
