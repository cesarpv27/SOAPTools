using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Net;

namespace SOAPTools.Core
{
    public class SOAPRequestBuilder
    {
        static SOAPRequestBuilder _SOAPRequestBuilder;
        private static SOAPRequestBuilder GetSOAPRequestBuilder()
        {
            if (_SOAPRequestBuilder == null)
                _SOAPRequestBuilder = new SOAPRequestBuilder();

            return _SOAPRequestBuilder;
        }

        #region RequestSOAPService

        /// <summary>
        /// Build a SOAP request.
        /// 
        /// <para>Making a SOAP request could be like this:</para>
        /// <para></para>
        /// <para>url: http://www.dneonline.com/calculator.asmx </para>
        /// <para>action: Add</para>
        /// <para>C#:</para>
        /// <para>var soapResponse = SOAPRequestBuilder.STRequestSOAPService(new { intA = 1, intB = 1 }, url, action);</para>
        ///     
        /// </summary>
        /// <param name="paramsContainer">Any property of 'paramsContainer' object will be transformed in parameter of target http method of request</param>
        /// <param name="url">Target SOAP url</param>
        /// <param name="action">Target SOAP method</param>
        /// <param name="header">Http header associated with a request</param>
        /// <param name="contentType">Http Content-type associated with a request</param>
        /// <param name="accept">Http Accept  associated with a request</param>
        /// <param name="httpMethod">Http method associated with a request</param>
        /// <returns>Response read as text</returns>
        public static string STRequestSOAPService(object paramsContainer, string url, string action,
            string header = DefaultConst.Header,
            string contentType = DefaultConst.ContentType,
            string accept = DefaultConst.Accept,
            string httpMethod = DefaultConst.HttpMethod)
        {
            return GetSOAPRequestBuilder().RequestSOAPService(paramsContainer, url, action, header, contentType, accept, httpMethod);
        }

        /// <summary>
        /// Build a SOAP request.
        /// 
        /// <para>Making a SOAP request could be like this:</para>
        /// <para></para>
        /// <para>url: http://www.dneonline.com/calculator.asmx </para>
        /// <para>action: Add</para>
        /// <para>C#:</para>
        /// <para>var soapResponse = new SOAPRequestBuilder().RequestSOAPService(new { intA = 1, intB = 1 }, url, action);</para>
        ///     
        /// </summary>
        /// <param name="paramsContainer">Any property of 'paramsContainer' object will be transformed in parameter of target http method of request</param>
        /// <param name="url">Target SOAP url</param>
        /// <param name="action">Target SOAP method</param>
        /// <param name="header">Http header associated with a request</param>
        /// <param name="contentType">Http Content-type associated with a request</param>
        /// <param name="accept">Http Accept  associated with a request</param>
        /// <param name="httpMethod">Http method associated with a request</param>
        /// <returns>Response read as text</returns>
        public virtual string RequestSOAPService(object paramsContainer, string url, string action,
            string header = DefaultConst.Header,
            string contentType = DefaultConst.ContentType,
            string accept = DefaultConst.Accept,
            string httpMethod = DefaultConst.HttpMethod)
        {
            var _xmlDocSOAPEnvelope = new XmlDocument();
            _xmlDocSOAPEnvelope.BuildNLoadSOAPEnvelope(paramsContainer, action);

            var request = _xmlDocSOAPEnvelope.CreateWebRequest(url, header, contentType, accept, httpMethod);

            using (WebResponse response = request.GetResponse())
                return response.ReadResponse();
        }

        #endregion

        #region BuildEnvelope

        /// <summary>
        /// Build envelope of SOAP request.
        /// 
        /// <para>Sample:</para>
        /// <para>C#:</para>
        /// <para>var xmlEnvelope = SOAPRequestBuilder.STBuildEnvelope(new { intA = 1, intB = 1 }, action);</para>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsContainer">Any property of 'paramsContainer' object will be transformed in parameter of target http method of request</param>
        /// <param name="action">Target SOAP method</param>
        /// <returns>Envelope as text</returns>
        public static string STBuildEnvelope<T>(T paramsContainer, string action) where T : class
        {
            return GetSOAPRequestBuilder().BuildEnvelope(paramsContainer, action);
        }

        /// <summary>
        /// Build envelope of SOAP request.
        /// 
        /// <para>Sample:</para>
        /// <para>C#:</para>
        /// <para>var xmlEnvelope = new SOAPRequestBuilder().BuildEnvelope(new { intA = 1, intB = 1 }, action);</para>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramsContainer">Any property of 'paramsContainer' object will be transformed in parameter of target http method of request</param>
        /// <param name="action">Target SOAP method</param>
        /// <returns>Envelope as text</returns>
        public virtual string BuildEnvelope<T>(T paramsContainer, string action) where T : class
        {
            return BuildEnvelope(BuildHeader(string.Empty) + BuildBody(BuildSoapAction(action, BuildSoapParams(paramsContainer))));
        }

        #endregion

        #region Build soapenv

        protected virtual string BuildEnvelope(string innerText)
        {
            ThrowIfNullOrEmpty(innerText);

            return BuildSoapenv(Envelope, innerText, SoapEnvelopeXmlnsSoapenvAttr, SoapEnvelopeXmlnsTemAttr);
        }

        protected virtual string BuildHeader(string innerText)
        {
            return BuildSoapenv(Header, innerText);
        }

        protected virtual string BuildBody(string innerText)
        {
            ThrowIfNullOrEmpty(innerText);

            return BuildSoapenv(Body, innerText);
        }

        protected virtual string BuildSoapenv(string name, string innerText, params string[] attrs)
        {
            return BuildTag(Soapenv, name, innerText, attrs);
        }

        #endregion

        #region Build soap params

        protected virtual string BuildSoapParams<T>(T paramsContainer) where T: class
        {
            if (paramsContainer == null)
                throw new ArgumentNullException(nameof(paramsContainer));

            return BuildSoapParams(paramsContainer, paramsContainer.GetType().GetProperties());
        }

        protected virtual string BuildSoapParams<T>(T objRequest, PropertyInfo[] objProperties)
        {
            if (objProperties == null || objProperties.Length == 0)
                throw new ArgumentException("No properties found");

            var paramsNameValue = new Dictionary<string, string>(objProperties.Length);
            foreach (PropertyInfo _prop in objProperties)
                try
                {
                    paramsNameValue.Add(_prop.Name, _prop.GetValue(objRequest).ToString());
                }
                catch
                {
                }

            return BuildSoapParams(paramsNameValue);
        }

        protected virtual string BuildSoapParams(Dictionary<string, string> paramsNameValue)
        {
            if (paramsNameValue == null || paramsNameValue.Count == 0)
                throw new ArgumentException("No properties found");

            string soapParams = string.Empty;
            foreach (var _paramNameValue in paramsNameValue)
                soapParams += BuildSoapParam(_paramNameValue.Key, _paramNameValue.Value);

            return soapParams;
        }

        protected virtual string BuildSoapParam(string name, string value)
        {
            ThrowIfNullOrEmpty(name);

            return BuildTem(name, value);
        }

        #endregion

        protected virtual string BuildSoapAction(string action, string innerText)
        {
            ThrowIfNullOrEmpty(action);
            ThrowIfNullOrEmpty(innerText);

            return BuildTem(action, innerText);
        }

        protected virtual string BuildTem(string name, string innerText)
        {
            return BuildTag(Tem, name, innerText);
        }

        protected virtual string BuildTag(string type, string name, string innerText, params string[] attrs)
        {
            var nameAttrs = $"{name}";
            if (attrs != null)
                foreach (var _attr in attrs)
                    nameAttrs += string.IsNullOrEmpty(_attr) ? string.Empty : $" {(_attr)}";

            return string.IsNullOrEmpty(innerText)
                ? $"<{type}:{nameAttrs}/>"
                : $"<{type}:{nameAttrs}>{innerText}</{type}:{name}>";
        }

        protected virtual string SoapEnvelopeXmlnsSoapenvAttr
        {
            get
            {
                return $"{Xmlns}:{Soapenv}='{SoapEnvelopeXmlnsSoapenvAttrValue}'";
            }
        }
        public virtual string SoapEnvelopeXmlnsSoapenvAttrValue { get; set; } = "http://schemas.xmlsoap.org/soap/envelope/";

        protected virtual string SoapEnvelopeXmlnsTemAttr
        {
            get
            {
                return $"{Xmlns}:{Tem}='{SoapEnvelopeXmlnsTemAttrValue}'";
            }
        }
        public virtual string SoapEnvelopeXmlnsTemAttrValue { get; set; } = "http://tempuri.org/";

        protected virtual void ThrowIfNullOrEmpty(string value, string paramName = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{(!string.IsNullOrEmpty(paramName) ? paramName : "String")} is null or empty");
        }

        #region Fixed properties

        protected virtual string Envelope { get; set; } = "Envelope";
        protected virtual string Header { get; set; } = "Header";
        protected virtual string Body { get; set; } = "Body";
        protected virtual string Soapenv { get; set; } = "soapenv";
        protected virtual string Tem { get; set; } = "tem";
        protected virtual string Xmlns { get; set; } = "xmlns";

        #endregion

    }
}
