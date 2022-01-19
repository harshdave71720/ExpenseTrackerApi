using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Helpers.Templates
{
    public class Template<T> : IDisposable
    {
        private readonly Stream _template;

        public Template(Stream template)
        {
            if(template == null)
                 throw new ArgumentNullException(nameof(template));
            _template = template;
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
            List<MemberInfo> fieldsOrProperties = new List<MemberInfo>();
            var missingMembers = new List<string>();

            Type type = typeof(T);
            fieldsOrProperties.AddRange(type.GetProperties());
            fieldsOrProperties.AddRange(type.GetFields());
            foreach (var member in fieldsOrProperties)
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

        private async Task<List<string>> GetColumnNames()
        {
            TextReader reader = new StreamReader(_template);
            try
            {
                string columnString = (await reader.ReadLineAsync()).Trim();
                if (string.IsNullOrEmpty(columnString))
                    return new List<string>();

                return columnString.Split(',').Select(s => s.Trim()).ToList();
            }
            catch (IOException)
            {
                throw new Exception("Error while reading the file");
            }
            finally
            {
                reader?.Close();
            }
        }

        public void Dispose()
        {
            _template?.Close();
            _template?.Dispose();
        }
    }
}
