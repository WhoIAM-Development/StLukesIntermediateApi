using IntermediateAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpResponseEnhancements
    {
        public string DeviceId { get; set; }
        public string TrueIp { get; set; }
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
        public string EmailDomin { get; set; }
        public string EmailPattern { get; set; }

        public void AugmentWithAdditionalProperties(DeviceAttribute attribute, CalculatedFeature calculatedFeature)
        {
            if (attribute != null)
            {
                TrueIp = attribute.TrueIp;
                DeviceId = attribute.DeviceId;
                DeviceCountryCode = attribute.DeviceCountryCode;
                DeviceState = attribute.DeviceState;
                DevicePostalCode = attribute.DevicePostalCode;
                DeviceAsn = attribute.DeviceAsn;
                Platform = attribute.Platform;
                BrowserUserAgentLanguages = attribute.BrowserUserAgentLanguages;
                BrowserUserAgent = attribute.BrowserUserAgent;
                TcpDistance = attribute.TcpDistance;
                Carrier = attribute.Carrier;
                IpRoutingType = attribute.IpRoutingType;
                Proxy = attribute.Proxy;
                UserAgentPlatform = attribute.UserAgentPlatform;
                UserAgentBrowser = attribute.UserAgentBrowser;
                UserAgentOperatingSystem = attribute.UserAgentOperatingSystem;
            }
            if (calculatedFeature != null)
            {
                EmailDomin = calculatedFeature.EmailDomin;
                EmailPattern = calculatedFeature.EmailPattern;
            }

        }
    }
}
