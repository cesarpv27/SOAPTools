﻿using System;
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

        public static string STRequestSOAPService(object paramsContainer, string url, string action)
        {
            return GetSOAPRequestBuilder().RequestSOAPService(paramsContainer, url, action);
        }

        public virtual string RequestSOAPService(object paramsContainer, string url, string action)
        {
            var _xmlDocSOAPEnvelope = new XmlDocument();
            _xmlDocSOAPEnvelope.BuildNLoadSOAPEnvelope(paramsContainer, action);

            var request = _xmlDocSOAPEnvelope.CreateWebRequest(url);

            using (WebResponse response = request.GetResponse())
                return response.ReadResponse();
        }

        #endregion

        #region BuildEnvelope

        public static string STBuildEnvelope<T>(T paramsContainer, string action) where T : class
        {
            return GetSOAPRequestBuilder().BuildEnvelope(paramsContainer, action);
        }

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
