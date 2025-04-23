using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GAC.WMS.Application
{
    public static  class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
