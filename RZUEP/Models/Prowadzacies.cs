namespace RZUEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Prowadzacies
    {
        public int id { get; set; }

        public int zajeciaid { get; set; }

        public string nazwa { get; set; }

        public virtual Zajecias Zajecias { get; set; }
    }
}
