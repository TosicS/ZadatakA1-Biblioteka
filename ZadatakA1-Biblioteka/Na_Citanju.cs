//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZadatakA1_Biblioteka
{
    using System;
    using System.Collections.Generic;
    
    public partial class Na_Citanju
    {
        public int KnjigaID { get; set; }
        public int CitalacID { get; set; }
        public System.DateTime DatumUzimanja { get; set; }
        public Nullable<System.DateTime> DatumVracanja { get; set; }
    
        public virtual Citalac Citalac { get; set; }
        public virtual Knjiga Knjiga { get; set; }
    }
}