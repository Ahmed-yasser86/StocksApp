using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;
using Servicess.Helpers;
using RepositryContracts;
using Repositories;

namespace Servicess
{
    public class PersonServices : IPersonServices
    {


        private readonly PersonRepositryContract PersonRipository;
        private readonly ICountryServices _countryServices;
        public PersonServices( PersonRepositryContract personRipository)
        {
            PersonRipository = personRipository;        
            
        }


        public async Task<PersonRespones> AddPerson(PersonAddRequest? personAddRequest)
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

            PersonRipository.AddPerson(Person);


            var PersonResponsType = Person.ConvertToPersonRespons();

            PersonResponsType.CountryName = Person.Country?.CountryName;
                return PersonResponsType;
           
        }

        public async Task<bool> DeletePersonByPersonId(Guid? personId)
        {
            if(personId == Guid.Empty || personId == null)
            {
                return false;
            }



            return await PersonRipository.DeletePerson(personId); 

        }



        public async Task<List<PersonRespones>> GetAllPersons()
        {
            var list = await  PersonRipository.GetAllPersons();
            return list.Select(p => p.ConvertToPersonRespons()).ToList();
        }

        public async Task<PersonRespones?> GetPersonByPersonId(Guid? personId)
        {
             if (personId == Guid.Empty || personId == null)
            {
                return null;

            }

            Person? person = await PersonRipository.GetPersonById(personId);
            if(person == null)
            {
                return null;
            }

            return person.ConvertToPersonRespons();

        }

        public async Task<List<PersonRespones>> getPersonsSorted(List<PersonRespones> persons, string? sortBy, sortedListOp sortOrder)
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

        public async Task<List<PersonRespones>> SearchPersonsBy(string? PersonParamter, string SearchBy)
        {
            List<PersonRespones> MatchingResults = new List<PersonRespones>();



            switch (SearchBy)
            {
                case nameof(PersonRespones.Name):
                    {
                        var filteredPersons = await PersonRipository.GetFilteredPersons(p => p.Name != null && p.Name.Contains(PersonParamter));
                        MatchingResults = filteredPersons
                            .Where(p => p != null)
                            .Select(p => p!.ConvertToPersonRespons())
                            .ToList();
                        break;
                    }
                case nameof(PersonRespones.email):
                    {
                        var filteredPersons = await PersonRipository.GetFilteredPersons(p => p.email != null && p.email.Contains(PersonParamter));
                        MatchingResults = filteredPersons
                            .Where(p => p != null)
                            .Select(p => p!.ConvertToPersonRespons())
                            .ToList();
                        break;
                    }
                case nameof(PersonRespones.phone):
                    {
                        var filteredPersons = await PersonRipository.GetFilteredPersons(p => p.phone != null && p.phone.Contains(PersonParamter));
                        MatchingResults = filteredPersons
                            .Where(p => p != null)
                            .Select(p => p!.ConvertToPersonRespons())
                            .ToList();
                        break;
                    }
                case nameof(PersonRespones.DateOfBirth):
                    {
                        var filteredPersons = await PersonRipository.GetFilteredPersons(p => p.DateOfBirth != null && p.DateOfBirth.Value.ToString("yyyy-MM-dd").Contains(PersonParamter));
                        MatchingResults = filteredPersons
                            .Where(p => p != null)
                            .Select(p => p!.ConvertToPersonRespons())
                            .ToList();
                        break;
                    }
                default:

                    var Persons = await PersonRipository.GetAllPersons();
                    MatchingResults = Persons
                        .Select(p => p!.ConvertToPersonRespons())
                        .ToList();
                    break;
            }
            return MatchingResults;
        }
        public async Task<PersonRespones?> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));

            }

          ValidationHelpers.ValidationFunction(personUpdateRequest);

          var person = await PersonRipository?.GetPersonById(personUpdateRequest.PersonId);
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
           await PersonRipository.UpdatePerson(person);
            return person.ConvertToPersonRespons();


        }
    }
}
