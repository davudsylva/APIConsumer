using Consumer.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Service.Services
{
    public class OwnersReport : IReporting
    {
        private readonly IDataClient _dataClient;
        private readonly IReportOutput _reportOutput;

        public OwnersReport(IDataClient dataClient, IReportOutput reportOutput)
        {
            _reportOutput = reportOutput;
            _dataClient = dataClient;
        }

        public async Task CreateReport()
        {
            var owners = await _dataClient.GetOwners();
            if (owners != null)
            {
                var formatter = new StringBuilder();

                await _reportOutput.Print(formatter.ToString());
            }
        } 
    }
}
