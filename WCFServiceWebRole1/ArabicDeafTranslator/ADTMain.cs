using MyClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCFServiceWebRole.MyClasses;

namespace WCFServiceWebRole.ArabicDeafTranslator
{
    public class ADTMain
    {

        public string ProcessedText { get; set; }


        public string DoWork(string yx)
        {
            SentenseAnalyzer sn = new SentenseAnalyzer(yx);
            ProcessedText = Analyzer.AnalyzeText(sn.FirstProccessOnSentense);
          return  DeafConvert();
        }



        string DeafConvert()
        {
       // ذهب خليل إلى البحر
            int row = 0;
            foreach (var c in Analyzer.AllWordsInfo)
            {
                int index = 0;
                foreach (var d in c)
                {
                    if (string.IsNullOrEmpty(d.Prefix.Replacer) == false)
                    {

                        var ex = new DeafSentensePriority(d.Prefix.Text, d.Prefix.Replacer, row, index, "");

                        AAli.Add(ex);
                    }
                    AAli.Add(new DeafSentensePriority(" ", d.Word, row, index, Interpreter.MeaningOf(d.Meaning)));

                    if (string.IsNullOrEmpty(d.Suffix.Replacer) == false)
                    {
                        var ex = new DeafSentensePriority(d.Suffix.Text, d.Suffix.Replacer, row, index, "");
                        var cou = (from k in AAli where k.Row == row && k.Word == d.Word select k).Count();
                        AAli.Add(ex);
                    }
                    //li.Add(new DeafSentensePriority(" ", d.ToString(), row, index, d.Interpretations == null ? "" : d.Interpretations[0].Description));
                    index += 1;
                }


                row += 1;
            }
            //int index = -1;
            //foreach(var c in li)
            //{
            //    li.Add(new DeafSentensePriority(" ", c.Word, c.Row, c.Index, c.Interpretations));

            //}
         return   Write2Deaf();
        }

   

        

        //private void txtOut_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    btnDetails.IsEnabled = false;
        //    btnModify.IsEnabled = false;
        //    if (txtOut.SelectionLength == 0)
        //    {
        //        lstPossibilities.Items.Clear();
        //        SelectedWordIndex = -1;
        //        return;
        //    }
        //    List<ArabicWord> prevWords = Analyzer.ExtractArabicWords(txtOut.Text.Substring(0, txtOut.SelectionStart));
        //    SelectedWordIndex = prevWords.Count;
        //    lstPossibilities.Items.Clear();
        //    //إذا لم تكن الكلمة منتقاة بشكل صحيح لا تعدل
        //    if (SelectedWordIndex == Analyzer.ArabicWords.Count || Tashkeel.Remove(txtOut.SelectedText.Trim()) != Analyzer.ArabicWords[SelectedWordIndex].word) return;

        //    for (int i = 0; i < Analyzer.AllWordsInfo[SelectedWordIndex].Count; i++)
        //    {
        //        lstPossibilities.Items.Add(Analyzer.AllWordsInfo[SelectedWordIndex][i]);
        //    }
        //    txtAnalysis.Text = "التحليل: " + Interpreter.MeaningOf(Analyzer.AllWordsInfo[SelectedWordIndex][0].Meaning);
        //    btnModify.IsEnabled = true;
        //    btnDetails.IsEnabled = true;
        //}

    
        List<DeafSentensePriority> AAli = new List<DeafSentensePriority>();
     

        protected string Write2Deaf()
        {
            string txt = "";
            string txtFromLast = "";
         



            var fa3l = (from k in AAli where k.Type.Contains("فاعل مرفوع") select k);
            foreach (var c in fa3l)
            {
                txt += c.Word + " + ";
                AAli.Remove(c);
            }


            var ver = (from k in AAli where k.Interpretations.StartsWith("فعل") select k).FirstOrDefault();
            if (ver != null)
            {


                try
                {
                    var v = (from k in AAli where k.Row > ver.Row select k).FirstOrDefault();


                    foreach (var c in AAli.Where(n => n.Row == v.Row))
                        c.Row -= 1;


                    foreach (var c in AAli.Where(n => n.Row == ver.Row))
                        c.Row += 1;
                }
                catch (Exception)
                {

                }

            }








            var nafi = (from k in AAli where k.Word.Contains("إشارة نفي") select k).ToList();
            bool NafiVerbDone = false;
            foreach (var c in nafi)
            {
                txt += c.Word + " + ";
                AAli.Remove(c);

                if (NafiVerbDone == false)
                {
                    var verb = (from k in AAli where k.Interpretations.StartsWith("فعل") select k).FirstOrDefault();
                    if (verb != null)
                    {
                        txtFromLast += " + " + verb.Word;
                        AAli.Remove(verb);

                        NafiVerbDone = true;
                    }

                }
            }

            //قد تكون الكلمة الاولى لها خيارات كثيرة ، والثانية خيارات اقل ، فلذلك يجب حل هدا الامر

            // && n.Index == 0
            foreach (var c in AAli.Where(n => n.Index == 0).OrderBy(n => n.Row))
            {
                if (c.Interpretations != null)
                {
                    if (c.Interpretations.StartsWith("فعل ماض"))
                    {
                        c.Word = " ( " + c.Word + " " + " + إشارة الماضي )";
                    }
                }
                txt += c.Word + " + ";
            }
            txt = txt + txtFromLast;
            txt = txt.Trim(" + ".ToCharArray());

            txt = txt.Replace("+  +", " + ");
            SentenseAnalyzer sn = new SentenseAnalyzer(txt);

            return sn.SecondProccessOnSentense;

        }


    }
}
