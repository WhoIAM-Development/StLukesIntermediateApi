using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models.Api
{
    public class Enrichment
    {
        public DeviceAttribute DeviceAttributes { get; set; }
        public CalculatedFeature CalculatedFeatures { get; set; }
    }
}
