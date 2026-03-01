using Entities;
using ServiceContracts;
using ServiceContracts.DTOs;

namespace Servicess
{
    public class CountryServices : ICountryServices
    {
       private readonly List<Country> countries;
        public CountryServices() {

            countries = new List<Country>();
        }

        public List<CountryResponse> Countries()
        {
           
            return countries.Select(country => country.ConvertToDto()).ToList();

        }
        public CountryResponse AddCountryRequest(CountryAddRequest? countryAddRequest)
        {


            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));


            if(countries.Where(countries=> countries.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException($"Country with name {countryAddRequest.CountryName} already exists.", nameof(countryAddRequest.CountryName));
            }

                Country country = new Country();
                country = countryAddRequest.ConvertToCountry();
                country.CountryId = Guid.NewGuid();
                countries.Add(country);
            


            return country.ConvertToDto();
        }

        public CountryResponse? GetCountryByCountryId(Guid? ID)
        {

            if (ID == null)
                return null;


            Country country = countries.FirstOrDefault(c => c.CountryId == ID);
            if (country == null)
                return null;


            return country.ConvertToDto();

        }
    }
}
