using System;
using System.Collections.Generic;
using System.Reflection;

namespace SOAPTools.Core
{
    public class SOAPRequestBuilder
    {
        static SOAPRequestBuilder _SOAPRequestBuilder;
        public static string BuildRequest(dynamic objRequest, string methodName)
        {
            if (_SOAPRequestBuilder == null)
                _SOAPRequestBuilder = new SOAPRequestBuilder();

            return _SOAPRequestBuilder._BuildRequest(objRequest, methodName);
        }

        public virtual string _BuildRequest(dynamic objRequest, string methodName)
        {
            return BuildEnvelope(BuildHeader(string.Empty) + BuildBody(BuildSoapMethod(methodName, BuildSoapParams(objRequest))));
        }

        #region Build soapenv

        protected virtual string BuildEnvelope(string innerText)
        {
            ThrowIfNullOrEmpty(innerText);

            return BuildSoapenv(Envelope, innerText, SoapEnvelopeXmlnsSoapenvAttr, SoapEnvelopeXmlnstemAttr);
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

        protected virtual string BuildSoapParams<T>(T objRequest)
        {
            if (objRequest == null)
                throw new ArgumentNullException(nameof(objRequest));

            return BuildSoapParams(objRequest, objRequest.GetType().GetProperties());
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

        protected virtual string BuildSoapMethod(string methodName, string innerText)
        {
            ThrowIfNullOrEmpty(methodName);
            ThrowIfNullOrEmpty(innerText);

            return BuildTem(methodName, innerText);
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

        protected virtual string SoapEnvelopeXmlnstemAttr
        {
            get
            {
                return $"{Xmlns}:{Tem}='{SoapEnvelopeXmlnstemAttrValue}'";
            }
        }
        public virtual string SoapEnvelopeXmlnstemAttrValue { get; set; } = "http://tempuri.org/";

        protected virtual void ThrowIfNullOrEmpty(string value, string paramName = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(paramName);
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
