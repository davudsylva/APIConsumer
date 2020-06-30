using System;
using System.Threading.Tasks;
using Consumer.Contracts.Interfaces;

namespace Consumer.Service.Services
{
    public class ConsoleOutput : IReportOutput
    {
        public async Task Print(string textOutput)
        {
            Console.Write(textOutput);
        }
    }
}
