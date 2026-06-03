using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;

namespace Servicess
{
    public class CountryServices : ICountryServices
    {
       private readonly PersonDBContext db;
        public CountryServices( PersonDBContext dbContext)
        {
            db = dbContext;

          
        }

        public async Task<List<CountryResponse>> Countries()
        {
            return await db.Set<Country>().Select(country => country.ConvertToDto()).ToListAsync();

        }
        public async Task<CountryResponse> AddCountryRequest(CountryAddRequest? countryAddRequest)
        {


            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
                throw new ArgumentException("Country name cannot be null or empty.", nameof(countryAddRequest.CountryName));


            if(db.Set<Country>().Any(countries=> countries.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException($"Country with name {countryAddRequest.CountryName} already exists.", nameof(countryAddRequest.CountryName));
            }

                Country country = new Country();
            country = countryAddRequest.ConvertToCountry();
            country.CountryId = Guid.NewGuid();
            db.Set<Country>().Add(country);
            await db.SaveChangesAsync();



            return country.ConvertToDto();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? ID)
        {

            if (ID == null)
                return null;


            Country country = await db.Set<Country>().FirstOrDefaultAsync(c => c.CountryId == ID);
            if (country == null)
                return null;


            return country.ConvertToDto();

        }
    }
}
