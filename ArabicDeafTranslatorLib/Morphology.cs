using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using ArabicDeafTranslator.MyClasses;
using System.Data.SqlClient;

namespace MyClasses
{

    /// <summary>
    /// إجراءات التحليل الصرفي للكلمات
    /// </summary>
    public class Morphology
    {


        /// <summary>
        /// استخراج الادوات والأحرف والكلمات المهملة
        /// </summary>
        /// <param name="WordToProcess"></param>
        /// <param name="Possibilities"></param>
        public static void CheckForSpecialWord(ArabicWord WordToProcess, ref List<WordInfo> Possibilities)
        {
            WordInfo NewInfo;
            HaifaDeafEntities km = new HaifaDeafEntities();
            var wproc = from k in km.SpecialWords where k.Word == WordToProcess.word select k;
            foreach (var c in wproc)
            {
                NewInfo = new WordInfo();
                NewInfo.Meaning = c.Meaning;
                NewInfo.SpecialClass = c.Class;


                //NewInfo.Word = WordToProcess.word;
                NewInfo.Word = c.Replacer;
                NewInfo.Diacritics = c.diacritics;
                

                Possibilities.Add(NewInfo);
            }
        }

        public static void CheckForPronoun(ArabicWord WordToProcess, ref List<WordInfo> Possibilities)
        {
            WordInfo NewInfo;

            HaifaDeafEntities km = new HaifaDeafEntities();
            var wproc = from k in km.ProperNouns where k.Word == WordToProcess.word select k;
            foreach (var c in wproc)
            {
                NewInfo = new WordInfo();
                NewInfo.Meaning = c.Meaning;
                NewInfo.SpecialClass = "PN";
                NewInfo.Word = WordToProcess.word;
                NewInfo.Diacritics = c.Diacritics;
                Possibilities.Add(NewInfo);
            }
         
        }

        /// <summary>
        /// جلب الكلمة العادية ، ومن ثم استبدال الأحرف الزادة عن الجذر بعلامة سلم
        /// </summary>
        /// <param name="OriginalWord"></param>
        /// <param name="Meanings"></param>
        /// <param name="Results"></param>
        /// <returns></returns>
        public static List<string[]> LookForTemplate(string OriginalWord,List<string> Meanings, List<string[]> Results)
        {
            string mask = OriginalWord.Replace("آ", "ءا");
            string additionals = "أءئؤسلتطمونيهاإ";
            byte RootLetters = 0;
            for (int i = 0; i < mask.Length; i++)
            {
                if (!additionals.Contains(mask[i]))
                {
                    mask = mask.Replace(mask[i], '#');
                    RootLetters++;
                }
            }
            Results = CheckWordMask(OriginalWord, mask, Meanings, Results);
            if (RootLetters == 4 || RootLetters == mask.Length) //أقصى عدد حروف أصلية ممكن
            {
                return Results;
            }
            string temp;
            List<string> Temps = new List<string>();
            Temps.Add(mask);
            for (byte j = 0; j < mask.Length; j++)
            {
                if (mask[j] != '#') //إذا كان الحرف من الحروف الزائدة فيحتمل أن يكون أصليا أو زائدا
                {
                    int Count = Temps.Count;
                    for (int k = 0; k < Count; k++)
                    {
                        temp = Temps[k].Substring(0, j) + '#' + Temps[k].Substring(j + 1);
                        Results = CheckWordMask(OriginalWord, temp, Meanings, Results);
                        Temps.Add(temp);
                    }
                }
            }
            return Results;
        }


        /// <summary>
        /// البحث عن الوزن في جدول WordTemplates وإرجاعه لبدء عمليات التحليل الصرفي
        /// </summary>
        /// <param name="word"></param>
        /// <param name="mask"></param>
        /// <param name="Meanings"></param>
        /// <param name="Results"></param>
        /// <returns></returns>
        public static List<string[]> CheckWordMask(string word, string mask, List<string> Meanings, List<string[]> Results)
        {

            SqlCommand com = new SqlCommand("select * from WordTemplates where Mask='" + mask + "'", Analyzer.conn);
            SqlDataReader dread = com.ExecuteReader();
            string[] fields;
            while (dread.Read())
            {
                //اختبار توافق تشكيل الوزن مع حروف الكلمة
                if (dread["Class"].ToString().StartsWith("V2"))//إذا كان وزن فعل مضارع
                {
                    if (!Tashkeel.CheckTashkeel(word, dread[1].ToString().Substring(1))) continue;//استثناء حرف المضارعة
                }
                else if (!Tashkeel.CheckTashkeel(word, dread[1].ToString()))
                {
                    continue;
                }
                //اختبار توافق نوع الوزن مع النوع المتوقع
                foreach (string M in Meanings)
                {
                    string MergedMeaning;
                    if (Interpreter.CompareMeanings(dread["Class"].ToString(), M, out MergedMeaning))
                    {
                        fields = new string[8];
                        for (int f = 0; f < fields.Length; f++)
                        {
                            if (f == 3)
                            {
                                fields[f] = MergedMeaning;
                                continue;
                            }
                            fields[f] = dread[f].ToString();
                        }
                        Results.Add(fields);
                    }
                }
            }
            dread.Close();
            return Results;
        }

        /// <summary>
        /// مفاضلة الأوزان من نفس قاعدة الاشتقاق لتحسين النتائج
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static byte CompareRules(string first, string second)
        {
            //في حالة الجذور الغير مرتبطة بقواعد الاشتقاق في قاعدة البيانات
            if (first.Length < 2 || second.Length < 2) return 0;//تعريف القاعدة من حرفين على الأقل
            string[] FirstRules = first.Split(',');
            string[] SecondRules = second.Split(',');
            bool SecondGreater = false, FirstGreater = false;
            foreach (string fRule in FirstRules)
            {
                if (fRule[0] == 'X') continue;
                foreach (string sRule in SecondRules)
                {
                    if (fRule[0] == sRule[0])//من نفس قاعدة الاشتقاق
                    {
                        if (fRule[1] != '1' && sRule[1] == '1')//الأولى حالة خاصة من الثانية
                        {
                            FirstGreater = true;
                        }
                        else if (sRule[1] != '1' && fRule[1] == '1')//الثانية حالة خاصة من الأولى
                        {
                            SecondGreater = true;
                        }
                    }
                }
            }
            if (FirstGreater && !SecondGreater)
            {
                return 1;
            }
            else if (SecondGreater && !FirstGreater)
            {
                return 2;
            }
            return 0; //لا صلة بينهما
        }

        public static List<string> CheckAdditionsCompatibility(string PrefixClass, string SuffixClass)
        {
            //اختبر توافق السابقة مع اللاحقة
            string[] PossiblePrefixes = PrefixClass.Split(',');
            string[] PossibleSuffixes = SuffixClass.Split(',');
            List<string> Compatibles = new List<string>();
            foreach (string PreClass in PossiblePrefixes)
            {
                foreach (string SufClass in PossibleSuffixes)
                {
                    string MergedMeaning;
                    if (Interpreter.CompareMeanings(PreClass, SufClass, out MergedMeaning))
                    {
                        Compatibles.Add(MergedMeaning);
                    }
                }
            }
            return Compatibles;
        }


        /// <summary>
        /// حالة الجذر إما أن يكون غير صيح، أو غير متوافق، أو متوافق
        /// </summary>
        public enum RootCompatibility : byte
        {
            InvalidRoot,
            IncompatibleValidRoot,
            CompatibleRoot
        }

        /// <summary>
        /// للتأكد من أن مجموعة من الحروف الأصلية عبارة عن جذر من جذور الكلمات العربية لتصفية نتائج الاحتمالات المختلفة لأقنعة الأوزان.
        /// ومن توافق الجذر عند العثور عليه مع وزن الكلمة، وتحديد نوع ارتباط الوزن بالجذر بما يشمل نوع الاشتقاق للأسماء، وكون الفعل متعد أم لازم.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="mask"></param>
        /// <param name="ف"></param>
        /// <param name="ع"></param>
        /// <param name="ل"></param>
        /// <param name="rules"></param>
        /// <param name="WordRoot"></param>
        /// <returns></returns>
        public static RootCompatibility CheckRoot(string word, string mask, string ف, string ع, string ل, string rules, ref string WordRoot)
        {
            List<string> Roots = new List<string>();
            word = word.Replace('ء', 'أ');
            word = word.Replace('إ', 'أ');
            word = word.Replace('ؤ', 'أ');
            word = word.Replace('ئ', 'أ');
            word = word.Replace("آ", "أا");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < word.Length; i++)
            {
                if (mask[i] == '#')
                {
                    sb.Append(word[i]);
                }
            }

            if (sb.Length > 2) Roots.Add(sb.ToString());

            #region فحص توافق الحروف
            if (ف.Length > 0)
            {
                if (sb.Length < 3)
                {
                    foreach (char item in ف)
                    {
                        Roots.Add(item + sb.ToString());
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ف)
                    {
                        if (sb[0] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return RootCompatibility.InvalidRoot;
                }

            }
            if (ع.Length > 0)
            {
                if (sb.Length < 3)
                {
                    foreach (char item in ع)
                    {
                        Roots.Add(sb[0].ToString() + item + sb[1]);
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ع)
                    {
                        if (sb[1] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return RootCompatibility.InvalidRoot;
                }

            }
            if (ل.Length > 0)
            {
                if (sb.Length < 3)
                {
                    if (ل == "ع")//جذر مضعف
                    {
                        Roots.Add(sb.ToString() + sb[1]);

                    }
                    else
                    {
                        foreach (char item in ل)
                        {
                            Roots.Add(sb.ToString() + item);
                        }
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ل)
                    {
                        if (sb[2] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return RootCompatibility.InvalidRoot;
                }

            }
            #endregion

            //if (Roots[0].Contains("ا"))
            //{
            //    Roots.Add(Roots[0].Replace('ا', 'ي'));
            //    Roots.Add(Roots[0].Replace('ا', 'و'));
            //}
            ////To be improved
            //else 
            if (Roots.Count > 0 && (Roots[0].EndsWith("ى") || Roots[0].EndsWith("ي") || Roots[0].EndsWith("و")))
            {
                Roots.Add(Roots[0].Substring(0, Roots[0].Length - 1) + 'ا');
            }
            SqlCommand com = new SqlCommand();
            com.Connection = Analyzer.conn;
            SqlDataReader dread;
            foreach (string root in Roots)
            {
                com.CommandText = "select * from roots where Root='" + root + "'";
                dread = com.ExecuteReader();
                if (dread.Read())
                {
                    if (dread[1].ToString().Trim().Length > 0 && rules.Trim().Length > 0)
                    {
                        string[] RootRules = dread[1].ToString().Split(',');
                        string[] TemplateRules = rules.Split(',');
                        for (int X = 0; X < RootRules.Length; X++)
                        {
                            for (int Y = 0; Y < TemplateRules.Length; Y++)
                            {
                                if (RootRules[X] == TemplateRules[Y])
                                {
                                    dread.Close();
                                    WordRoot = root;
                                    return RootCompatibility.CompatibleRoot;
                                }
                            }
                        }
                    }
                    dread.Close();
                    WordRoot = root;
                    return RootCompatibility.IncompatibleValidRoot;
                }
                dread.Close();
            }
            return RootCompatibility.InvalidRoot;
        }

    
    }
}
