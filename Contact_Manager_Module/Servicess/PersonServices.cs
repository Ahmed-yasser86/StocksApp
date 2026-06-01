using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;
using Servicess.Helpers;

namespace Servicess
{
    public class PersonServices : IPersonServices
    {


        private readonly PersonDBContext db;
        private readonly ICountryServices _countryServices;
        public PersonServices(PersonDBContext dbContext)
        {
            db = dbContext;
            _countryServices = new CountryServices(dbContext);

          
            
        }


        public PersonRespones AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));


            //ValidationContext validationContext = new ValidationContext(personAddRequest, null, null);

            //List<ValidationResult> validationResults = new List<ValidationResult>();
            //bool isValid = Validator.TryValidateObject(personAddRequest, validationContext, validationResults, true);


            //if (!isValid)
            //{
            //    string errorMessages = string.Join("; ", validationResults.Select(vr => vr.ErrorMessage));
            //    throw new ValidationException(errorMessages);
            //}

            ValidationHelpers.ValidationFunction(personAddRequest);


            var Person = personAddRequest.ToPerson();
            Person.PersonId = Guid.NewGuid();

            db.Set<Person>().Add(Person);
            db.SaveChanges();
            
                var PersonResponsType = Person.ConvertToPersonRespons();

                PersonResponsType.CountryName = _countryServices.GetCountryByCountryId(
                    Person.CountryId)?.CountryName ?? string.Empty;
                return PersonResponsType;
           
        }

        public bool DeletePersonByPersonId(Guid? personId)
        {
            if(personId == Guid.Empty || personId == null)
            {
                return false;
            }


            Person? person = db.Persons.FirstOrDefault(p => p.PersonId == personId);

            if (person == null)
            {
                return false;
            }

            db.Persons.Remove(db.Persons.First(p => p.PersonId == personId));
            db.SaveChanges();

            return true;




        }



        public List<PersonRespones> GetAllPersons()
        {
                var list = db.Persons
                .Select(p => p.ConvertToPersonRespons())
                .ToList();

            return list;
        }

        public PersonRespones? GetPersonByPersonId(Guid? personId)
        {
             if (personId == Guid.Empty || personId == null)
            {
                return null;

            }

            Person? person = db.Persons.FirstOrDefault(p => p.PersonId == personId);
            if(person == null)
            {
                return null;
            }

            return person.ConvertToPersonRespons();

        }

        public List<PersonRespones> getPersonsSorted(List<PersonRespones> persons, string? sortBy, sortedListOp sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return persons;
            }


            List<PersonRespones> sortedPersons = (sortOrder, sortOrder) switch
            {
                (sortedListOp.Ascending, _) => sortBy switch
                {
                    nameof(PersonRespones.Name) => persons.OrderBy(p => p.Name).ToList(),
                    nameof(PersonRespones.email) => persons.OrderBy(p => p.email).ToList(),
                    nameof(PersonRespones.phone) => persons.OrderBy(p => p.phone).ToList(),
                    nameof(PersonRespones.DateOfBirth) => persons.OrderBy(p => p.DateOfBirth).ToList(),
                    nameof(PersonRespones.CountryName) => persons.OrderBy(p => p.CountryName).ToList(),
                    nameof(PersonRespones.Age) => persons.OrderBy(p => p.Age).ToList(),
                    nameof(PersonRespones.PersonId) => persons.OrderBy(p => p.PersonId).ToList(),
                    nameof(PersonRespones.Address) => persons.OrderBy(p => p.Address).ToList(),
                    _ => persons
                },
                (sortedListOp.Descending, _) => sortBy switch
                {
                    nameof(PersonRespones.Name) => persons.OrderByDescending(p => p.Name).ToList(),
                    nameof(PersonRespones.email) => persons.OrderByDescending(p => p.email).ToList(),
                    nameof(PersonRespones.phone) => persons.OrderByDescending(p => p.phone).ToList(),
                    nameof(PersonRespones.DateOfBirth) => persons.OrderByDescending(p => p.DateOfBirth).ToList(),
                    nameof(PersonRespones.CountryName) => persons.OrderByDescending(p => p.CountryName).ToList(),
                    nameof(PersonRespones.Age) => persons.OrderByDescending(p => p.Age).ToList(),
                    nameof(PersonRespones.PersonId) => persons.OrderByDescending(p => p.PersonId).ToList(),
                    nameof(PersonRespones.Address) => persons.OrderByDescending(p => p.Address).ToList(),


                    _ => persons
                },
                _ =>  persons
            };
            return sortedPersons;

        }

        public List<PersonRespones> SearchPersonsBy(string? PersonParamter, string SearchBy)
        {
            List<PersonRespones> allPersons = db.Persons.Select(p=>p.ConvertToPersonRespons()).ToList();

            List<PersonRespones> MatchingResults = allPersons;


            if (string.IsNullOrEmpty(PersonParamter) || string.IsNullOrEmpty(SearchBy))
            {

                return MatchingResults;
            }


            switch (SearchBy)
            {
                case nameof(PersonRespones.Name):
                    MatchingResults = allPersons.Where(p => p.Name.Contains(PersonParamter, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonRespones.email):
                    MatchingResults = allPersons.Where(p => p.email != null && p.email.Contains(PersonParamter, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonRespones.phone):
                    MatchingResults = allPersons.Where(p => p.phone.Contains(PersonParamter, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                case nameof(PersonRespones.DateOfBirth):
                    MatchingResults = allPersons.Where(p => p.DateOfBirth != null && p.DateOfBirth.Value.ToString("yyyy-MM-dd").Contains(PersonParamter)).ToList();
                    break;

                default: return MatchingResults;

            }
        return MatchingResults;

        }

        public PersonRespones? UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));

            }

          ValidationHelpers.ValidationFunction(personUpdateRequest);

          var person = db.Persons.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId);
            if (person == null)
            {
                throw new ArgumentException("Given person ID does not exist.");
            }
            person.Name = personUpdateRequest.Name ?? person.Name;
            person.DateOfBirth = personUpdateRequest.DateOfBirth ?? person.DateOfBirth;
            person.email = personUpdateRequest.email ?? person.email;
            person.phone = personUpdateRequest.phone ?? person.phone;
            person.NewsLetter = personUpdateRequest.NewsLetter ?? person.NewsLetter;
            person.Address = personUpdateRequest.Address ?? person.Address;
            person.CountryId = personUpdateRequest.CountryId ?? person.CountryId;
            person.Gender = personUpdateRequest.Gender.ToString() ?? person.Gender;
           db.SaveChanges();
            return person.ConvertToPersonRespons();


        }
    }
}
