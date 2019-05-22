using MyClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WCFServiceWebRole.ArabicDeafTranslator;
using WCFServiceWebRole.MyClasses;

namespace WCFServiceWebRole
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string finalText = TextBox1.Text;
            ATKFunctions.ATKFunctions a = new ATKFunctions.ATKFunctions(finalText);
            Response.Write(a.Processor().AnalyzedText);


            //Response.Write(finalText);
            //(TOP (S (S (PRT (IN س)) (VP (VBP تقوم) (NP (NNP يوبي) (NNP سوفت)) (PP (PREP في) (NP (DTNN الخريف) (DTJJ المقبل))) (PP (PREP ب) (NP (NN طرح) (NP (NP (DTNN الجزء) (DTJJ الثاني)) (PP (PREP من) (NP (NN لعبة) (NP (NNP Wَتcه) (NNP ضْغس))))))))) (CC و) (S (NP (PRP هي)) (VP (VBN تعد) (NP (NN عشاق) (NP (DTNN اللعبة))) (PP (PREP ب) (SBAR (CC أن) (VP (VBP تقدم) (PP (PREP ل) (NP (PRP هم))) (NP (NN جزء)))))))))

            //   var ag = a.SARF();

            //TextBox2.Text = a.POSParser();

            //SentenseAnalyzer sn = new SentenseAnalyzer(TextBox1.Text);
            //string st = Analyzer.AnalyzeText(sn.SecondProccessOnSentense);
            //ADTMain m = new ADTMain();
            //Response.Write(st);

            //Amal
            //    TextBox2.Text = m.DoWork(st);
            //TextBox2.Text = m.DoWork(a.InputText);

            //Analyzer.DeafConvert();


        }
    }
}