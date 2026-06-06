using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;

using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EntityFrameworkCoreMock;

namespace CRUDTests
{
    public class CountryServiceTest
    {
        private readonly ICountryServices _countryServices;

        public CountryServiceTest()
        {

            List<Country> countries = new List<Country>();
            DbContextMock<AppDBContext> dbContextMock = new DbContextMock<AppDBContext>(new DbContextOptionsBuilder<AppDBContext>().Options);
            AppDBContext db = dbContextMock.Object;
           dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);
            _countryServices = new Servicess.CountryServices(null);

            // _countryServices = new Servicess.CountryServices(new PersonDBContext(new DbContextOptionsBuilder<PersonDBContext>().Options));



        }

        #region AddCountryRequest Tests

        [Fact]
        public async Task AddCountryRequest_NameNullValue()
        {
            // Arrange
            var countryAddRequest = new CountryAddRequest
            {
                CountryName = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _countryServices.AddCountryRequest(countryAddRequest));
        }

        [Fact]
        public async Task AddCountryRequest_NullValue()
        {
            // Arrange
            CountryAddRequest countryAddRequest = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countryServices.AddCountryRequest(countryAddRequest));
        }

        [Fact]
        public async Task AddCountryRequest_ProperCountryDetail()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "Japan"
            };

            // Act
            var CountryResponse = await _countryServices.AddCountryRequest(countryAddRequest);
            List<CountryResponse> GetAllCountries = await _countryServices.Countries();

            // Assert
            Assert.True(CountryResponse.CountryId != null);
            Assert.Contains(CountryResponse, GetAllCountries);
        }

        [Fact]
        public async Task AddCountryRequest_AddDuplicateCountry()
        {
            // Arrange
            var countryAddRequest = new CountryAddRequest
            {
                CountryName = "Test Country"
            };

            var countryAddRequest2 = new CountryAddRequest
            {
                CountryName = "Test Country"
            };

            await _countryServices.AddCountryRequest(countryAddRequest);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countryServices.AddCountryRequest(countryAddRequest2);
            });
        }

        #endregion

        #region GetAllCountriesRequest Tests

        [Fact]
        public async Task GetAllCountriesRequest_EmptyList()
        {
            // Act
            var countries = await _countryServices.Countries();

            // Assert
            Assert.Empty(countries);
        }

        [Fact]
        public async Task GetAllCountriesRequest_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>
            {
                new CountryAddRequest { CountryName = "Country 1" },
                new CountryAddRequest { CountryName = "Country 2" },
                new CountryAddRequest { CountryName = "Country 3" }
            };

            List<CountryResponse> expectedCountries = new List<CountryResponse>();

            foreach (var request in countryAddRequests)
            {
                expectedCountries.Add(await _countryServices.AddCountryRequest(request));
            }

            // Act
            List<CountryResponse> actualCountries = await _countryServices.Countries();

            // Assert
            foreach (var expectedCountry in expectedCountries)
            {
                Assert.Contains(expectedCountry, actualCountries);
            }
        }

        #endregion

        #region GetCountryById Tests

        [Fact]
        public async Task GetCountryByCountryId_NullValue()
        {
            // Arrange
            Guid? CID = null;

            // Act
            CountryResponse? countryResponse = await _countryServices.GetCountryByCountryId(CID);

            // Assert
            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByCountryId_ValidateCountryId()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "Test Country"
            };

            // Act
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(countryAddRequest);
            CountryResponse? actualCountryResponse = await _countryServices.GetCountryByCountryId(countryResponse.CountryId);

            // Assert
            Assert.NotNull(actualCountryResponse);
            Assert.Equal(countryResponse.CountryId, actualCountryResponse.CountryId);
            Assert.Equal(countryResponse.CountryName, actualCountryResponse.CountryName);
        }

        #endregion
    }
}

