using Entities;
using ServiceContracts;
using ServiceContracts.DTOs;

namespace Servicess
{
    public class CountryServices : ICountryServices
    {
       private readonly List<Country> countries;
        public CountryServices(bool generateFakeData = true)
        {
            countries = new List<Country>();

            if (generateFakeData)
            {
                countries.AddRange(new List<Country>
            {
                new Country { CountryId = Guid.Parse("7C9E6645-3677-448A-95B7-511B41F17491"), CountryName = "Japan" },
                new Country { CountryId = Guid.Parse("A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6"), CountryName = "Canada" },
                new Country { CountryId = new Guid("4A91B323-6902-4D3E-B147-3A2F6990C254"), CountryName = "Norway" },
                new Country { CountryId = new Guid("99C6A23D-8D1E-4E90-95B6-03B576C75F71"), CountryName = "Australia" },
                new Country { CountryId = new Guid("F2345B12-1111-4A55-89CC-5521AABBCCDD"), CountryName = "Brazil" }
            });
            }
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
