using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Net;
using SoapDemo_Quinyx.Models;

namespace SoapDemo_Quinyx
{
    public class SoapApi
    {
        public void Request()
        {
            var _url = "https://api.quinyx.com/FlexForceWebServices.php";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest(_url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            var drivers = new List<Driver>();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();




                    Driver[] drivers1 = null;

                    XmlSerializer serializer = new XmlSerializer(typeof(Driver[]));
                    rd.ReadToEnd();
                    //drivers1 = (Driver[])serializer.Deserialize(rd);
                    Console.WriteLine();
                    Console.WriteLine(soapResult);
                }



                var badgeNoRegex = new Regex("([<][b][a][d][g][e][N][o][xsi:type=xsd:\"string\\s]+[>])(\\d +)([<][\\/][badgeNo] +[>])");

                Console.Write(soapResult);
            }
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uri=""uri:FlexForce""><soapenv:Header/><soapenv:Body><uri:wsdlGetSchedulesV2 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><apiKey>b5a9-8730-3e14-d72e</apiKey><getSchedulesV2Request xsi:type=""flex:getSchedulesV2Request"" xmlns:flex=""http://qwfm/soap/FlexForce""><fromDate xsi:type=""xsd:string"">2019-08-25</fromDate><fromTime xsi:type=""xsd:string"">00:00:00</fromTime><toDate xsi:type=""xsd:string"">2019-08-26</toDate><toTime xsi:type=""xsd:string"">00:00:00</toTime><scheduledShifts xsi:type=""xsd:boolean"">true</scheduledShifts><absenceShifts xsi:type=""xsd:boolean"">false</absenceShifts><allUnits xsi:type=""xsd:boolean"">false</allUnits><includeCosts xsi:type=""xsd:boolean"">false</includeCosts></getSchedulesV2Request></uri:wsdlGetSchedulesV2></soapenv:Body></soapenv:Envelope>");
            return soapEnvelopeDocument;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}
