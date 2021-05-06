using System;
using System.Xml;
using System.Net;
using System.IO;
using SOAPTools.Core;

namespace TestingSOAPTools
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(AddUsingWebService(1, 1));

            // Built envelope by SOAPRequestBuilder:
            /*
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>
            	<soapenv:Header/>
            	<soapenv:Body>
            		<tem:Add>
            			<tem:intA>1</tem:intA>
            			<tem:intB>1</tem:intB>
            		</tem:Add>
            	</soapenv:Body>
            </soapenv:Envelope>
            */

            // SOAP service response:
            /*
            <?xml version="1.0" encoding="utf-8"?>
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            	<soap:Body>
            		<AddResponse xmlns="http://tempuri.org/">
            			<AddResult>2</AddResult>
            		</AddResponse>
            	</soap:Body>
            </soap:Envelope>
             */

            Console.ReadKey();
        }

        public static string AddUsingWebService(int A, int B)
        {
            var url = "http://www.dneonline.com/calculator.asmx";
            var action = "Add";

            var _xmlDocSOAPEnvelope = CreateSoapEnvelope(A, B, action);

            var request = CreateWebRequest(url, action, _xmlDocSOAPEnvelope);

            using (WebResponse Serviceres = request.GetResponse())
            {
                using (var _streamReader = new StreamReader(Serviceres.GetResponseStream()))
                {
                    var result = _streamReader.ReadToEnd();

                    return result;
                }
            }
        }

        private static HttpWebRequest CreateWebRequest(string url, string action, XmlDocument _xmlDocSOAPEnvelope)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            webRequest.InsertSOAPEnvelope(_xmlDocSOAPEnvelope);

            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(int A, int B, string methodName)
        {
            var _xmlDocSOAPEnvelope = new XmlDocument();
            var xmlEnvelope = SOAPRequestBuilder.STBuildEnvelope(
                new
                {
                    intA = A,
                    intB = B
                }, methodName);

            // Built envelope:
            /*
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>
            	<soapenv:Header/>
            	<soapenv:Body>
            		<tem:Add>
            			<tem:intA>1</tem:intA>
            			<tem:intB>1</tem:intB>
            		</tem:Add>
            	</soapenv:Body>
            </soapenv:Envelope>
            */

            _xmlDocSOAPEnvelope.LoadXml(xmlEnvelope);
            return _xmlDocSOAPEnvelope;
        }
    }
}
