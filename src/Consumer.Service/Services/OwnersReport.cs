using Consumer.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            if (owners != null && owners.Count() > 0)
            {
                var formatter = new StringBuilder();

                var flattened = owners
                    .Where(x => x.Pets != null && x.Pets.Count() > 0)
                    .SelectMany(o => o.Pets?.Select(p => new { Gender=o.Gender, PetName=p?.Name}));

                var grouped = flattened
                    .GroupBy(f => f.Gender,
                             (key, g) => new { Gender = key, Names = g.Select(x => x.PetName).OrderBy(x => x).ToList() }); 

                foreach (var set in grouped.ToList())
                {
                    formatter.Append($"\n{set.Gender}\n\n");
                    foreach (var name in set.Names)
                    {
                        formatter.Append($"   * {name}\n" );
                    }
                }

                await _reportOutput.Print(formatter.ToString());
            }
        } 
    }
}
