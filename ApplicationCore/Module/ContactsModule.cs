using ApplicationCore.Mapper;
using ApplicationCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsCoreModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddContactsModule(configuration);
    }

    public static IServiceCollection AddContactsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddAutoMapper(cfg => cfg.AddProfile<ContactsMappingProfile>());
        return services;
    }
}
