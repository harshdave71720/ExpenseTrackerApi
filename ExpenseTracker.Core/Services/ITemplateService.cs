using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Services
{
    public interface ITemplateService
    {
        Task<IEnumerable<T>> GetRecordsFromTemplate<T>(Stream stream);
    }
}
