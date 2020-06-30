using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer.Contracts.Models
{
    public class Owner
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public IEnumerable<Pet> Pets { get; set; }
    }
}
