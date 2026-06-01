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
            

        public List<CountryResponse> Countries()
        {
           
            return db.Set<Country>().Select(country => country.ConvertToDto()).ToList();

        }
        public CountryResponse AddCountryRequest(CountryAddRequest? countryAddRequest)
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
            db.SaveChanges();



            return country.ConvertToDto();
        }

        public CountryResponse? GetCountryByCountryId(Guid? ID)
        {

            if (ID == null)
                return null;


            Country country = db.Set<Country>().FirstOrDefault(c => c.CountryId == ID);
            if (country == null)
                return null;


            return country.ConvertToDto();

        }
    }
}
