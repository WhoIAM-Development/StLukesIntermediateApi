using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models.Api
{
    public class DeviceAttribute
    {
        public string TrueIp { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCountryCode { get; set; }
        public string DeviceState { get; set; }
        public string DeviceCity { get; set; }
        public string DevicePostalCode { get; set; }
        public string DeviceAsn { get; set; }
        public string Platform { get; set; }
        public string BrowserUserAgentLanguages { get; set; }
        public string BrowserUserAgent { get; set; }
        public string TcpDistance { get; set; }
        public string Carrier { get; set; }
        public string IpRoutingType { get; set; }
        public string Proxy { get; set; }
        public string UserAgentPlatform { get; set; }
        public string UserAgentBrowser { get; set; }
        public string UserAgentOperatingSystem { get; set; }
    }
}
