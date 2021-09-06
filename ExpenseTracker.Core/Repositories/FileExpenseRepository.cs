using System;
using ExpenseTracker.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace ExpenseTracker.Core.Repositories
{
    public class FileExpenseRepository : IExpenseRepository, IDisposable
    {
        private string _filePath;
        
        public FileExpenseRepository(string filePath)
        {
            _filePath = filePath;
            if(!File.Exists(filePath))
            {
                using(StreamWriter writer = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.WriteLine(Expense.HeadersToCsv());
                    writer.Close();
                }
            }
        }

        public async Task<Expense> Add(Expense expense)
        {
            int id = await GetNewId();
            expense = new Expense(id, expense);
            using(StreamWriter writer = new StreamWriter(new FileStream(_filePath, FileMode.Append, FileAccess.Write)))
            {
                await writer.WriteLineAsync(expense.ToCsv());

                writer.Close();
            }

            return expense;
        }

        public async Task<Expense> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Expense>> Expenses(Predicate<Expense> filter)
        {
            List<Expense> expenses = new List<Expense>();
            
            using(StreamReader reader = new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read)))
            {
                await reader.ReadLineAsync();

                while(!reader.EndOfStream)
                {
                    var expenseString = await reader.ReadLineAsync();
                    expenses.Add(Expense.FromCsv(expenseString));
                }
            }

            return expenses;
        }

        public async Task<Expense> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Expense> Update(Expense expense)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetNewId()
        {
            int id = 1;
            using(StreamReader reader = new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read)))
            {
                await reader.ReadLineAsync();
                
                while(!reader.EndOfStream)
                {
                    id++;
                    await reader.ReadLineAsync();
                }

                reader.Close();
            }

            return id;
        }

        public void Dispose()
        {
            // _file?.Close();
            // _file?.Dispose();
        }

        public Task SaveChangesAsync()
        {
            return null;
        }
    }
}