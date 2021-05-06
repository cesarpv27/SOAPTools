using System;
using System.Xml;
using System.Net;
using System.IO;

namespace SOAPTools.Core
{
    public static class Extensions
    {
        public static void InsertSOAPEnvelope(this HttpWebRequest webRequest, XmlDocument soapEnvelope)
        {
            ThrowIfNull(soapEnvelope, nameof(soapEnvelope));

            using (Stream stream = webRequest.GetRequestStream())
                soapEnvelope.Save(stream);
        }

        public static void BuildNLoadSOAPEnvelope(this XmlDocument _xmlDoc, dynamic dynRequestParams, string action)
        {
            ThrowIfNull(dynRequestParams, nameof(dynRequestParams));
            ThrowIfNullOrEmpty(action, nameof(action));

            _xmlDoc.LoadXml(SOAPRequestBuilder.STBuildEnvelope(dynRequestParams, action));
        }

        public static string GetResponse(this WebResponse response)
        {
            using (var _streamReader = new StreamReader(response.GetResponseStream()))
                return _streamReader.ReadToEnd();
        }

        public static HttpWebRequest CreateWebRequest(this XmlDocument _xmlDocSOAPEnvelope, string url,
            string header = "SOAP:Action",
            string contentType = "text/xml;charset=\"utf-8\"",
            string accept = "text/xml",
            string httpMethod = "POST")
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.InitializeWebRequest(header, contentType, accept, httpMethod);

            webRequest.InsertSOAPEnvelope(_xmlDocSOAPEnvelope);

            return webRequest;
        }

        public static void InitializeWebRequest(this HttpWebRequest webRequest,
            string header = "SOAP:Action",
            string contentType = "text/xml;charset=\"utf-8\"",
            string accept = "text/xml",
            string httpMethod = "POST")
        {
            ThrowIfNullOrEmpty(header, nameof(header));
            ThrowIfNullOrEmpty(contentType, nameof(contentType));
            ThrowIfNullOrEmpty(accept, nameof(accept));
            ThrowIfNullOrEmpty(httpMethod, nameof(httpMethod));

            webRequest.Headers.Add(header);
            webRequest.ContentType = contentType;
            webRequest.Accept = accept;
            webRequest.Method = httpMethod;
        }

        private static void ThrowIfNullOrEmpty(string value, string paramName = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(paramName);
        }

        private static void ThrowIfNull(object value, string paramName = null)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }
    }
}
