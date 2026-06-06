
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositryContracts;

namespace Repositories
{
    public class CountryRepository : CountryRepositryContract
    {
      private readonly  AppDBContext _db;

        public CountryRepository(AppDBContext db)
        {
            this._db = db;
        }


        public async Task<Country> AddCountry(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();   
            return country;
        }

        public async Task<IEnumerable<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }
        

        

        public async Task<Country>? GetCountryByName(string name)
        {
            return await _db.Countries.FirstOrDefaultAsync(c => c.CountryName == name);    
        }

        public async Task<Country>? GetCountryById(Guid? id)
        {
            return await _db.Countries.FindAsync(id);
        }

        public Task<Country> UpdateCountry(Country country)
        {
            throw new NotImplementedException();
        }

    }
}
