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


        private readonly List<Person> _persons;
        private readonly ICountryServices _countryServices;
        public PersonServices(bool generateFakeData = true)
        {
            _persons = new List<Person>();
            _countryServices = new CountryServices(generateFakeData);

            if (generateFakeData)
            {
                _persons.AddRange(new List<Person>
            {
                new Person
                {
                    PersonId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Alice Smith",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    email = "alice@example.com",
                    phone = "555-0101",
                    Gender = "Female",
                    NewsLetter = true,
                    Address = "123 Sakura St",
                    CountryId = Guid.Parse("7C9E6645-3677-448A-95B7-511B41F17491") // Linked to Japan
                },
                new Person
                {
                    PersonId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Bob Johnson",
                    DateOfBirth = new DateTime(1985, 11, 2),
                    email = "bob@maple.ca",
                    phone = "555-0202",
                    Gender = "Male",
                    NewsLetter = false,
                    Address = "456 Maple Rd",
                    CountryId = Guid.Parse("A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6") // Linked to Canada
                },
                new Person
                {
                    PersonId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Charlie Davis",
                    DateOfBirth = new DateTime(1995, 7, 22),
                    email = "charlie@nordic.no",
                    phone = "555-0303",
                    Gender = "Female",
                    NewsLetter = true,
                    Address = "789 Fjord Ln",
                    CountryId = Guid.Parse("4A91B323-6902-4D3E-B147-3A2F6990C254") // Linked to Norway
                },
                new Person
                {
                    PersonId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Diana Prince",
                    DateOfBirth = new DateTime(1988, 3, 10),
                    email = "diana@outback.au",
                    phone = "555-0404",
                    Gender = "Female",
                    NewsLetter = true,
                    Address = "321 Outback Way",
                    CountryId = Guid.Parse("99C6A23D-8D1E-4E90-95B6-03B576C75F71") // Linked to Australia
                },
                new Person
                {
                    PersonId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Eduardo Gomes",
                    DateOfBirth = new DateTime(1992, 12, 25),
                    email = "edu@rio.br",
                    phone = "555-0505",
                    Gender = "Male",
                    NewsLetter = false,
                    Address = "654 Samba Blvd",
                    CountryId = Guid.Parse("F2345B12-1111-4A55-89CC-5521AABBCCDD") // Linked to Brazil
                }
            });
            }
        }


        public PersonRespones AddPerson(PersonAddRequest? personAddRequest)
        {
            if(personAddRequest == null)
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

            _persons.Add(Person);

            var PersonResponsType= Person.ConvertToPersonRespons();

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


            Person? person = _persons.FirstOrDefault(p => p.PersonId == personId);

            if (person == null)
            {
                return false;
            }


            _persons.RemoveAll(p => p.PersonId == personId);

            return true;




        }

        public List<PersonRespones> GetAllPersons()
        {
            return _persons.Select(p =>
            {
                var personResponse = p.ConvertToPersonRespons();
                personResponse.CountryName = _countryServices.GetCountryByCountryId(p.CountryId)?.CountryName ?? string.Empty;
                return personResponse;
            }).ToList();

        }

        public PersonRespones? GetPersonByPersonId(Guid? personId)
        {
             if (personId == Guid.Empty || personId == null)
            {
                return null;

            }

            Person? person = _persons.FirstOrDefault(p => p.PersonId == personId);
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
            List<PersonRespones> allPersons = GetAllPersons();

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

          var person = _persons.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId);
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
           


            return person.ConvertToPersonRespons();


        }
    }
}
