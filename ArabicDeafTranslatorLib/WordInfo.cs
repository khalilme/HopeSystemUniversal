using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyClasses
{
    /// <summary>
    /// فئة الكلمة العربية الصحيحة 
    /// </summary>
    public class ArabicWord
    {
        public string word;
        public int Start, End;
        public bool Corrected = false;
    }


    /// <summary>
    /// فئة تفصيل الكلمة العربية من جذرها وإعرابها في الجملة،
    /// </summary>
    public class WordInfo
    {
        public WordInfo()
        {
            Prefix = new WordPrefix();
            Suffix = new WordSuffix();
            Word = "";
            Meaning = "";
            Template = "";
        }
        public WordPrefix Prefix { get; set; }
        public WordSuffix Suffix { get; set; }
        public ArabicRoot Root { get; set; }

        /// <summary>
        /// حركات التشكيل لأصل الكلمة دون السوابق واللواحق
        /// </summary>
        public string Diacritics { get; set; }

        /// <summary>
        /// أصل الكلمة دون سوابق أو لواحق
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// وزن الكلمة لعرضه ضمن تفاصيل التحليل
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// ترميز معنى الكلمة دون معنى السابقة واللاحقة
        /// </summary>
        public string Meaning { get; set; }
        public string Replacer { get; set; }

        /// <summary>
        /// قائمة الإعرابات الممكنة للكلمة
        /// </summary>
        public List<MyInterpretation> Interpretations { get; set; }
        string CompleteDiacritics
        {
            get
            {
                if (Interpretations != null && Interpretations.Count > 0)
                {
                    return Interpreter.FinishDiacritics(Prefix.Tashkeel + Diacritics + Suffix.Tashkeel, Interpretations[0].Meaning);
                }
                return Interpreter.FinishDiacritics(Prefix.Tashkeel + Diacritics + Suffix.Tashkeel, Meaning);
            }
        }

        public override string ToString()
        {
            return Tashkeel.SetTashkeel(Prefix.Text + Word + Suffix.Text, CompleteDiacritics);
        }
        /// <summary>
        /// المعنى بالكامل؛ ناتج دمج معنى السابقة مع أصل الكلمة مع اللاحقة
        /// </summary>
        /// <returns></returns>
        public string CompleteMeaning()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((Prefix.Meaning.Length > 0) ? Prefix.Meaning + "+" : "");
            sb.Append(Meaning);
            sb.Append((Suffix.Meaning.Length > 0) ? "+" + Suffix.Meaning : "");
            return sb.ToString();
        }

        /// <summary>
        /// تصنيف الكلمة إذا كان لها تصنيف خاص
        /// </summary>
        public string SpecialClass { get; set; }
    }
    /// <summary>
    /// يخزن معلومات عن حروف الجذر، وقاعدة اشتقاقه من الكلمة، ونوع الاشتقاق للأسماء، وهل الجذر مؤكد توافقه مع الوزن أم لا
    /// </summary>
    public struct ArabicRoot
    {
        public string Root { get; set; }
        public Morphology.RootCompatibility RootCompatibility { get; set; }
        public string DerivationRules { get; set; }
    }
}
enum WordType
{
    مهملة, // مثلا حرف أو اشارة
    إضافة_على_,
    rr, tt, yy

}
