# SOAPTools example

            url: http://www.dneonline.com/calculator.asmx
            action: Add
            
            C#:
                var xmlEnvelope = SOAPRequestBuilder.STBuildEnvelope(
                new
                {
                    intA = 1,
                    intB = 1
                }, methodName);
                
            Built envelope by SOAPRequestBuilder:
            
            <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>
            	<soapenv:Header/>
            	<soapenv:Body>
            		<tem:Add>
            			<tem:intA>1</tem:intA>
            			<tem:intB>1</tem:intB>
            		</tem:Add>
            	</soapenv:Body>
            </soapenv:Envelope>
            

            SOAP service response:
            
            <?xml version="1.0" encoding="utf-8"?>
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            	<soap:Body>
            		<AddResponse xmlns="http://tempuri.org/">
            			<AddResult>2</AddResult>
            		</AddResponse>
            	</soap:Body>
            </soap:Envelope>
             
             
            Also can:             
            C#:            
                int intA = 1;
                int intB = 1;
                var xmlEnvelope = SOAPRequestBuilder.STBuildEnvelope(
                new
                {
                    intA,
                    intB
                }, methodName);
                