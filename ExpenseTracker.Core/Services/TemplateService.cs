using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Core.Helpers.Templates;

namespace ExpenseTracker.Core.Services
{
    public class TemplateService : ITemplateService
    {
        public async Task<IEnumerable<T>> GetRecordsFromTemplate<T>(Stream stream) 
        {
            Template<T> template = new Template<T>(stream);
            return await template.GetRecords();
        }
    }
}
