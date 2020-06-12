using System;

namespace MCHomem.Prototype.Correios.Models.Entities
{
    public class Address
    {
        #region Properties

        public String ZipCode { get; set; }
        public String PulicPlace { get; set; }
        public String Neighborhood { get; set; }
        public String City { get; set; }
        public String FederationUnity { get; set; }

        #endregion

        #region Methods

        public Address() { }

        #endregion
    }
}
