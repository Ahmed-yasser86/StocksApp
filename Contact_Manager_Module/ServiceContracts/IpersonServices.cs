using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonServices
    {

        public PersonRespones AddPerson(PersonAddRequest? personAddRequest);
        public List<PersonRespones> GetAllPersons();

        public PersonRespones? GetPersonByPersonId(Guid? personId);

        public List<PersonRespones> getPersonsSorted(List<PersonRespones> persons, string? sortBy, sortedListOp sortOrder);
        public List<PersonRespones> SearchPersonsBy(string? SearchBy,string SearchString);
        public PersonRespones? UpdatePerson(PersonUpdateRequest? personUpdateRequest);


     public   bool DeletePersonByPersonId(Guid? personId);
//object GetPersonByPersonId(int id);
    }
}
