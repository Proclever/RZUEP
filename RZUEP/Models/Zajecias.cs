namespace RZUEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Zajecias
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Zajecias()
        {
            Prowadzacies = new HashSet<Prowadzacies>();
        }

        public int id { get; set; }

        public int planid { get; set; }

        public string godzinaod { get; set; }

        public string godzinado { get; set; }

        public string rodzaj { get; set; }

        public string nazwa { get; set; }

        public string sala { get; set; }

        public int dzien { get; set; }

        public string info { get; set; }

        public virtual Plans Plans { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prowadzacies> Prowadzacies { get; set; }
    }
}
