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

        public static void BuildNLoadSOAPEnvelope<T>(this XmlDocument _xmlDoc, T paramsContainer, string action) where T: class
        {
            ThrowIfNull(paramsContainer, nameof(paramsContainer));
            ThrowIfNullOrEmpty(action, nameof(action));

            _xmlDoc.LoadXml(SOAPRequestBuilder.STBuildEnvelope(paramsContainer, action));
        }

        public static string ReadResponse(this WebResponse response)
        {
            using (var _streamReader = new StreamReader(response.GetResponseStream()))
                return _streamReader.ReadToEnd();
        }

        public static HttpWebRequest CreateWebRequest(this XmlDocument _xmlDocSOAPEnvelope, string url,
            string header = DefaultConst.Header,
            string contentType = DefaultConst.ContentType,
            string accept = DefaultConst.Accept,
            string httpMethod = DefaultConst.HttpMethod)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.InitializeWebRequest(header, contentType, accept, httpMethod);

            webRequest.InsertSOAPEnvelope(_xmlDocSOAPEnvelope);

            return webRequest;
        }

        public static void InitializeWebRequest(this HttpWebRequest webRequest,
            string header = DefaultConst.Header,
            string contentType = DefaultConst.ContentType,
            string accept = DefaultConst.Accept,
            string httpMethod = DefaultConst.HttpMethod)
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
