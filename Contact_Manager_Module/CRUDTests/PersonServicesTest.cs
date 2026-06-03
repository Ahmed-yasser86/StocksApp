using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CRUDTests
{
    public class PersonServicesTest
    {
        private readonly IPersonServices _personServices;
        private readonly ICountryServices _countryServices;

        public PersonServicesTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PersonDBContext>().Options;
            _personServices = new Servicess.PersonServices(new PersonDBContext(dbContextOptions));
            _countryServices = new Servicess.CountryServices(new PersonDBContext(dbContextOptions));
        }

        #region AddPerson Tests

        [Fact]
        public async Task AddPerson_null()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_nullPersonName()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest
            {
                Name = null,
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "test@example.com",
                Address = "123 Main St",
                CountryId = Guid.NewGuid(),
                phone = "123456789",
                Gender = GenderOptions.Male,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest
            {
                Name = "null",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "test@example.com",
                Address = "123 Main St",
                CountryId = Guid.NewGuid(),
                phone = "123456789",
                Gender = GenderOptions.Male,
            };

            // Act
            var personResponse = await _personServices.AddPerson(personAddRequest);
            var PeronsList = await _personServices.GetAllPersons();

            // Assert
            Assert.Contains(personResponse, PeronsList);
            Assert.True(personResponse.PersonId != null);
        }

        #endregion

        #region GetPersonByPersonId Tests

        [Fact]
        public async Task GetPersonByPersonId_null()
        {
            // Arrange
            Guid? personId = null;

            // Act
            var k = await _personServices.GetPersonByPersonId(personId);

            // Assert
            Assert.Null(k);
        }

        [Fact]
        public async Task GetPersonByPersonId_Test()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "null",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "test@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
            };

            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India",
            };

            var countryResponse = await _countryServices.AddCountryRequest(countryAddRequest);
            personAddRequest.CountryId = countryResponse.CountryId;

            // Act
            var personResponse = await _personServices.AddPerson(personAddRequest);
            var getPersonResponse = await _personServices.GetPersonByPersonId(personResponse.PersonId);

            // Assert
            Assert.NotNull(getPersonResponse);
            Assert.Equal(personResponse.PersonId, getPersonResponse.PersonId);
        }

        #endregion

        #region GetAllPersons Tests

        [Fact]
        public async Task GetAllPersons_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryAddRequest CountryAdded2 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest CountryAdded3 = new CountryAddRequest { CountryName = "UK" };

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse1.CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse2.CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse3.CountryId,
            };

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> sentlist = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                sentlist.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> actualList = await _personServices.GetAllPersons();

            // Assert
            foreach (var p in actualList)
            {
                Assert.Contains(p, actualList);
            }
        }

        #endregion

        #region SearchBy Tests

        [Fact]
        public async Task GetPersonsByName_Empty_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryAddRequest CountryAdded2 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest CountryAdded3 = new CountryAddRequest { CountryName = "UK" };

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse1.CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse2.CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse3.CountryId,
            };

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> sentlist = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                sentlist.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> actualList = await _personServices.SearchPersonsBy(nameof(Person.Name), "");

            // Assert
            foreach (var p in actualList)
            {
                Assert.Contains(p, actualList);
            }
        }

        [Fact]
        public async Task GetPersonsByName_GetSomeResults_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryAddRequest CountryAdded2 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest CountryAdded3 = new CountryAddRequest { CountryName = "UK" };

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse1.CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse2.CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse3.CountryId,
            };

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> sentlist = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                sentlist.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> actualList = await _personServices.SearchPersonsBy(nameof(Person.Name), "ND");

            // Assert
            foreach (var p in actualList)
            {
                if (p.Name.Contains("ND", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(p, actualList);
                }
            }
        }

        #endregion

        #region GetPersonSorted Tests

        [Fact]
        public async Task GetPersonsSorted_DESC_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryAddRequest CountryAdded2 = new CountryAddRequest { CountryName = "USA" };
            CountryAddRequest CountryAdded3 = new CountryAddRequest { CountryName = "UK" };

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse1.CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse2.CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse3.CountryId,
            };

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> RecivedAfterAdditionList = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                RecivedAfterAdditionList.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> all_persons = await _personServices.GetAllPersons();
            List<PersonRespones> actualList = await _personServices.getPersonsSorted(all_persons, nameof(Person.Name), sortedListOp.Descending);

            RecivedAfterAdditionList = RecivedAfterAdditionList.OrderByDescending(p => p.Name).ToList();

            // Assert
            for (int i = 0; i < RecivedAfterAdditionList.Count; i++)
            {
                Assert.Equal(RecivedAfterAdditionList[i], actualList[i]);
            }
        }

        #endregion

        #region UpdatePerson Tests

        [Fact]
        public async Task UpdatePerson_ProperDetails_IdIsNull_Test()
        {
            // Arrange
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = null,
                Name = null,
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personServices.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_Null_Test()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personServices.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ProperDetails_NameIsNull_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = PrRespomns.PersonId,
                Name = null,
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personServices.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ProperDetails_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = PrRespomns.PersonId,
                Name = "null",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            // Act
            PersonRespones prsonAfterUpdate = await _personServices.UpdatePerson(personUpdateRequest);
            PersonRespones RetrivedPerson = await _personServices.GetPersonByPersonId(PrRespomns.PersonId);

            // Assert
            Assert.Equal(prsonAfterUpdate, RetrivedPerson);
        }

        #endregion

        #region DeletePersonByPersonId Tests

        [Fact]
        public async Task DeletePersonByPersonId_Null_Test()
        {
            // Arrange
            Guid? personId = null;

            // Act & Assert
            Assert.False(await _personServices.DeletePersonByPersonId(personId));
        }

        [Fact]
        public async Task DeletePersonByPersonId_ValidTest_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            // Act
            bool IsTrue = await _personServices.DeletePersonByPersonId(PrRespomns.PersonId);

            // Assert
            Assert.True(IsTrue);
        }

        [Fact]
        public async Task DeletePersonByPersonId_InValidTest_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest { CountryName = "India" };
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            // Act
            bool IsTrue = await _personServices.DeletePersonByPersonId(Guid.NewGuid());

            // Assert
            Assert.False(IsTrue);
        }

        #endregion
    }
}