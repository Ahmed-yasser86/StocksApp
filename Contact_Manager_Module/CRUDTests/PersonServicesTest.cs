using Entities;
using ServiceContracts;
using ServiceContracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using ServiceContracts.DTOs.Enums;

namespace CRUDTests
{
    public class PersonServicesTest
    {

        private readonly IPersonServices _personServices;
        private readonly ICountryServices _countryServices;
        public PersonServicesTest()
        {
            _personServices = new Servicess.PersonServices();
            _countryServices = new Servicess.CountryServices();
        }

        #region AddPerson Tests

        [Fact]
        public void AddPerson_null()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public void AddPerson_nullPersonName()
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
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,

            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personServices.AddPerson(personAddRequest));
        }


        [Fact]
        public void AddPerson_ProperPersonDetails()
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
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,

            };

            // Act & Assert

            var personResponse = _personServices.AddPerson(personAddRequest);
            var PeronsList = _personServices.GetAllPersons();
            Assert.Contains(personResponse, PeronsList);
            Assert.True(personResponse.PersonId != null);

        }


        #endregion

        #region GetPersonByPersonId Tests

        [Fact]
        public void GetPersonByPersonId_null()
        {
            // Arrange
            Guid? personId = null;
            // Change the call to:
            var k = _personServices.GetPersonByPersonId(personId);            // Act & Assert
            Assert.Null(k);
        }

        [Fact]
        public void GetPersonByPersonId_Test()
        {

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "null",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "test@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,


            };

            CountryAddRequest countryAddRequest = new CountryAddRequest
            {
                CountryName = "India",
            };

            var countryResponse = _countryServices.AddCountryRequest(countryAddRequest);
            personAddRequest.CountryId = countryResponse.CountryId;
            var personResponse = _personServices.AddPerson(personAddRequest);

            var getPersonResponse = _personServices.GetPersonByPersonId(personResponse.PersonId);

            Assert.NotNull(getPersonResponse);
            Assert.Equal(personResponse.PersonId, getPersonResponse.PersonId);
        }
        #endregion

        #region

        [Fact]
        public void GetAllPersons_Test()
        {

            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };

            CountryAddRequest CountryAdded2 = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryAddRequest CountryAdded3 = new CountryAddRequest
            {
                CountryName = "UK",
            };


            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded1).CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded2).CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded3).CountryId,
            };


            List<PersonAddRequest> expectedList = new List<PersonAddRequest>
            {
               p1, p2, p3
            };

            List<PersonRespones> sentlist = new List<PersonRespones>();
            foreach (var person in expectedList)
            {

                sentlist.Add(_personServices.AddPerson(person));
            }
            List<PersonRespones> actualList = _personServices.GetAllPersons();


            foreach (var p in actualList)
            {
                Assert.Contains(p, actualList);

            }


        }


        #endregion

        #region SearchBy

        [Fact]
        public void GetPersonsByName_Empty_Test()
        {

            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };

            CountryAddRequest CountryAdded2 = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryAddRequest CountryAdded3 = new CountryAddRequest
            {
                CountryName = "UK",
            };


            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded1).CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded2).CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded3).CountryId,
            };


            List<PersonAddRequest> expectedList = new List<PersonAddRequest>
            {
               p1, p2, p3
            };

            List<PersonRespones> sentlist = new List<PersonRespones>();
            foreach (var person in expectedList)
            {

                sentlist.Add(_personServices.AddPerson(person));
            }
            List<PersonRespones> actualList = _personServices.SearchPersonsBy(nameof(Person.Name), "");


            foreach (var p in actualList)
            {
                Assert.Contains(p, actualList);

            }


        }



        [Fact]
        public void GetPersonsByName_GetSomeResults_Test()
        {

            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };

            CountryAddRequest CountryAdded2 = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryAddRequest CountryAdded3 = new CountryAddRequest
            {
                CountryName = "UK",
            };


            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded1).CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded2).CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded3).CountryId,
            };


            List<PersonAddRequest> expectedList = new List<PersonAddRequest>
            {
               p1, p2, p3
            };

            List<PersonRespones> sentlist = new List<PersonRespones>();
            foreach (var person in expectedList)
            {

                sentlist.Add(_personServices.AddPerson(person));
            }
            List<PersonRespones> actualList = _personServices.SearchPersonsBy(nameof(Person.Name), "ND");


            foreach (var p in actualList)
            {

                if (p.Name.Contains("ND", StringComparison.OrdinalIgnoreCase))
                {

                    Assert.Contains(p, actualList);
                }


            }


        }


        #endregion


        #region GetPersonSorted 


        [Fact]
        public void GetPersonsSorted_DESC_Test()
        {

            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };

            CountryAddRequest CountryAdded2 = new CountryAddRequest
            {
                CountryName = "USA",
            };

            CountryAddRequest CountryAdded3 = new CountryAddRequest
            {
                CountryName = "UK",
            };


            PersonAddRequest p1 = new PersonAddRequest
            {
                Name = "NDJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNFst@example.com",
                Address = "123 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded1).CountryId,
            };

            PersonAddRequest p2 = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded2).CountryId,
            };

            PersonAddRequest p3 = new PersonAddRequest
            {
                Name = "NDGDHYFGHDSJSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDGDSNFst@example.com",
                Address = "12FD3 DS,SMain St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = _countryServices.AddCountryRequest(CountryAdded3).CountryId,
            };


            List<PersonAddRequest> expectedList = new List<PersonAddRequest>
            {
               p1, p2, p3
            };

            List<PersonRespones> RecivedAfterAdditionList = new List<PersonRespones>();
            foreach (var person in expectedList)
            {

                RecivedAfterAdditionList.Add(_personServices.AddPerson(person));
            }

            List<PersonRespones> all_persons = _personServices.GetAllPersons();

            List<PersonRespones> actualList = _personServices.getPersonsSorted(all_persons, nameof(Person.Name), sortedListOp.Descending);
            RecivedAfterAdditionList = RecivedAfterAdditionList.OrderByDescending(p => p.Name).ToList();


            for (int i = 0; i < RecivedAfterAdditionList.Count; i++)
            {
                Assert.Equal(RecivedAfterAdditionList[i], actualList[i]);
            }
        }


        #endregion


        #region UpdatePerson Tests




        [Fact]
        public void UpdatePerson_ProperDetails_IdIsNull_Test()
        {
           


            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = null,
                Name = null,
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",


            };

            Assert.Throws<ArgumentException>(() => _personServices.UpdatePerson(personUpdateRequest));


        }


        [Fact]
        public void UpdatePerson_Null_Test()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personServices.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public void UpdatePerson_ProperDetails_NameIsNull_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };
            CountryResponse countryResponse = _countryServices.AddCountryRequest(CountryAdded1);


            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = PrRespomns.PersonId,
                Name = null,
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",


            };

            Assert.Throws<ArgumentException>(() => _personServices.UpdatePerson(personUpdateRequest));

        }


        [Fact]
        public void UpdatePerson_ProperDetails_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };
            CountryResponse countryResponse = _countryServices.AddCountryRequest(CountryAdded1);


            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                PersonId = PrRespomns.PersonId,
                Name = "null",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "ahmed@gmail.com",
                Gender = GenderOptions.Male,
                CountryId = countryResponse.CountryId,

            };

            PersonRespones prsonAfterUpdate = _personServices.UpdatePerson(personUpdateRequest);
            PersonRespones RetrivedPerson = _personServices.GetPersonByPersonId(PrRespomns.PersonId);

            Assert.Equal(prsonAfterUpdate, RetrivedPerson);


        }
        #endregion

        #region DeletePersonByPersonId Tests

        [Fact]
    public    void DeletePersonByPersonId_Null_Test()
        {
            // Arrange
            Guid? personId = null;
            // Act & Assert
            Assert.False(_personServices.DeletePersonByPersonId(personId));
        }

        [Fact]
        public void DeletePersonByPersonId_ValidTest_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };
            CountryResponse countryResponse = _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = _personServices.AddPerson(personAddRequest);
            bool IsTrue = _personServices.DeletePersonByPersonId(PrRespomns.PersonId);

            Assert.True(IsTrue);

        }


        [Fact]
        public void DeletePersonByPersonId_InValidTest_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = new CountryAddRequest
            {
                CountryName = "India",
            };
            CountryResponse countryResponse = _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = new PersonAddRequest
            {
                Name = "NDGDHSHJFD",
                DateOfBirth = new DateTime(1998, 1, 1),
                email = "tDKNDDFst@example.com",
                Address = "12FD3 Main St",
                phone = "123456789",
                Gender = ServiceContracts.DTOs.Enums.GenderOptions.Male,
                CountryId = countryResponse.CountryId,
            };

            PersonRespones PrRespomns = _personServices.AddPerson(personAddRequest);
            bool IsTrue = _personServices.DeletePersonByPersonId(Guid.NewGuid());

            Assert.False(IsTrue);

        }



        #endregion


    }
}