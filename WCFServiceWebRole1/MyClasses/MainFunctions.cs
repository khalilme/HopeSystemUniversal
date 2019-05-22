using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for MainFunctions
/// </summary>
public static class MainFunctions
{


    //- التخلص من كل علامات الترقيم والتنصيص والحصر.
    // التخلص من آل التعريف من كل الاسماء
//5- تسوية بعض الحروف وارجاعها إلى صيغة واحدة مثل الالف مع الهمزة وايضا مثل التاء المربوطة والهاء وهكذا.

    public static string[] Split(string s, string patttern)
    {
        // استبدال المسافتين بمسافة واحدة واعتبار الجمل القصيرة جمل عادية واستبدال الفاصلة بنقطة
        s = s.Replace("  ", " ").Replace(",", ".").Replace(";", ".").Replace("ـ", "").Replace("\n", ".").Replace(":", ".");
        // s = ReplaceForbiddenWord();
        return s.Split( patttern.ToCharArray());
    }
    private static bool Contains(string input, string wordToFind)
    {
        string pattern = String.Format(@"\b{0}\b", wordToFind);


        Match match = Regex.Match(input, pattern);


        return match.Success;
    }
    private static string ReplaceForbiddenWord(string input, string word)
    {
        string pattern = String.Format(@"\b{0}\b", word);


        string result = Regex.Replace(input, pattern, new String('*', word.Length));


        return result;
    }


    private static List<string> GetSentencesContaining(string input, string wordToFind)
    {
        string pattern = String.Format(@"[^\.]*?\b{0}\b[^\.?!]*[\.?!]", wordToFind);


        MatchCollection matches = Regex.Matches(input, pattern);


        List<string> matchesList = new List<string>();


        foreach (Match match in matches)
        {
            matchesList.Add(match.Value.TrimStart());
        }


        return matchesList;
    }
    static int inc = 0;
    public static string GiveID()
    {
        inc += 1;
        return "اسم " + inc;
    }



}