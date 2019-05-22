using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class HopeService : IHopeService
    {
        public List<Bing.WebResult> WebSearch(string query, int resultReturn )
        {
            BingFunctions b = new BingFunctions();
            return b.WebSearch(query, resultReturn);
        }
        public List<Bing.ImageResult> ImageSearch(string query, int resultReturn)
        {
            BingFunctions b = new BingFunctions();
            return b.ImageSearch(query, resultReturn);
        }

        public AnalyzedCorpus CorpusAnalyzer(string query)
        {
            //Queue<CloudSingleWord> a = new Queue<CloudSingleWord>();
            ATKFunctions.ATKFunctions a = new ATKFunctions.ATKFunctions(query);
            return a.Processor();
        }
    }
}
