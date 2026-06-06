using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using Repositories;
using RepositryContracts;
namespace Servicess
{
    public class CountryServices : ICountryServices
    {
       private readonly CountryRepositryContract CountriesRipositry;
        public CountryServices( CountryRepositryContract countriesRipositry)
        {
            CountriesRipositry = countriesRipositry;

          
        }


        public async Task<List<CountryResponse>> Countries()
        {
            return  (await CountriesRipositry.GetAllCountries()).Select(country => country.ConvertToDto()).ToList();

        }
        public async Task<CountryResponse> AddCountryRequest(CountryAddRequest? countryAddRequest)
        {


            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));


            if(CountriesRipositry.GetCountryByName(countryAddRequest.CountryName)!=null)
            {
                throw new ArgumentException($"Country with name {countryAddRequest.CountryName} already exists.", nameof(countryAddRequest.CountryName));
            }

                Country country = new Country();
            country = countryAddRequest.ConvertToCountry();
            country.CountryId = Guid.NewGuid();
            CountriesRipositry.AddCountry(country);
      



            return  country.ConvertToDto();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? ID)
        {

            if (ID == null)
                return null;


            Country country = await CountriesRipositry.GetCountryById(ID);
            if (country == null)
                return null;


            return country.ConvertToDto();

        }
    }
}
