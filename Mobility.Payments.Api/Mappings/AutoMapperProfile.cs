namespace Mobility.Payments.Api.Mappings
{
    using AutoMapper;
    using Mobility.Payments.Api.Contracts.Request;
    using Mobility.Payments.Api.Contracts.Response;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Domain.Entities;

    /// <summary>
    /// Defines AutoMapper profiles for object-to-object mappings used in the application.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// Configures the mapping rules for various types in the application.
        /// </summary>
        public AutoMapperProfile()
        {
            this.CreateMap<Payment, PaymentDto>();
            this.CreateMap<PaymentDto, PaymentResponse>();
            this.CreateMap<CreateUserRequest, UserDto>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Balance, opt => opt.Ignore());
        }
    }
}
