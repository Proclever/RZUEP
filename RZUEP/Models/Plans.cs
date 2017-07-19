namespace RZUEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Plans
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Plans()
        {
            Zajecias = new HashSet<Zajecias>();
        }

        public int id { get; set; }

        public string semestr { get; set; }

        public string tryb { get; set; }

        public string stopien { get; set; }

        public string rok { get; set; }

        public string kierunek { get; set; }

        public string grupa { get; set; }

        public string specjalnosc { get; set; }

        public string wydzial { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Zajecias> Zajecias { get; set; }
    }
}
