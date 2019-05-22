using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServiceWebRole
{
    [DataContract]
    public enum CloudWordType
    {
        Word=1,
        Image,
        NameFinger,
        NameFingerImage,
        NameImage
    }

    [DataContract]
    public class CloudSingleWord
    {
        [DataMember]
        public CloudWordType Type { get; set; }

        [DataMember]
        public string Word { get; set; }

        [DataMember]
        public string Image { get; set; }
    }
}