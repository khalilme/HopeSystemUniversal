using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.ComponentModel;
using System.Data.SqlClient;

namespace MyClasses
{
    public class Analyzer
    {
        public static OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='|DataDirectory|\\DB.mdb'");
        public static SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=HaifaDB;Integrated Security=True");

        static private int Corrections;
        static private int NotRecognized = 0;

        public static int NotRecognizedWords
        {
            get { return NotRecognized; }
        }
        static public int CorrectedWords
        {
            get { return Corrections; }
            set { Corrections = value; }
        }
        public static List<List<WordInfo>> AllWordsInfo;//قائمة معلومات الكلمات
        public static List<ArabicWord> ArabicWords; //قائمة بالكلمات العربية ضمن النص المعالج
        public static List<List<MyInterpretation>> AllWordsInterpretations = new List<List<MyInterpretation>>();

        public static void LoadInterpreterData()
        {
            MyInterpretation.InitializeInterpretations();
            GrammarRelation.InitializeRules();
        }

        public static string AnalyzeText(string TextToParse, ref BackgroundWorker BackGroundProcess)
        {

            Corrections = 0;
            NotRecognized = 0;
            ArabicWords = ExtractArabicWords(TextToParse);
            //للاحتفاظ باحتمالات النطق المختلفة لكل كلمة لأغراض التعديل
            AllWordsInfo = new List<List<WordInfo>>();
            StringBuilder OutPutText = new StringBuilder();
            conn.Open();
            con.Open();
            LoadInterpreterData();
            for (int i = 0; i < ArabicWords.Count; i++) //التحليل الصرفي
            {
				//
				///http://ar.wikipedia.org/wiki/%D8%B2%D8%A7%D8%A6%D8%AF%D8%A9_(%D9%84%D8%BA%D8%A9)
				List<WordInfo> NewWordInfo = ProcessWord(ArabicWords[i]); //ابدأ معالجة الكلمة الجديدة
                AllWordsInfo.Add(NewWordInfo); //أضف ناتج المعالجة لمعلومات الكلمات
                if (NewWordInfo.Count == 0) //إذا لم يجد تفسير للكلمة
                { 
                    //فشل تفسير الكلمة والتشكيل بناء على تتابع الحروف -غالبا للكلمات الأعجمية
                    //هذه الخوارزمية تحتاج تحسين
                    //للتعامل مع الكلمات الأعجمية المضاف لها ألف ولام أو ياء نسب
                    WordInfo NewWord = new WordInfo();
                    NewWord.Word = ArabicWords[i].word;
                    NewWord.Diacritics = Tashkeel.Guess(ArabicWords[i].word);
                    NewWordInfo.Add(NewWord);
                    NotRecognized += 1;
                }
                BackGroundProcess.ReportProgress((i + 1) * 100 / ArabicWords.Count);//تحديث شريط التقدم
            }

            int LastAdded = -1;
            WordInfo WordToAdd;
            GrammarRelation GR;
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                GR = new GrammarRelation();
                i += Interpreter.StartInterpreting(ref GR, i, "", 1, false);
            }
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                WordToAdd = AllWordsInfo[i][0];
                OutPutText.Append(TextToParse.Substring(LastAdded + 1, ArabicWords[i].Start - (LastAdded + 1)));
                OutPutText.Append(WordToAdd.ToString());
                LastAdded = ArabicWords[i].End; //آخر حرف أضيف هو آخر حروف الكلمة 
                //إضافة الحروف غير العربية التالية إلى المخرجات كما هي
                if (i + 1 == ArabicWords.Count && LastAdded + 1 < TextToParse.Length)
                {
                    OutPutText.Append(TextToParse.Substring(LastAdded + 1));
                }
            }

            conn.Close();
            con.Close();
            return OutPutText.ToString();
        }
        public static string AnalyzeText(string TextToParse)
        {

            Corrections = 0;
            NotRecognized = 0;
            ArabicWords = ExtractArabicWords(TextToParse);
            //للاحتفاظ باحتمالات النطق المختلفة لكل كلمة لأغراض التعديل
            AllWordsInfo = new List<List<WordInfo>>();
            StringBuilder OutPutText = new StringBuilder();
            conn.Open();
            con.Open();
            LoadInterpreterData();
            for (int i = 0; i < ArabicWords.Count; i++) //التحليل الصرفي
            {
                ///http://ar.wikipedia.org/wiki/%D8%B2%D8%A7%D8%A6%D8%AF%D8%A9_(%D9%84%D8%BA%D8%A9)
                List<WordInfo> NewWordInfo = ProcessWord(ArabicWords[i]); //ابدأ معالجة الكلمة الجديدة
                AllWordsInfo.Add(NewWordInfo); //أضف ناتج المعالجة لمعلومات الكلمات
                if (NewWordInfo.Count == 0) //إذا لم يجد تفسير للكلمة
                {
                    //فشل تفسير الكلمة والتشكيل بناء على تتابع الحروف -غالبا للكلمات الأعجمية
                    //هذه الخوارزمية تحتاج تحسين
                    //للتعامل مع الكلمات الأعجمية المضاف لها ألف ولام أو ياء نسب
                    WordInfo NewWord = new WordInfo();
                    NewWord.Word = ArabicWords[i].word;
                    NewWord.Diacritics = Tashkeel.Guess(ArabicWords[i].word);
                    NewWordInfo.Add(NewWord);
                    NotRecognized += 1;
                }
            }

            int LastAdded = -1;
            WordInfo WordToAdd;
            GrammarRelation GR;
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                GR = new GrammarRelation();
                i += Interpreter.StartInterpreting(ref GR, i, "", 1, false);
            }
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                WordToAdd = AllWordsInfo[i][0];
                OutPutText.Append(TextToParse.Substring(LastAdded + 1, ArabicWords[i].Start - (LastAdded + 1)));
                OutPutText.Append(WordToAdd.ToString());
                LastAdded = ArabicWords[i].End; //آخر حرف أضيف هو آخر حروف الكلمة 
                //إضافة الحروف غير العربية التالية إلى المخرجات كما هي
                if (i + 1 == ArabicWords.Count && LastAdded + 1 < TextToParse.Length)
                {
                    OutPutText.Append(TextToParse.Substring(LastAdded + 1));
                }
            }

            conn.Close();
            con.Close();
            return OutPutText.ToString();
        }


        /// <summary>
        /// استخراج الكلمات العربية المحتوية على الأحرف العربية المعتادة
        /// </summary>
        /// <param name="EntireText"></param>
        /// <returns></returns>
        public static List<ArabicWord> ExtractArabicWords(string EntireText)
        {
            int WordStartAt = -1;
            int WordEndAt = -1;
            int Unicode;
            List<ArabicWord> ArabicWords = new List<ArabicWord>();
            for (int i = 0; i < EntireText.Length; i++)
            {
                Unicode = (int)EntireText[i];
                if (Unicode >= 1569 && Unicode <= 1618) //إذا كان الحرف من الحروف العربية المعتادة
                {
                    if (WordStartAt == -1) WordStartAt = i;
                    if (i + 1 == EntireText.Length)
                    {
                        WordEndAt = i;
                    }
                }
                else //حرف غير عربي
                {
                    if (WordStartAt >= 0)
                    {
                        WordEndAt = i - 1;
                    }
                }

                if (WordStartAt >= 0 && WordEndAt >= 0) //كلمة عربية كاملة
                {
                    ArabicWord NewWord = new ArabicWord();
                    NewWord.Start = WordStartAt;
                    NewWord.End = WordEndAt;
                    NewWord.word = EntireText.Substring(WordStartAt, WordEndAt - WordStartAt + 1);
                    ArabicWords.Add(NewWord);
                    WordStartAt = -1;
                    WordEndAt = -1;
                }
            }
            return ArabicWords;
        }

        /// <summary>
        /// الإجراء الرئيسي في التحليل الصرفي؛ يقوم بتحليل الكلمة وإنتاج قائمة باحتمالات التحليل الصرفي.
        /// </summary>
        /// <param name="WordToProcess"></param>
        /// <returns></returns>
        private static List<WordInfo> ProcessWord(ArabicWord WordToProcess)
        {
            List<WordInfo> CurrentWordInfo = new List<WordInfo>();
            WordInfo NewInfo;

            Morphology.CheckForSpecialWord(WordToProcess, ref CurrentWordInfo);
            if (CurrentWordInfo.Count > 0) goto AddWord;
            Morphology.CheckForPronoun(WordToProcess, ref CurrentWordInfo);
            List<WordPrefix> ValidPrefixes = WordPrefix.CheckPrefix(WordToProcess.word);
            List<WordSuffix> ValidSuffixes = WordSuffix.CheckSuffixes(WordToProcess.word);
            //المعاني الافتراضية عند عدم وجود إضافات
            ValidPrefixes.Add(new WordPrefix() { WordClass = "N1" });//الأسماء نكرة
            ValidPrefixes.Add(new WordPrefix() { WordClass = "V1" });//فعل ماض
            ValidPrefixes.Add(new WordPrefix() { WordClass = "V3" });//فعل أمر

            ValidSuffixes.Add(new WordSuffix() { WordClass = "N001" });//اسم مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V10311" });//فعل ماض غائب مفرد مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V20211" });//فعل مضارع مخاطب مفرد مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V201" });//فعل مضارع متكلم
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V2031" });//فعل مضارع غائب مفرد
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V30011" });//فعل أمر مفرد مذكر
            List<string[]> Result = new List<string[]>();
            string Stem;
            for (int i = 0; i < ValidPrefixes.Count; i++)
            {
                for (int j = 0; j < ValidSuffixes.Count; j++)
                {
                    Result = new List<string[]>();
                    if (WordToProcess.word.Length <= (ValidSuffixes[j].Text.Length + ValidPrefixes[i].Text.Length))
                    {   //طول الإضافات يغطي طول الكلمة بأكملها
                        continue;
                    }
                    List<string> CompatibleAdditions = Morphology.CheckAdditionsCompatibility(ValidPrefixes[i].WordClass, ValidSuffixes[j].WordClass);
                    if (CompatibleAdditions.Count == 0)
                    {   //إضافات غير متوافقة
                        continue;
                    }
                    Stem = WordToProcess.word.Substring(ValidPrefixes[i].Text.Length, WordToProcess.word.Length - (ValidPrefixes[i].Text.Length + ValidSuffixes[j].Text.Length));
                    //ابحث عن الأوزان المتوافقة مع الإضافات المحددة
                    Result = Morphology.LookForTemplate(Stem, CompatibleAdditions, Result);
                    if (Result.Count == 0) continue;

                    /* اختبار وجود جذر للكلمة متوافق مع الوزن المحدد
                     * واختبار توافق الجذر الموجود مع هذا الوزن
                     * يمكن الاستغناء عن بعض هذه الخطوات عند إكمال قاعدة البيانات
                     * 
                     */

                    #region اختبار توافق الوزن والجذر
                    string[] CurrentResult;
                    string CurrentRoot = "";
                    List<ArabicRoot> CheckRootResults = new List<ArabicRoot>();
                    for (int R = 0; R < Result.Count; R++)
                    {
                        CurrentResult = Result[R];
                        Morphology.RootCompatibility RootResult = Morphology.CheckRoot(Stem, CurrentResult[2], CurrentResult[4], CurrentResult[5], CurrentResult[6], CurrentResult[7], ref CurrentRoot);
                        switch (RootResult) //اختبار وجود الجذر حسب الوزن 
                        {
                            case Morphology.RootCompatibility.InvalidRoot:
                                Result.RemoveAt(R);
                                R--;
                                break;
                            case Morphology.RootCompatibility.CompatibleRoot:
                                for (int prev = 0; prev < R; prev++)
                                {
                                    //عثر على جذر متوافق احذف كل الأوزان السابقة التي ليس لها جذور متوافقة
                                    //if (CurrentResult[2] == Result[prev][2]) //نفس نمط الوزن
                                    {
                                        if (CheckRootResults[prev].RootCompatibility == Morphology.RootCompatibility.IncompatibleValidRoot)
                                        {
                                            Result.RemoveAt(prev);
                                            CheckRootResults.RemoveAt(prev);
                                            R--;
                                            prev--;
                                        }
                                    }
                                }
                                CheckRootResults.Add(new ArabicRoot() { Root = CurrentRoot, RootCompatibility = RootResult });
                                break;
                            case Morphology.RootCompatibility.IncompatibleValidRoot:
                                bool AddThisOne = true;
                                for (int prev = 0; prev < R; prev++)
                                {
                                    if (CurrentResult[2] == Result[prev][2]) //نفس نمط الوزن
                                    {
                                        if (CheckRootResults[prev].RootCompatibility == Morphology.RootCompatibility.CompatibleRoot)
                                        {
                                            AddThisOne = false; //عثر من قبل على جذور متوافقة لها أولوية
                                            Result.RemoveAt(R--);
                                            break;
                                        }
                                    }
                                    //مفاضلة الأوزان من نفس قاعدة الاشتقاق
                                    byte CompareResult = Morphology.CompareRules(CurrentResult[7], Result[prev][7]);
                                    if (CompareResult == 1)
                                    {
                                        CheckRootResults.RemoveAt(prev);
                                        Result.RemoveAt(prev--);
                                        R--;
                                    }
                                    else if (CompareResult == 2)//الوزن المضاف مسبقا أولى
                                    {
                                        AddThisOne = false;
                                        Result.RemoveAt(R--);
                                        break;
                                    }
                                }
                                if (AddThisOne)
                                {
                                    CheckRootResults.Add(new ArabicRoot() { Root = CurrentRoot, RootCompatibility = RootResult, DerivationRules = CurrentResult[7] });
                                }
                                break;
                        }
                    }
                    #endregion

                    for (int R = 0; R < Result.Count; R++)
                    {
                        NewInfo = new WordInfo();
                        NewInfo.Word = Stem;
                        NewInfo.Diacritics = Result[R][1];
                        NewInfo.Template = Result[R][0];
                        NewInfo.Meaning = Result[R][3];
                        NewInfo.Root = CheckRootResults[R];
                        NewInfo.Prefix = ValidPrefixes[i];
                        NewInfo.Suffix = ValidSuffixes[j];
                        Tashkeel.DiacritizeWord(NewInfo);
                        CurrentWordInfo.Add(NewInfo);
                    }
                }
            }

            AddWord:
            for (int W = 0; W < CurrentWordInfo.Count; W++)
            {
                if (CurrentWordInfo[W].Root.RootCompatibility == Morphology.RootCompatibility.CompatibleRoot)
                {
                    WordInfo Temp;
                    for (int prev = W; prev > 0; prev--)
                    {
                        Temp = CurrentWordInfo[prev];
                        CurrentWordInfo[prev] = CurrentWordInfo[prev - 1];
                        CurrentWordInfo[prev - 1] = Temp;
                    }
                }
            }
            return CurrentWordInfo;
        }



        internal static void StoreCorrections()
        {
            throw new NotImplementedException();
        }
    }
}
