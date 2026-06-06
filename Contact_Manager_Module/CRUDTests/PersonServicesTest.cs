using Entities;
using EntityFrameworkCoreMock;
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
using AutoFixture;
using Moq;
using RepositryContracts;
using FluentAssertions;
namespace CRUDTests
{
    public class PersonServicesTest
    {
        private readonly IPersonServices _personServices;
        private readonly ICountryServices _countryServices;
        private readonly IFixture _fixture;
        private readonly Mock<PersonRepositryContract> _personRepositryContractMoq;
       private readonly PersonRepositryContract _personRepositryContract;
        public PersonServicesTest()
        {
            _fixture = new Fixture();

            List<Person> persons = new List<Person>();
            List<Country> countries = new List<Country>();
            DbContextMock<AppDBContext> dbContextMock = new DbContextMock<AppDBContext>(new DbContextOptionsBuilder<AppDBContext>().Options);

            _personRepositryContractMoq = new Mock<PersonRepositryContract>();
            _personRepositryContract = _personRepositryContractMoq.Object;  

            dbContextMock.CreateDbSetMock(temp => temp.Persons, persons);
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);

            _personServices = new Servicess.PersonServices(_personRepositryContract);
        //    _countryServices = new Servicess.CountryServices(null);
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
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Name, (string?)null)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();


            Person person = personAddRequest.ToPerson();

            PersonRespones personResponesobj = person.ConvertToPersonRespons();
            // Act
            var personResponse_expecteed  = await _personServices.AddPerson(personAddRequest);
            

            _personRepositryContractMoq.Setup(repo => repo.AddPerson(It.IsAny<Person>())).ReturnsAsync(personAddRequest.ToPerson());



            personResponse_expecteed.PersonId.Should().NotBe(Guid.Empty);
            personResponesobj.PersonId = personResponse_expecteed.PersonId;
            personResponesobj.Should().Be(personResponse_expecteed);
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
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded2 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded3 = _fixture.Create<CountryAddRequest>();

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse1.CountryId)
                .With(p => p.email, "p1@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse2.CountryId)
                .With(p => p.email, "p2@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse3.CountryId)
                .With(p => p.email, "p3@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> sentlist = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                sentlist.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> actualList = await _personServices.GetAllPersons();

            // Assert
            foreach (var p in sentlist)
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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded2 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded3 = _fixture.Create<CountryAddRequest>();

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse1.CountryId)
                .With(p => p.email, "p1@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse2.CountryId)
                .With(p => p.email, "p2@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse3.CountryId)
                .With(p => p.email, "p3@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            List<PersonAddRequest> expectedList = new List<PersonAddRequest> { p1, p2, p3 };
            List<PersonRespones> sentlist = new List<PersonRespones>();

            foreach (var person in expectedList)
            {
                sentlist.Add(await _personServices.AddPerson(person));
            }

            // Act
            List<PersonRespones> actualList = await _personServices.SearchPersonsBy(nameof(Person.Name), "");

            // Assert
            foreach (var p in sentlist)
            {
                Assert.Contains(p, actualList);
            }
        }

        [Fact]
        public async Task GetPersonsByName_GetSomeResults_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded2 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded3 = _fixture.Create<CountryAddRequest>();

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Name, "Ronaldo_ND")
                .With(p => p.CountryId, countryResponse1.CountryId)
                .With(p => p.email, "p1@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Name, "Andres_ND")
                .With(p => p.CountryId, countryResponse2.CountryId)
                .With(p => p.email, "p2@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Name, "Mona")
                .With(p => p.CountryId, countryResponse3.CountryId)
                .With(p => p.email, "p3@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded2 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest CountryAdded3 = _fixture.Create<CountryAddRequest>();

            var countryResponse1 = await _countryServices.AddCountryRequest(CountryAdded1);
            var countryResponse2 = await _countryServices.AddCountryRequest(CountryAdded2);
            var countryResponse3 = await _countryServices.AddCountryRequest(CountryAdded3);

            PersonAddRequest p1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse1.CountryId)
                .With(p => p.email, "p1@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse2.CountryId)
                .With(p => p.email, "p2@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonAddRequest p3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse3.CountryId)
                .With(p => p.email, "p3@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

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
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.PersonId, (Guid?)null)
                .With(p => p.Name, (string?)null)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .Create();

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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse.CountryId)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.PersonId, PrRespomns.PersonId)
                .With(p => p.Name, (string?)null)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personServices.UpdatePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ProperDetails_Test()
        {
            // Arrange
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse.CountryId)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .With(p => p.PersonId, PrRespomns.PersonId)
                .With(p => p.CountryId, countryResponse.CountryId)
                .With(p => p.email, "updated@example.com")
                .With(p => p.phone, "987654321")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse.CountryId)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

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
            CountryAddRequest CountryAdded1 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countryServices.AddCountryRequest(CountryAdded1);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.CountryId, countryResponse.CountryId)
                .With(p => p.email, "test@example.com")
                .With(p => p.phone, "123456789")
                .With(p => p.Gender, GenderOptions.Male)
                .Create();

            PersonRespones PrRespomns = await _personServices.AddPerson(personAddRequest);

            // Act
            bool IsTrue = await _personServices.DeletePersonByPersonId(Guid.NewGuid());

            // Assert
            Assert.False(IsTrue);
        }

        #endregion
    }
}