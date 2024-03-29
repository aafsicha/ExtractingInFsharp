﻿using LiveCoding.Domain;
using LiveCoding.Persistence;
using LiveCoding.Services;

namespace LiveCoding.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection InitializeDependencyInjection(this IServiceCollection services)
        {
            return services
                .AddScoped<IBarRepository, BarRepository>()
                .AddScoped<IDevRepository, DevRepository>()
                .AddScoped<BookingService>();
        }
    }
}
