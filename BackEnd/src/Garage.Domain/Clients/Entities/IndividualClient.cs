using Garage.Domain.Clients.Enums;


namespace Garage.Domain.Clients.Entities
{
    public sealed class IndividualClient : Client
    {
        public string? Address { get; private set; } = null!;

        private IndividualClient() { }

        public IndividualClient(
            Guid userId,
            string nameAr,
            string nameEn,
            string phoneNumber,
            Guid? resourceId,
            string? address)
            : base(userId, ClientType.Individual, nameAr, nameEn, phoneNumber, resourceId)
        {
            Address = address;
        }

        public void UpdatePersonalInfo( string address)
        {
            Address = address;
        }
    }

}
