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
        public List<Driver> Request()
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
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                return DeSerializeData(soapResult);
            }
        }



        public List<Driver> DeSerializeData(string resultFromSoapRequest)
        {
            //Badge Matching
            var badgeNoRegex = new Regex("([<][b][a][d][g][e][N][o] [xsi:type=xsd:\"string\\s]+[>])(\\d+)([<][\\/][badgeNo]+[>])");

            var badgeCollection = badgeNoRegex.Matches(resultFromSoapRequest);
            List<int> badgeList = new List<int>();
            List<string> beginList = new List<string>();
            List<string> endList = new List<string>();
            List<string> categoryNameList = new List<string>();

            foreach (var row in badgeCollection)
            {
                var match = badgeNoRegex.Match(row.ToString());
                if (match.Success)
                {
                    var valueString = match.Groups[2].Value;
                    int.TryParse(valueString, out int result);
                    badgeList.Add(result);
                }
            }

            var begTimeRegex = new Regex("([<]begTime xsi:type=\"xsd:time\"[>])([\\d:]+)([<][\\/]begTime[>])");

            var begTimeCollection = begTimeRegex.Matches(resultFromSoapRequest);

            foreach (var row in begTimeCollection)
            {
                var match = begTimeRegex.Match(row.ToString());
                if (match.Success)
                {
                    beginList.Add(match.Groups[2].Value);
                }
            }


            var endTimeRegex = new Regex("([<]endTime xsi:type=\"xsd:time\"[>])([\\d:]+)([<][\\/]endTime[>])");

            var endTimeCollection = endTimeRegex.Matches(resultFromSoapRequest);

            foreach (var row in endTimeCollection)
            {
                var match = endTimeRegex.Match(row.ToString());
                if (match.Success)
                {
                    endList.Add(match.Groups[2].Value);
                }
            }

            var categoryNameRegex = new Regex("([<]categoryName xsi:type=\"xsd:string\"[>])([O]?[F]?[O]?[A]?[M]?[ ]?[.]?[E]?[f]?[t]?[e]?[r]?[m]?[i]?[d]?[d]?[a]?[g]?[T]?[r]?[a]?[n]?[s]?[p]?[o]?[r]?[t]?[l]?[e]?[d]?[a]?[r]?[e]?)([<][\\/]categoryName[>])");

            var categoryNameCollection = categoryNameRegex.Matches(resultFromSoapRequest);

            foreach (var row in categoryNameCollection)
            {
                var match = categoryNameRegex.Match(row.ToString());
                if (match.Success)
                {
                    categoryNameList.Add(match.Groups[2].Value);
                }
            }

            var returnList = new List<Driver>();
            for (int i = 0; i < badgeList.Count; i++)
            {
                TimeSpan.TryParse(beginList[i], out TimeSpan outBegTime);
                TimeSpan.TryParse(endList[i], out TimeSpan outEndTime);
                var driver = new Driver()
                {
                    BadgeId = badgeList[i],
                    StartTime = outBegTime,
                    EndTime = outEndTime,
                    CategoryName = categoryNameList[i]
                };
                
                returnList.Add(driver);
            }
            return returnList;
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
            string fileName = "apikey.txt";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullName = System.IO.Path.Combine(desktopPath, fileName);
            string key;
            
            
            using (StreamReader steamReader = new StreamReader(fullName))
                {
                    key = steamReader.ReadToEnd();
                }


            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(@$"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:uri=""uri:FlexForce""><soapenv:Header/><soapenv:Body><uri:wsdlGetSchedulesV2 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><apiKey>{key}</apiKey><getSchedulesV2Request xsi:type=""flex:getSchedulesV2Request"" xmlns:flex=""http://qwfm/soap/FlexForce""><fromDate xsi:type=""xsd:string"">2019-08-25</fromDate><fromTime xsi:type=""xsd:string"">00:00:00</fromTime><toDate xsi:type=""xsd:string"">2019-08-26</toDate><toTime xsi:type=""xsd:string"">00:00:00</toTime><scheduledShifts xsi:type=""xsd:boolean"">true</scheduledShifts><absenceShifts xsi:type=""xsd:boolean"">false</absenceShifts><allUnits xsi:type=""xsd:boolean"">false</allUnits><includeCosts xsi:type=""xsd:boolean"">false</includeCosts></getSchedulesV2Request></uri:wsdlGetSchedulesV2></soapenv:Body></soapenv:Envelope>");
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
