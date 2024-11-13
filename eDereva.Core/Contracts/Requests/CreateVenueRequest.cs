using FluentValidation;

namespace eDereva.Core.Contracts.Requests
{
    public class CreateVenueRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public List<string>? ImageUrls { get; set; }
        public int Capacity { get; set; }
        public Guid DistrictId { get; set; }
        public List<Guid>? VenueManagers { get; set; }
    }

    public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
    {
        public CreateVenueRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Venue name is required.")
                .MinimumLength(3)
                .WithMessage("Venue name is too short, minimum length is 3 characters.");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MinimumLength(5)
                .WithMessage("Address is too short, minimum length is 5 characters.");

            RuleFor(x => x.ImageUrls)
                .Must(imageUrls => imageUrls == null || imageUrls.All(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)))
                .WithMessage("All image URLs must be valid.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity must be greater than 0.");

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .WithMessage("District ID is required.");

            RuleFor(x => x.VenueManagers)
                .Must(venueManagers => venueManagers == null || venueManagers.All(managerId => managerId != Guid.Empty))
                .WithMessage("All venue manager IDs must be valid.")
                .Must(venueManagers => venueManagers == null || venueManagers.Count <= 2)
                .WithMessage("You can assign at most 2 venue managers.");
        }
    }

}
