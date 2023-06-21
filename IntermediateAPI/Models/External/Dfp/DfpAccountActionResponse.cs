using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models.External.Dfp
{
    public class DfpAccountActionResponse
    {
        public List<ResultDetail> ResultDetails { get; set; }
        public Enrichment Enrichments { get; set; }
        public string TransactionReferenceId { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }
}
