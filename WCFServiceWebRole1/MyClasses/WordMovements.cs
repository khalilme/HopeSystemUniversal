//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFServiceWebRole.MyClasses
{
    using System;
    using System.Collections.Generic;
    
    public partial class WordMovements
    {
        public int ID { get; set; }
        public string Word { get; set; }
        public string Language { get; set; }
        public string SignMove { get; set; }
        public string Description { get; set; }
        public Nullable<int> RootID { get; set; }
    
        public virtual VerbsRoot VerbsRoot { get; set; }
    }
}
