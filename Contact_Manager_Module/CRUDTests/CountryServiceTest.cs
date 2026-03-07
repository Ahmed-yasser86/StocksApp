using ServiceContracts;
using ServiceContracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class CountryServiceTest
    {
        private readonly ICountryServices _countryServices;

        public CountryServiceTest()
        {
            _countryServices = new Servicess.CountryServices(true);
        }



        #region AddCountryRequest Tests

        [Fact]
      
         public void AddCountryRequest_NameNullValue()
          {
            // Arrange
            var countryAddRequest = new CountryAddRequest
            {
                CountryName = null
            };

            Assert.Throws<ArgumentException>(() => _countryServices.AddCountryRequest(countryAddRequest));

          
        }

                [Fact]
        public void AddCountryRequest_NullValue()
        {
            // Arrange
            CountryAddRequest countryAddRequest = null;

            Assert.Throws<ArgumentNullException>(() => _countryServices.AddCountryRequest(countryAddRequest));

          
        }



        [Fact]
        public void AddCountryRequest_ProperCountryDetail()
        {
            // Arrange

            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "Japan"
            };

            var CountryResponse = _countryServices.AddCountryRequest(countryAddRequest);

            List<CountryResponse> GetAllCountries= _countryServices.Countries();

            Assert.True(CountryResponse.CountryId != null);
            Assert.Contains(CountryResponse, GetAllCountries);

        }



        [Fact]
        public void AddCountryRequest_AddDuplicateCountry()
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
            _countryServices.AddCountryRequest(countryAddRequest);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => {
                _countryServices.AddCountryRequest(countryAddRequest);
                _countryServices.AddCountryRequest(countryAddRequest2);
            });
            }


        #endregion


        #region GetAllCountriesRequest Tests


        [Fact]
        public void GetAllCountriesRequest_EmptyList()
        {
            // Act
            var countries = _countryServices.Countries();

            // Assert
            Assert.Empty(countries);
        }

        [Fact]
        public void GetAllCountriesRequest_AddFewCountries()
        {
         
          List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>
            {
                new CountryAddRequest { CountryName = "Country 1" },
                new CountryAddRequest { CountryName = "Country 2" },
                new CountryAddRequest { CountryName = "Country 3" }
            };

            List<CountryResponse> expectedCountries = new List<CountryResponse>();
         
            foreach (var request in countryAddRequests)
            {

                
                expectedCountries.Add( _countryServices.AddCountryRequest(request));
            }



            // Act

            List<CountryResponse> actualCountries = _countryServices.Countries();
            foreach (var expectedCountry in expectedCountries)
            {
                // Assert
                Assert.Contains(expectedCountry, actualCountries);
            }
        }

        #endregion

        #region GetCountryById Tests

        [Fact]
        public void GetCountryByCountryId_NullValue()
        {

            Guid? CID = null;

            CountryResponse ?countryResponse = _countryServices.GetCountryByCountryId(CID);

            Assert.Null(countryResponse);

        }

        [Fact]
        public void GetCountryByCountryId_ValidateCountryId()
        {
            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "Test Country"
            };
            CountryResponse countryResponse = _countryServices.AddCountryRequest(countryAddRequest);
            CountryResponse? actualCountryResponse = _countryServices.GetCountryByCountryId(countryResponse.CountryId);
            Assert.NotNull(actualCountryResponse);
            Assert.Equal(countryResponse.CountryId, actualCountryResponse.CountryId);
            Assert.Equal(countryResponse.CountryName, actualCountryResponse.CountryName);


        }


        #endregion



    };

        }

