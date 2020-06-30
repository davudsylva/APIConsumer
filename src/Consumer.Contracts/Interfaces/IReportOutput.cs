using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.Contracts.Interfaces
{
    public interface IReportOutput
    {
        Task Print(string textOutput);
    }
}
