using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts
{
   
    public interface ICountryServices
     {

        public CountryResponse AddCountryRequest(CountryAddRequest? countryAddRequest);


        public List<CountryResponse> Countries();


        public CountryResponse GetCountryByCountryId(Guid? ID);
    }

}
