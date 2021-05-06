using System;
using System.Xml;
using System.Net;
using System.IO;
using SOAPTools.Core;

namespace TestingSOAPTools
{
    class Program
    {
        static string url;
        static string action;
        static void Main()
        {
            url = "http://www.dneonline.com/calculator.asmx";
            action = "Add";

            Console.WriteLine("Executing request...");
            Console.WriteLine();

            var Response = AddUsingWebService(1, 1);

            Console.WriteLine($"{nameof(Response)}:");
            Console.WriteLine(Response);

            // url: http://www.dneonline.com/calculator.asmx
            // action: Add
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
            var _xmlDocSOAPEnvelope = new XmlDocument();
            _xmlDocSOAPEnvelope.BuildNLoadSOAPEnvelope(
                new
                {
                    intA = A,
                    intB = B
                }, 
                action);

            var request = _xmlDocSOAPEnvelope.CreateWebRequest(url);

            using WebResponse response = request.GetResponse();
            return response.GetResponse();
        }

        public static string AddUsingWebService_2(int A, int B)
        {
            var _xmlDocSOAPEnvelope = CreateSOAPEnvelope(A, B, action);

            var request = _xmlDocSOAPEnvelope.CreateWebRequest(url);

            using WebResponse Serviceres = request.GetResponse();
            using var _streamReader = new StreamReader(Serviceres.GetResponseStream());

            var result = _streamReader.ReadToEnd();

            return result;
        }

        private static XmlDocument CreateSOAPEnvelope(int A, int B, string methodName)
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
