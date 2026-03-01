using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Entities
{

    public class Person
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }

        public DateTime ?DateOfBirth { get; set; }
        public string? email { get; set; }

        public string phone { get; set; }

        public string Gender { get; set; }

        public bool ?NewsLetter { get; set; }

        public string ?Address { get; set; }

        public Guid CountryId { get; set; }
    }
}
