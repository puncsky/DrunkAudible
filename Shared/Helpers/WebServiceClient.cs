// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace DrunkAudible.Mobile
{
    public class WebServiceClient : RestClient
    {
        const String API_BASE_URL = "https://192.168.1.3:8080/api";

        public WebServiceClient ()
            : base (API_BASE_URL)
        {
            #if DEBUG
            SSLValidator.OverrideValidation ();
            #endif
        }

        static class SSLValidator
        {
            static bool OnValidateCertificate (
                object sender, X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
            {
                return true;
            }

            public static void OverrideValidation ()
            {
                ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
                ServicePointManager.Expect100Continue = true;
            }
        }
    }
}

