using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCFServiceWebRole.ArabicDeafTranslator;
using WCFServiceWebRole.ATK;
using WCFServiceWebRole.ATK.POS;
using WCFServiceWebRole.ATK.SARF;
using WCFServiceWebRole.ATKCo;
using WCFServiceWebRole.ATKNER;
using WCFServiceWebRole.MyClasses;

namespace WCFServiceWebRole.ATKFunctions
{

    /// <summary>
    /// دوال المعالجة الطبيعية لللغة العربية
    /// </summary>
    public class ATKFunctions
    {
        //protected readonly string AppID = "12685099-2724-449a-81f9-c7f90965c991";
        protected readonly string AppID = "3aae8afe-5b4c-44c9-b6f7-2c9e4ca2ee08";
        protected string inputText;
        public ATKFunctions(string inputText)
        {
            this.inputText = inputText;
        }
        public string InputText
        {
            get
            {
                return inputText;
            }
        }

        public NamedEntityLister NamedEntityList = new NamedEntityLister();


        /// <summary>
        /// تقوم هذه الدالة بتصحيح الأخطاء اللغوية في الجملة المدخلة
        /// </summary>
        /// <returns></returns>
        public string Speller()
        {
            using (SpellerServiceClient SpellerClient = new SpellerServiceClient("HTTPS_ISpellerService"))
            {
                ErroneousWord[] WrongWords = null;
                string CorrectedText = null;
                Dictionary<string, List<string>> Suggestions = null;

                // Detect mistakes, get suggestions for wrong words and autocorrect the input text
                SpellerErrorCode RetCode = SpellerClient.AutocorrectAndSuggest(AppID, inputText, false, out CorrectedText, out WrongWords);

                if (RetCode == SpellerErrorCode.Success)
                {
                    //// Display the suggested autocorrection for the whole text
                    //textAutocorrectedLine.Text = CorrectedText;

                    // Prepare list of suggestions for non-autocorrected words
                    Suggestions = new Dictionary<string, List<string>>();

                    foreach (ErroneousWord WrongWord in WrongWords)
                    {
                        //// Highlight the wrong words
                        //HighlightWrongWord(WrongWord.Position, WrongWord.Word.Length);

                        // Fill word suggestions for non-autocorrected words
                        Suggestions.Add(WrongWord.Word, new List<string>());
                        foreach (Correction SuggestedCorrection in WrongWord.Corrections)
                        {
                            if (SuggestedCorrection.CorrectionType != CorrectionType.CORRECTION_TYPE_AUTO)
                            {
                                Suggestions[WrongWord.Word].Add(SuggestedCorrection.CorrectionText);
                            }
                        }

                        //// Some processing or user input ...

                        //// If the word is not wrong, it is recommended to report that
                        //// the word is wrongly detected for enhancing Speller performance
                        //SpellerClient.ReportWronglyDetectedWord(AppID, WrongWord.Word, inputText);

                        //// If the suggested corrections are not sufficient, it is recommended to suggest a new correction
                        //SpellerClient.SuggestMissingCorrection(AppID, WrongWord.Word, inputText, YourSuggestedCorrection);
                    }
                    inputText = CorrectedText;
                    return CorrectedText;
                }
                else
                {
                    throw new Exception("Failed with error code " + RetCode.ToString());
                }
            }
        }

        /// <summary>
        /// تحويل النص العامي إلى نص فصيح
        /// </summary>
        /// <returns></returns>
        public string ColloquialConverter()
        {
            using (ColloquialConverterServiceClient converterClient = new ColloquialConverterServiceClient("HTTPS_IColloquialConverterService"))
            {
                string msaText = null;
                // Translates the input colloquial text to modern standard Arabic (MSA) text
                ColloquialConverterErrorCode RetCode = converterClient.ConvertText(AppID, inputText, out msaText);

                if (RetCode == ColloquialConverterErrorCode.Success)
                {
                    //// Display the translated text
                    //textTranslatedLine.Text = msaText;

                    // Some processing or user input ... 

                    //// If there is a better translation, it is recommended to suggest it
                    //converterClient.SuggestConversion(AppID, inputText, suggestedTranslation);
                    //inputText = msaText;
                    return msaText;
                }
                else
                {
                    throw new Exception("Failed with error code " + RetCode.ToString());
                }
            }

        }

        /// <summary>
        /// إيجاد الأسماء والأعلام
        /// </summary>
        /// <returns></returns>
        public NamedEntity[] NamedEntityRecognizer()
        {
        NamedEntity[] namedEntities = null;
            using (ANERServiceClient ANERClient = new ANERServiceClient("HTTPS_IANERService"))
            {
                NERErrorCode errorCode;


                // Setup the Preprocessing / Detection components that will be used by the Arabic NER
                // You may use NEROption.UseSpeller instead of UseAutoCorrector in case of poor Arabic text input.
                NEROption NEROptions = NEROption.UseAllComponents | NEROption.UseAutoCorrector;

                // Extract and Classify named entities
                errorCode = ANERClient.GetArabicNamedEntities(AppID, inputText, NEROptions, out namedEntities);


                foreach (NamedEntity item in namedEntities)
                {
                    string wordreplaced = item.NamedEntityWords[0].WordString;
                    inputText = inputText.Replace(wordreplaced, "#" + wordreplaced);
                    NamedEntityType type = (NamedEntityType)item.NamedEntityType;


                    //if (type == NamedEntityType.Human)
                    //    Console.WriteLine("{0}\t{1}", item.NamedEntityPhrase, "Person");
                    //else if (type == NamedEntityType.Location)
                    //    Console.WriteLine("{0}\t{1}", item.NamedEntityPhrase, "Location");
                    //else if (type == NamedEntityType.Organization)
                    //    Console.WriteLine("{0}\t{1}", item.NamedEntityPhrase, "Organization");


                    NamedItem rw = new NamedItem()
                    {
                        Word = item.NamedEntityPhrase,
                        Type = item.NamedEntityType
                    };

                    NamedEntityList.Add(rw);
                }
                // Report a wrong named entity type.
                //NamedEntityType correctNamedEntityType; // initialize it to the correct Named Entity type that should have been classified into.
                //ANERClient.ReportWronglyDetectedNamedEntity(AppID, item.NamedEntityPhrase, inputText, type, correctNamedEntityType);

                if (errorCode == NERErrorCode.Success)
                {
                  
                    //// Suggest a missing named entity
                    //string missingNamedEntityPhrase; // initialize it to the missing named entity that should have been detected and classified
                    //NamedEntityType namedEntityType; // intialize it to the named entity type of the missing named entity
                    //errorCode = ANERClient.SuggestMissingNamedEntity(AppID, missingNamedEntityPhrase, inputText, namedEntityType);



                }
                else
                {
                    throw new Exception("Failed with error code " + errorCode.ToString());
                }
                return namedEntities;
            }

        }

        public string POSParser()
        {
            string parseTree = "";
            using (ParserServiceClient ParserClient = new ParserServiceClient("HTTPS_IParserService"))
            {
                double score = 0.0;

                // Parse the input Arabic sentence
                ParserErrorCode RetCode = ParserClient.Parse(AppID, inputText, out parseTree, out score);

                if (RetCode == ParserErrorCode.Success)
                {

                    //// If this is a wrong parse, it is recommended to suggest a new parse tree
                    //ParserClient.SuggestParseTree(AppID, arabicSentence, suggestedParseTree);
                }
                else
                {
                    throw new Exception("Failed with error code " + RetCode.ToString());
                }


            }
            return parseTree;
        }


        /// <summary>
        /// morphological analyzer, 
        /// https://www.microsoft.com/en-us/research/project/sarf-morphological-analyzer/
        /// وسيحاكمونهم
        /// </summary>
        /// <returns></returns>
        public SarfAnalysis[] SARF()
        {
            SarfAnalysis[] AllAnalyses = null;
            List<string> RetLi = new List<string>();
            using (SarfServiceClient SarfClient = new SarfServiceClient("HTTPS_ISarfService"))
            {

                // Split an Arabic line of text into tokens
                string[] Tokens = inputText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string Token in Tokens)
                {
                    // Analyze each token using Sarf
                    SarfErrorCode RetCode = SarfClient.AnalyzeToken(AppID, Token, LinguisticMode.Classical, out AllAnalyses);

                    if (RetCode == SarfErrorCode.Success)
                    {
                        foreach (SarfAnalysis an in AllAnalyses)
                        {
                            // Display the analysis
                            RetLi.Add(an.DiacToken + ", " + an.Pos);

                            // Some processing or user input ...

                            //// If the analysis is wrong, it is recommended to report it for enhancing Sarf performance
                            //SarfClient.ReportWrongAnalysis(AppID, Token, textArabicLine.Text, an);
                        }

                        // Some processing or user input ...

                        //// If the analysis is wrong, it is recommended to report it
                        //// by forming a new analysis or modifying an existing analysis
                        //SarfAnalysis newAnalysis = new SarfAnalysis();
                        //// Fill as much fields as available
                        //newAnalysis.DiacToken = SomeDiacritizedWord;
                        //newAnalysis.Stem = WordStem;
                        //newAnalysis.Root = WordRoot;
                        //newAnalysis.Pattern = WordPattern;
                        //newAnalysis.Pos = WordPartOfSpeech;
                        //// Suggest the new analysis
                        //SarfClient.SuggestMissingAnalysis(AppID, Token, textArabicLine.Text, newAnalysis);
                    }
                    else
                        throw new Exception("Failed with error code " + RetCode.ToString());
                }
            }

            return AllAnalyses;
        }
        public AnalyzedCorpus Processor()
        {
            AnalyzedCorpus ac = new AnalyzedCorpus();

            string ou = Speller();
            ou = ColloquialConverter();
            ac.EntitesList = NamedEntityRecognizer();
            Queue<CloudSingleWord> li = new Queue<CloudSingleWord>();
            CloudSingleWord sin = new CloudSingleWord();


            POSParser();



            SentenseAnalyzer sn = new SentenseAnalyzer(ou);
            string st = Analyzer.AnalyzeText(sn.SecondProccessOnSentense);
            ADTMain m = new ADTMain();


            ac.AnalyzedText = m.DoWork(inputText);
        









            //foreach (var c in wo)
            //{
            //    HopeService hs = new HopeService();
            //    switch (c.NamedEntityType)
            //    {
            //        case NamedEntityType.Human:
            //            sin.Type = CloudWordType.NameFingerImage;
            //            c.Image = hs.ImageSearch(c.NamedEntityPhrase, 1);

            //            break;
            //        case NamedEntityType.Location:
            //            sin.Type = CloudWordType.NameImage;
            //            c.Image = hs.ImageSearch(c.NamedEntityPhrase, 1);
            //            break;
            //        case NamedEntityType.Organization:
            //            sin.Type = CloudWordType.NameImage;
            //            rw.Image = hs.ImageSearch(c.NamedEntityPhrase, 1);
            //            break;
            //        case NamedEntityType.NotNamedEntity:
            //            sin.Type = CloudWordType.Word;
            //            break;
            //        default:
            //            break;
            //    }
            //}


            return ac;
        }
    }




    public class NamedEntityLister : List<NamedItem>
    {
        public void AddNewItem(NamedEntity na)
        {
            Add(new NamedItem()
            {
                NamedEntity=na,
                Replacer="#" + na.NamedEntityWords[0].WordString + ""
            });
        }

    }

    public class NamedItem
    {
        public NamedEntity NamedEntity { get; set; }
        public string Replacer { get; set; }
        public string Word { get; set; }
        public NamedEntityType Type { get; set; }
    }
}

