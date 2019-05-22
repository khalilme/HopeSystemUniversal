using MyClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClasses
{
  public  class DeafSentensePriority
    {
        private string _word;
        public string Word {
            get
            {
                if (_word != null)
                    return _word;
                else
                    return "";
            }
            set
            {
                _word = value;
            }
        }
        public string Type { get; set; }

        //public List<Interpretation> Interpretations { get; set; }
        public string Interpretations { get; set; }
        public int Row { get; set; }
        public int Index { get; set; }
        public DeafSentensePriority( string type,string word,int row,int index,string interpretation)//,List<Interpretation> li)
        {
            Word = word;
            Type = type;
            Row = row;
            Index = index;
            Interpretations = interpretation;
        }
    }

    enum Priority : int
    {
        استفهام,
        نفي,
        نهي,
        أمر,
    متكلم,
    فاعل,
    مضارع,
    مستقبل,
    فعل,
    غائب,
    مخاطب,
    }
}
