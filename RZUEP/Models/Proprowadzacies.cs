namespace RZUEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Proprowadzacies
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proprowadzacies()
        {
            Prozajecias = new HashSet<Prozajecias>();
        }

        public int id { get; set; }

        public string semestr { get; set; }

        public string tryb { get; set; }

        public string wydzial { get; set; }

        public string nazwa { get; set; }

        public string jednostka { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prozajecias> Prozajecias { get; set; }
    }
}
