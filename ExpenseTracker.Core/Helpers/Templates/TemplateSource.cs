using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ExpenseTracker.Core.Helpers.Templates
{
    public class TemplateSource<T>
    {
        private readonly Type _sourceType;
        private List<MemberInfo> _dataMembers;

        public TemplateSource()
        { 
            _sourceType = typeof(T);
        }

        public Dictionary<MemberInfo, int> GetColumnOrdinals(string[] columnNames)
        {
            var dataMembers = this.GetDataMembers();
            var templateColumns = columnNames;
            var dataMemberOrdinals = new Dictionary<MemberInfo, int>();

            foreach (var dataMember in dataMembers)
            {
                int ordinal = -1;
                for (int i = 0; i < templateColumns.Count(); i++)
                {
                    if (dataMember.Name.Equals(templateColumns[i], StringComparison.OrdinalIgnoreCase))
                    {
                        ordinal = i;
                        break;
                    }
                }

                dataMemberOrdinals.Add(dataMember, ordinal);
            }

            return dataMemberOrdinals;
        }

        public List<MemberInfo> GetDataMembers()
        {
            if (_dataMembers == null)
            {
                _dataMembers = new List<MemberInfo>(_sourceType.GetProperties());
                _dataMembers.AddRange(_sourceType.GetFields());
            }

            return _dataMembers;
        }

        public T CreateSourceInstance(string[] columnNames, string[] values)
        {
            try
            {
                Dictionary<MemberInfo, int> columnOrdinals = GetColumnOrdinals(columnNames);
                T instance = (T)Activator.CreateInstance(typeof(T));
                foreach (var pair in columnOrdinals)
                {
                    if (pair.Value == -1)
                        continue;

                    if (pair.Key.MemberType == MemberTypes.Property)
                    {
                        ((PropertyInfo)pair.Key).SetValue(instance, Convert.ChangeType(values[pair.Value], ((PropertyInfo)pair.Key).PropertyType));
                    }
                    if (pair.Key.MemberType == MemberTypes.Field)
                    {
                        ((FieldInfo)pair.Key).SetValue(instance, Convert.ChangeType(values[pair.Value], ((FieldInfo)pair.Key).FieldType));
                    }
                }

                return instance;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentException("Value missing for one of the columns");
            }
        }
    }
}
