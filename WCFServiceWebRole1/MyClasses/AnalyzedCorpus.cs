using System.Runtime.Serialization;
using WCFServiceWebRole.ATKNER;

namespace WCFServiceWebRole
{
    [DataContract]
    public class AnalyzedCorpus
    {
        [DataMember]
        public string AnalyzedText { get; set; }
        [DataMember]
        public NamedEntity[] EntitesList { get; set; }
    }
}