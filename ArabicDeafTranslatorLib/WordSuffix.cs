using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace MyClasses
{

    public class WordSuffix
    {
        public WordSuffix()
        {
            Meaning = Text = WordClass = Tashkeel = ConnectedLetter = "";
        }

        /// <summary>
        /// ترميز المعنى إذا كانت اللاحقة تحمل دلالة في ذاتها
        /// </summary>
        public string Meaning { get; set; }

        /// <summary>
        /// نص حروف اللاحقة دون تشكيل.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// ترميز معنى الكلمة التي تدخل عليها اللاحقة.
        /// </summary>
        public string WordClass { get; set; }


        public string Tashkeel { get; set; }

        /// <summary>
        /// حركة الحرف المتصل باللاحقة من أصل الكلمة.
        /// </summary>
        public string ConnectedLetter { get; set; }
        public string Replacer { get; set; }
        public static List<WordSuffix> CheckSuffixes(string Word)
        {
            string[] suffixes = { "ة", "ت", "ا", "ن", "تم", "ك", "كم", "هم", "ه", "ي" };
            List<WordSuffix> ValidSuffixes = new List<WordSuffix>();
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (Word.EndsWith(suffixes[i], StringComparison.Ordinal))
                {
                    OleDbCommand com = new OleDbCommand();
                    com.Connection = Analyzer.con;
                    com.CommandText = "select * from suffixes where StrComp( Right( add , " + suffixes[i].Length + "),'" + suffixes[i] + "',0)=0 order by pri ";
                    OleDbDataReader dread = com.ExecuteReader();
                    while (dread.Read())

                        // strcomp(w1,w2,0)=2
                    {
                        if (Word.EndsWith(dread[0].ToString(), StringComparison.Ordinal))
                        {
                            WordSuffix s = new WordSuffix();
                            s.Text = dread["Add"].ToString();
                            s.Tashkeel = dread["Diacritics"].ToString();
                            s.WordClass = dread["Class"].ToString();
                            s.Meaning = dread["Meaning"].ToString();
                            s.ConnectedLetter = dread["WordLetter"].ToString();
                            s.Replacer = dread["Replacer"].ToString();
                            ValidSuffixes.Add(s);
                        }
                    }
                    dread.Close();
                    break;
                }
            }
            return ValidSuffixes;
        }

      
    }

}
