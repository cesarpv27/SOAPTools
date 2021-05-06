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
            if (soapEnvelope == null)
                throw new ArgumentNullException(nameof(soapEnvelope));

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelope.Save(stream);
            }
        }
    }
}
