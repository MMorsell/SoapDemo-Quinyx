using System;

namespace SoapDemo_Quinyx
{
    class Program
    {
        static void Main(string[] args)
        {
            var soapApi = new SoapApi();

            var listOfDrivers = soapApi.Request();

            foreach (var item in listOfDrivers)
            {
                Console.WriteLine($"{item.BadgeId} - {item.CategoryName}");
                Console.WriteLine($"{item.StartTime} - {item.EndTime}");
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
