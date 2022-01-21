using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Helpers.Templates
{
    public class Template<T> : IDisposable
    {
        private readonly Stream _templateStream;
        private readonly TemplateSource<T> _templateSource;
        private string[] _columnNames;

        public Template(Stream templateStream)
        {
            if (templateStream == null)
                throw new ArgumentNullException(nameof(templateStream));
            _templateStream = templateStream;
            _templateSource = new TemplateSource<T>();
        }

        public async Task<IEnumerable<TemplateValidationError>> Validate()
        {
            List<TemplateValidationError> errors = new List<TemplateValidationError>();

            errors.AddRange(await this.ValidateRequiredFields());

            return errors;
        }

        private async Task<IEnumerable<TemplateValidationError>> ValidateRequiredFields()
        {
            var templateColumns = await this.GetColumnNames();
            List<MemberInfo> publicDataMembers = _templateSource.GetDataMembers();
            var missingMembers = new List<string>();

            foreach (var member in publicDataMembers)
            {
                if (member.CustomAttributes
                          .Any(a => a.AttributeType == typeof(RequiredAttribute))
                    && !templateColumns.Contains(member.Name, StringComparer.OrdinalIgnoreCase))
                {
                    missingMembers.Add(member.Name);
                }
            }

            return missingMembers.Select(f => new TemplateValidationError { Message = $"Template is missing column for {f}" });
        }

        private async Task<string[]> GetColumnNames()
        {
            if (_columnNames != null)
                return _columnNames;

            _templateStream.Seek(0, SeekOrigin.Begin);
            try
            {
                using (TextReader reader = new StreamReader(_templateStream, Encoding.UTF8, false, 1024, leaveOpen: true))
                {
                    string columnString = (await reader.ReadLineAsync()).Trim();
                    if (string.IsNullOrEmpty(columnString))
                        _columnNames = Array.Empty<string>();
                    else
                        _columnNames = columnString.Split(',').Select(s => s.Trim()).ToArray();

                    reader.Close();
                    
                    return _columnNames;
                }
            }
            catch (IOException)
            {
                throw new Exception("Error while reading the file");
            }
        }

        public async Task<IEnumerable<T>> GetRecords()
        {
            List<T> records = new List<T>();
            var columns = await GetColumnNames();
            try
            {
                _templateStream.Seek(0, SeekOrigin.Begin);
                using (TextReader reader = new StreamReader(_templateStream, Encoding.UTF8, false, 1024, leaveOpen: true))
                {
                    string nextRecord = await reader.ReadLineAsync();
                    while ((nextRecord = await reader.ReadLineAsync()) != null)
                    {
                        string[] values = nextRecord.Split(',');
                        records.Add(_templateSource.CreateSourceInstance(columns, values));
                    }

                   reader.Close();
                }
            } catch (Exception ex)
            {
                throw new ArgumentException("Error reading data from file.");
            }
            return records;
        }

        public void Dispose()
        {
            _templateStream?.Close();
            _templateStream?.Dispose();
        }
    }
}
