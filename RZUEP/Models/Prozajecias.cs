namespace RZUEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Prozajecias
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prozajecias()
        {
            Grupies = new HashSet<Grupies>();
        }

        public int id { get; set; }

        public int proprowadzacyid { get; set; }

        public int dzien { get; set; }

        public string godzinaod { get; set; }

        public string godzinado { get; set; }

        public string rodzaj { get; set; }

        public string nazwa { get; set; }

        public string sala { get; set; }

        public string info { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grupies> Grupies { get; set; }

        public virtual Proprowadzacies Proprowadzacies { get; set; }
    }
}
