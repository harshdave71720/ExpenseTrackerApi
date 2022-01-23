using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Core.Helpers.Templates;
using System.IO;
using System.Linq;

namespace ExpenseTracker.Tests.Core.Helpers.Templates
{
    [TestFixture]
    internal class TemplateTests
    {
        [Test]
        public async Task Validate_Should_ReturnMissingColumns()
        {
            // Arrange
            var columns = new string[] { nameof(Sample.SampleProp) };
            var sut = CreateTemplate<Sample>(new string[][] { columns });

            // Act
            IEnumerable<TemplateValidationError> errors = await sut.Validate();

            // Assert
            Assert.That(errors.Any(e => e.Message.Contains($"Template is missing column for {nameof(Sample.IntegerProp)}")));
            Assert.That(errors.Any(e => e.Message.Contains($"Template is missing column for {nameof(Sample.DateField)}")));

            sut.Dispose();
        }

        [Test]
        public async Task Validate_Should_NotCheckForMissingPrivateColumns()
        {
            // Arrange
            var columns = new string[] { nameof(Sample.SampleProp) };
            var sut = CreateTemplate<Sample>(new string[][] { columns });

            // Act
            var errors = await sut.Validate();

            // Assert
            Assert.That(!errors.Any(e => e.Message.Contains("PrivateProp")));

            sut.Dispose();
        }

        [Test]
        public async Task GetRecords_Should_ReturnCollectionOfSourceFromData()
        {
            // Arrange
            string[][] rows = new string[][]
            {
                new string[] { nameof(Sample.SampleProp), nameof(Sample.IntegerProp), nameof(Sample.DateField) },
                new string[] { "SampleValue1", "-1", DateTime.Now.AddDays(-1).ToString() },
                new string[] { "SampleValue2", "400", DateTime.Now.AddDays(300).ToString() }
            };
            
            var sut = CreateTemplate<Sample>(rows);

            // Act
            IEnumerable<Sample> samples = await sut.GetRecords();

            // Assert
            Assert.That(samples.Count, Is.EqualTo(rows.Length - 1));
            for (int i = 1; i < rows.Length; i++)
            {
                var sample = samples.Single(s => s.SampleProp == rows[i][0]
                                                && s.IntegerProp == Convert.ToInt32(rows[i][1])
                                                && s.DateField == Convert.ToDateTime(rows[i][2])
                );

                Assert.That(sample, Is.Not.Null);
            }

            sut.Dispose();
        }


        private Template<T> CreateTemplate<T>(string[][] rows)
        {
            StringBuilder contentBuilder = new StringBuilder(string.Empty);
            foreach (var row in rows)
                contentBuilder.AppendLine(string.Join(',', row));

            Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(contentBuilder.ToString()));
            return new Template<T>(stream);
        }

        private class Sample
        {
            [Required]
            public string SampleProp { get; set; }

            [Required]
            public int IntegerProp { get; set; }

            [Required]
            public DateTime DateField;

            public string StringProp { get; set; }

            public Boolean BooleanField;

            [Required]
            private string PrivateProp { get; set; }
        }
    }
}
