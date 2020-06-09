using System.Collections.Generic;

namespace TFG.ServiceContracts.Models
{
    public class User
    {
        public string Name { get; set; }

        public string PinNumber { get; set; }

        public List<string> Allergies { get; set; }
    }
}
