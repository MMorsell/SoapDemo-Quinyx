using System;

namespace SoapDemo_Quinyx
{
    class Program
    {
        static void Main(string[] args)
        {
            var soapApi = new SoapApi();

            soapApi.Request();
        }
    }
}
