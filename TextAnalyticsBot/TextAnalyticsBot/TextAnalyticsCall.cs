using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextAnalyticsBot
{
    public class TextAnalyticsCall
    {
    }
    public class BatchInput
    {
        public List<DocumentInput> Documents { get; set; }
    }
    public class DocumentInput
    {
        public double id { get; set; }
        public string text { get; set; }
    }

    // Classes to store the result from the sentiment analysis
    public class BatchResult
    {
        public List<DocumentResult> Documents { get; set; }
    }
    public class DocumentResult
    {
        public double Score { get; set; }
        public string id { get; set; }
    }

}