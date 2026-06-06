using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositryContracts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace Repositories
{
    public class PersonRepository :  PersonRepositryContract
    {

        private readonly AppDBContext _db;

        public PersonRepository(AppDBContext db)
        {
            this._db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            if (id == null) return false;

            var person = await _db.Persons.FindAsync(id);
            if (person == null) return false;

            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Person>> GetAllPersons()
        {
            return await _db.Persons.Include(c=>c.Country).ToListAsync();
        }

        public async Task<List<Person?>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include(c=>c.Country).Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid? id)
        {
            if (id == null) return null;
            return await _db.Persons.FindAsync(id);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            
            _db.Persons.Update(person);
            await _db.SaveChangesAsync();
            return person;
        }
    
    }
}
