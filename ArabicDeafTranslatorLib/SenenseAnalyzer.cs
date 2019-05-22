using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ArabicDeafTranslator.MyClasses;


/// <summary>
/// Summary description for SenenseAnalyzer
/// </summary>
public class SentenseAnalyzer
{

    private readonly string _Sentense;
    public SentenseAnalyzer(string sentense)
    {
        _Sentense = sentense;
        CommonReplacedWords.Add(new WordAndItsReplacer("ما هذا","ماهذا"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما هذه", "ماهذه"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما ذلك", "ماذلك"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما تلك", "ماتلك"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما هو", "ماهو"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما هي", "ماهي"));
        CommonReplacedWords.Add(new WordAndItsReplacer("ما سبب", "ماسبب"));

    }

        //List<WordAndItsReplacer> Replaced = new List<WordAndItsReplacer>();
    private string Analyze()
    {
        string retVal = _Sentense;
        HaifaDeafEntities km = new HaifaDeafEntities();


        /////قائمة الكلمات الثنائية المستبدلة 
        //foreach (var c in toget)
        //{
        //    if (retVal.Contains(c))
        //    {
        //        var rep = new WordAndItsReplacer() { WordID = c, WordReplacer = MainFunctions.GiveID() };
        //        Replaced.Add(rep);
        //        retVal = retVal.Replace(c, rep.WordReplacer);
        //    }
        //}

        // تقطيع الجمل حسب النقطة
        var Paragraphs = MainFunctions.Split(retVal, ".");
        string retSentense = "";
        foreach (var P in Paragraphs)
        {
            string temp = P.Trim();
            // الان هنا سيتم صياغة الجملة بمنظورها النهائي
            if (temp.Contains("!"))
            {
                temp = temp.Replace("!", "");
                retSentense += "تعجب، ";

            }
            if (temp.Contains("?") || temp.Contains("؟"))
            {

                temp = temp.Replace("?", "").Replace("؟", "");
                retSentense += "استفهام، ";
            }

            // إشارة | لكي أقوم بفصل المقدمة عن النهاية
            retSentense = retSentense + temp;
        }
        return retSentense;
    }


    public string FirstProccessOnSentense
    {
        get
        {
            string MySent = _Sentense;
            foreach (var c in CommonReplacedWords)
            {
                if (MySent.Contains(c.WordID))
                    MySent = MySent.Replace(c.WordID, c.WordReplacer);
            }
            return MySent;
        }
    }


    public string SecondProccessOnSentense
    {
        get
        {
            return Analyze();
        }
    }


    public List<WordAndItsReplacer> CommonReplacedWords=new List<WordAndItsReplacer>();
}
public class WordAndItsReplacer
{
    public WordAndItsReplacer(string word,string replacer)
    {
        WordID = word;
        WordReplacer = replacer;
    }
    public string WordID { get; set; }
    public string WordReplacer { get; set; }
}