using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;
using System;
using System.Collections.Generic;

namespace CRUDTests.CustomFactory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IPersonServices> PersonServicesMock { get; private set; }
        public Mock<ICountryServices> CountryServicesMock { get; private set; }

        public CustomWebApplicationFactory()
        {
            PersonServicesMock = new Mock<IPersonServices>();
            CountryServicesMock = new Mock<ICountryServices>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing service registrations
                var personServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IPersonServices));
                if (personServiceDescriptor != null)
                {
                    services.Remove(personServiceDescriptor);
                }

                var countryServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICountryServices));
                if (countryServiceDescriptor != null)
                {
                    services.Remove(countryServiceDescriptor);
                }

                // Add mocked services
                services.AddScoped(_ => PersonServicesMock.Object);
                services.AddScoped(_ => CountryServicesMock.Object);
            });
        }
    }
}