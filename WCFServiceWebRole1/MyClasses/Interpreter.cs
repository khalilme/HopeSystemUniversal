using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using WCFServiceWebRole.MyClasses;

namespace MyClasses
{
    public class MyInterpretation
    {
        public string Description { get; set; }
        public string Meaning { get; set; }
        public GrammarRelation SuperRelation { get; set; }

        public static Dictionary<string, string[]> Interpretations;
        public static void InitializeInterpretations()
        {
            HaifaDBEntities km = new HaifaDBEntities();
            var interpretation = from k in km.interpretationn select k;

            Interpretations = new Dictionary<string, string[]>();
            string[] Details;
            foreach(var c in interpretation)
            {
                Details = new string[2];
                Details[0] = c.Description;
                Details[1] = c.Sign;
                Interpretations.Add(c.ID, Details);
            }
        }
    }

    /// <summary>
    /// تعبر عن علاقة جملة تحدد المواقع الإعرابية للكلمات التابعة لها
    /// </summary>
    public class GrammarRelation : MyInterpretation
    {
        public Dictionary<int, MyInterpretation> Elements = new Dictionary<int, MyInterpretation>();
        public int WordsCovered { get; set; }
        public static List<string[]> GrammarRules;
        public static void InitializeRules()
        {
            HaifaDBEntities km = new HaifaDBEntities();
            var ML = from k in km.GrammarRules select k;

            GrammarRules = new List<string[]>();
            string[] Rule;
            foreach (var c in ML)
            {
                Rule = new string[2];
                Rule[0] = c.result;
                Rule[1] = c.expression; 
                GrammarRules.Add(Rule);
            }
        }
    }

    public class Interpreter
    {
        public static int StartInterpreting(ref GrammarRelation Relation, int StartIndex, string ExpectedMeaning, byte MaximumRecursion, bool RecursiveCall)
        {
            int Offset = 0;
            bool ValidRule = true;
                foreach (string[] Rule in GrammarRelation.GrammarRules)
                {

                    if (!CompareMeanings(ExpectedMeaning, Rule[0], out ExpectedMeaning))//القاعدة لا تحقق المعنى المطلوب
                    {
                        continue;
                    }
                    WordInfo CurrentWord = Analyzer.AllWordsInfo[StartIndex][0];

                    string[] expression = Rule[1].Split('+');//فصل مكونات علاقة القاعدة النحوية
                    string ElementMeaning, ElementInterpretation = "";
                    string[] InterpretFound;
                    Relation = new GrammarRelation();
                    ValidRule = true;
                    for (int Element = 0; Element < expression.Length; Element++)//بدء مطابقة القاعدة 
                    {
                        if (StartIndex + Element + Offset >= Analyzer.AllWordsInfo.Count)
                        {
                            ValidRule = false;
                            break;
                        }
                        if (expression[Element].Contains(':'))
                        {
                            //فصل المعنى عن الإعراب
                            ElementMeaning = expression[Element].Substring(0, expression[Element].IndexOf(':')).Trim();
                            ElementInterpretation = expression[Element].Substring(expression[Element].IndexOf(':') + 1).Trim();
                        }
                        else
                        {
                            ElementMeaning = expression[Element].Trim();//حرف من الحروف ليس له إعراب
                        }
                        string InterpretedMeaning;
                        bool ValidElement = CompareMeanings(Analyzer.AllWordsInfo[StartIndex + Element + Offset][0].Meaning, ElementMeaning, out InterpretedMeaning);
                        if (CurrentWord.Interpretations != null)
                        {
                            if (Element == 0 && ValidElement)//العنصر الرئيسي معرب مسبقا ويصلح استخدامه
                            {
                                Relation.Elements.Add(StartIndex + Element + Offset, CurrentWord.Interpretations[0]);
                                continue;
                            }
                            else
                            {
                                ValidRule = false;
                                break;
                            }
                        }

                        for (int Possibility = 0; Possibility < Analyzer.AllWordsInfo[StartIndex + Element + Offset].Count; Possibility++)
                        {
                            if (Possibility > 0)
                            {
                                ValidElement = CompareMeanings(Analyzer.AllWordsInfo[StartIndex + Element + Offset][Possibility].Meaning, ElementMeaning, out InterpretedMeaning);
                                if (ValidElement)
                                {
                                    //انقل المعنى المتوافق لبداية القائمة
                                    WordInfo T = Analyzer.AllWordsInfo[StartIndex + Element + Offset][0];
                                    Analyzer.AllWordsInfo[StartIndex + Element + Offset][0] = Analyzer.AllWordsInfo[StartIndex + Element + Offset][Possibility];
                                    Analyzer.AllWordsInfo[StartIndex + Element + Offset][Possibility] = T;
                                }
                            }
                            if (ValidElement && ElementMeaning[0] == 'T')// إذا كان العنصر حرفا صالحا
                            {
                                MyInterpretation NewInterpret = new MyInterpretation();//إعراب جديد
                                NewInterpret.Meaning = InterpretedMeaning;
                                Relation.Elements.Add(StartIndex + Element + Offset, NewInterpret);
                                NewInterpret.SuperRelation = Relation;
                                break;
                            }
                            if (!MyInterpretation.Interpretations.ContainsKey(ElementInterpretation))//إذا كان الإعراب المخزن وصفه غير موجود 
                            {
                                //خطأ في قاعدة البيانات أو حرف غير صالح للقاعدة
                                if (ElementMeaning[0] == 'T')
                                {
                                    continue;
                                }
                                ValidElement = false;
                                ValidRule = false;
                                break;
                            }
                            InterpretFound = MyInterpretation.Interpretations[ElementInterpretation];//البحث عن معنى رمز الإعراب
                            if (ValidElement)
                            {
                                MyInterpretation NewInterpret = new MyInterpretation();//إعراب جديد
                                NewInterpret.Description = InterpretFound[0];//نحميل وصف الإعراب
                                //if (ElementMeaning[0] == 'N')//إضافة الإعراب إلى المعنى حسب نوع الكلمة
                                //{
                                //    CompareMeanings(InterpretedMeaning, "N000" + InterpretFound[1], out ElementMeaning);
                                //}
                                //else if (ElementMeaning[0] == 'V')
                                //{
                                //    CompareMeanings(InterpretedMeaning, "V000000" + InterpretFound[1], out ElementMeaning);
                                //}
                                NewInterpret.Meaning = InterpretedMeaning;
                                Relation.Elements.Add(StartIndex + Element + Offset, NewInterpret);
                                NewInterpret.SuperRelation = Relation;
                                break;
                            }
                            else if (MaximumRecursion > 0 && Element > 0)
                            {
                                //إذا كان العنصر لا يصلح للقاعدة وليس العنصر الرئيسي بها
                                //وهناك فرصة للجمل المتداخلة 
                                //ابحث عن مجموعة كلمات تحقق المعنى المطلوب
                                GrammarRelation SubRelation = new GrammarRelation();
                                int ModifiedOffset = StartInterpreting(ref SubRelation, StartIndex + Element + Offset, ElementMeaning, --MaximumRecursion, true);
                                if (ModifiedOffset == 0)
                                {
                                    //معنى الكلمة غير صالح؛ ابحث باقي احتمالات المعنى
                                    continue;
                                }
                                //إذا نجح البحث عن مجموعة كلمات
                                Offset += ModifiedOffset;
                                SubRelation.Description = InterpretFound[0];
                                SubRelation.Meaning = InterpretedMeaning;
                                SubRelation.SuperRelation = Relation;
                                Relation.Elements.Add(-1, SubRelation);
                                ValidElement = true;
                                break;
                            }
                        }
                        if (!ValidElement)
                        {
                            ValidRule = false;
                            break;
                        }
                    }
                    if (!ValidRule) continue;
                    if (!RecursiveCall) ApplyGrammarRelation(Relation);
                    break;
                }
                if (ValidRule)
                {
                    return Relation.Elements.Count;
                }
                else return 0;
        }

        public static void ApplyGrammarRelation(GrammarRelation SuperRelation)
        {
            GrammarRelation GR;
            foreach (int WordIndex in SuperRelation.Elements.Keys)
            {
                GR = SuperRelation.Elements[WordIndex] as GrammarRelation;
                if (GR != null)
                {
                    ApplyGrammarRelation(GR);
                }
                else
                {
                    string InterpretedMeaning = Analyzer.AllWordsInfo[WordIndex][0].Meaning;
                    string InterpretedClass=Analyzer.AllWordsInfo[WordIndex][0].SpecialClass;
                    if (Analyzer.AllWordsInfo[WordIndex][0].Interpretations == null)
                    {
                        Analyzer.AllWordsInfo[WordIndex][0].Interpretations = new List<MyInterpretation>();
                    }
                    Analyzer.AllWordsInfo[WordIndex][0].Interpretations.Add(SuperRelation.Elements[WordIndex]);

                    string Temp;
                    for (int i = 1; i < Analyzer.AllWordsInfo[WordIndex].Count; i++)
                    {

                        if (CompareMeanings(Analyzer.AllWordsInfo[WordIndex][i].Meaning, InterpretedMeaning, out Temp)
                            && Analyzer.AllWordsInfo[WordIndex][i].SpecialClass == InterpretedClass)
                        {
                            if (Analyzer.AllWordsInfo[WordIndex][i].Interpretations == null)
                            {
                                Analyzer.AllWordsInfo[WordIndex][i].Interpretations = new List<MyInterpretation>();
                            }
                            Analyzer.AllWordsInfo[WordIndex][i].Interpretations.Add(SuperRelation.Elements[WordIndex]);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// تشكيل الكلمة بناء على الإعراب، أو حالة الإعراب الافتراضية
        /// </summary>
        /// <param name="Diacritics"></param>
        /// <param name="Meaning"></param>
        /// <returns></returns>
        public static string FinishDiacritics(string Diacritics, string Meaning)
        {
            if (Meaning.Length == 0) return Diacritics;
            if (Diacritics.IndexOf('!') < 0 && Diacritics.IndexOf('#') < 0) return Diacritics; //لا مكان للعلامات الإعرابية
            char[] Symbols = { '!', '#' };
            int position = Diacritics.IndexOfAny(Symbols);
            switch (Meaning[0])
            {
                case 'N'://الكلمة اسم
                    if (Meaning[1] != '1' && Meaning[1] != '2' && Meaning[1] != '0')
                    {
                        return Diacritics; //التشكيل للأسماء المعربة فقط
                    }
                    if (Meaning.Length < 5 || Meaning[4] == '2' || Meaning[3] == '0')
                    {
                        if (Meaning[1] == '1' && (Meaning.Length > 5 && Meaning[5] != '1' || Meaning.Length <= 5))//نكرة وليس مضافا
                            {
                                Diacritics = Diacritics.Replace('#', 'U');
                                return Diacritics.Replace('!', 'O');//تنوين ضمتين
                            }
                            Diacritics = Diacritics.Replace('#', '5');
                            return Diacritics.Replace('!', '2');
                    } 
                    switch(Meaning[4])
                    {
                        case '1'://اسم منصوب 
                            if (Meaning[2] == '3' && Meaning[3] == '2')//جمع مؤنث سالم
                            {
                                if (Meaning[1] == '1' && Meaning.Length > 5 && Meaning[5] != '1')//نكرة وليس مضافا
                                {
                                    return Diacritics.Replace('!', 'E');//تنوين كسرتين
                                }
                                return Diacritics.Replace('!', '3');//كسرة فقط
                            }
                            else
                            {
                                if (Meaning[1] == '1' && (Meaning.Length > 5 && Meaning[5] != '1' || Meaning.Length <= 5))//نكرة وليس مضافا
                                {
                                    return Diacritics.Replace('!', 'A');//تنوين فتحتين
                                }
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                        case '3':
                            if (Meaning[1] == '1' && (Meaning.Length > 5 && Meaning[5] != '1' || Meaning.Length <= 5))//نكرة وليس مضافا
                            {
                                Diacritics = Diacritics.Replace('#', 'I');
                                return Diacritics.Replace('!', 'E');//تنوين كسرتين
                            }
                            Diacritics = Diacritics.Replace('#', '6');
                            return Diacritics.Replace('!', '3');
                    }
                    break;
                case 'V':
                    //if (Meaning[1] != '2') return Diacritics;//الإعراب للفعل المضارع فقط
                    switch (Meaning[1])
                    {
                        case '1':
                            if (Meaning.Length < 8 || Meaning[7] == '1' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            else if (Meaning[7] == '2')
                            {
                                Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                            }
                            break;
                        case '2':
                            if (Meaning.Length < 8 || Meaning[7] == '2' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '5' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '2' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            switch (Meaning[7])
                            {
                                case '1':
                                    if (Diacritics[position] == '#')
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                    }
                                    else
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                    }
                                    goto default;
                                case '3':
                                    if (Diacritics[position] == '#')
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                    }
                                    else
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                                    }
                                    goto default;
                                default:
                                    return Diacritics.Replace('!', '2');
                            }
                        case '3':
                            if (Meaning.Length < 8 || Meaning[7] == '1' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            else if (Meaning[7] == '2')
                            {
                                Diacritics = Diacritics.Substring(0, position) + '3' + Diacritics.Substring(position + 1);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return Diacritics;
        }

        /// <summary>
        /// اختبار توافق المعنى وإرجاع المعنى الناتج
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CompareMeanings(string first, string second, out string result)
        {
          
            string longest, shortest;
            StringBuilder merged = new StringBuilder();
            if (first.Length > second.Length)
            {
                longest = first;
                shortest = second;
            }
            else
            {
                longest = second;
                shortest = first;
            }

            for (int i = 0; i < shortest.Length; i++)
            {

                if (shortest[i] != longest[i] && longest[i] != '0' && shortest[i] != '0')
                {
                    result = "";
                    return false;
                }
                else if (longest[i] == '0')
                {
                    merged.Append(shortest[i]);
                }
                else
                {
                    merged.Append(longest[i]);
                }
            }
            merged.Append(longest.Substring(shortest.Length));
            result = merged.ToString();
            return true;
        }

        public static string MeaningOf(string Symbol) //تحويل المعنى المرمز إلى نص مقروء
        {
            if (Symbol == null || Symbol == "") return "";
            Symbol = Symbol.ToUpper();
            StringBuilder sb = new StringBuilder();
            if (Symbol.Length > 0)
                switch (Symbol[0])
                {
                    case 'N':
                        sb.Append("اسم");
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append(" نكرة");
                                    break;
                                case '2':
                                    sb.Append(" معرفة");
                                    break;
                                case '3':
                                    sb.Append(" إشارة للقريب");
                                    break;
                                case '4':
                                    sb.Append(" إشارة للبعيد");
                                    break;
                                case '5':
                                    sb.Append(" موصول");
                                    break;
                                case '6':
                                    sb.Append("، ضمير متكلم");
                                    break;
                                case '7':
                                    sb.Append("، ضمير مخاطب");
                                    break;
                                case '8':
                                    sb.Append("، ضمير غائب");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 2)
                            switch (Symbol[2])
                            {
                                case '1':
                                    sb.Append("، مفرد");
                                    break;
                                case '2':
                                    sb.Append("، مثنى");
                                    break;
                                case '3':
                                    sb.Append("، جمع");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 3)
                            switch (Symbol[3])
                            {
                                case '1':
                                    sb.Append("، مذكر");
                                    break;
                                case '2':
                                    sb.Append("، مؤنث");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 4)
                            switch (Symbol[4])
                            {
                                case '1':
                                    sb.Append("، منصوب");
                                    break;
                                case '2':
                                    sb.Append("، مرفوع");
                                    break;
                                case '3':
                                    sb.Append("، مجرور");
                                    break;
                            }
                        sb.Append('.');
                        break;
                    case 'V':
                        sb.Append("فعل");
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append(" ماض");
                                    break;
                                case '2':
                                    sb.Append(" مضارع");
                                    break;
                                case '3':
                                    sb.Append(" أمر");
                                    break;
                                default:
                                    return sb.ToString();
                            }
                        else break;
                        if (Symbol.Length > 2)
                            switch (Symbol[2])
                            {
                                case '1':
                                    sb.Append(" لازم");
                                    break;
                                case '2':
                                    sb.Append(" متعد");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 3)
                        {
                            if (Symbol[1] != '3')//ليس فعل أمر
                            {
                                switch (Symbol[3])
                                {
                                    case '1':
                                        sb.Append(" والفاعل متكلم");
                                        break;
                                    case '2':
                                        sb.Append(" والفاعل مخاطب");
                                        break;
                                    case '3':
                                        sb.Append(" والفاعل غائب");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else break;
                        if (Symbol.Length > 4)
                            switch (Symbol[4])
                            {
                                case '1':
                                    sb.Append(" مفرد");
                                    break;
                                case '2':
                                    sb.Append(" مثنى");
                                    break;
                                case '3':
                                    sb.Append(" جمع");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 5)
                            switch (Symbol[5])
                            {
                                case '1':
                                    sb.Append(" مذكر");
                                    break;
                                case '2':
                                    sb.Append(" مؤنث");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 6)
                            switch (Symbol[6])
                            {
                                case '1':
                                    sb.Append(" مبني للمعلوم");
                                    break;
                                case '2':
                                    sb.Append(" مبني للمجهول");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 7)
                            if (Symbol[1] == '2')//خاص بإعراب المضارع
                            {
                                switch (Symbol[7])
                                {
                                    case '1':
                                        sb.Append(" منصوب");
                                        break;
                                    case '2':
                                        sb.Append(" مرفوع");
                                        break;
                                    case '3':
                                        sb.Append(" مجزوم");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else break;
                        sb.Append(".");
                        break;
                    case 'T':
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append("حرف جر.");
                                    break;
                                case '2':
                                    sb.Append("حرف عطف.");
                                    break;
                                case '3':
                                    sb.Append("أداة نفي.");
                                    break;
                                case '4':
                                    sb.Append("أداة استفهام.");
                                    break;
                                case '5':
                                    sb.Append("أداة شرط.");
                                    break;
                                case '6':
                                    sb.Append("أداة استثناء.");
                                    break;
                                case '7':
                                    sb.Append("من إن وأخواتها.");
                                    break;
                                case '8':
                                    sb.Append("ظرف مكان أو زمان.");
                                    break;
                                default:
                                    sb.Append("حرف.");
                                    break;
                            }

                        break;
                    default:
                        switch (Symbol)
                        {
                            case "PN":
                                sb.Append("(اسم علم)");
                                break;
                            case "ST1":
                                sb.Append("(تجزم الفعل المضارع)");
                                break;
                            default:
                                sb.Append("فشل تفسير الكلمة.");
                                break;
                        }
                        break;
                }
            return sb.ToString();
        }
    }
}
