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
            var columns = new string[]{ nameof(Sample.SampleProp) };
            var template = new Template<Sample>(new MemoryStream(Encoding.ASCII.GetBytes(nameof(Sample.SampleProp))));

            // Act
            IEnumerable<TemplateValidationError> errors = await template.Validate();

            // Assert
            Assert.That(errors.Any(e => e.Message.Contains($"Template is missing column for {nameof(Sample.IntegerProp)}")));
            Assert.That(errors.Any(e => e.Message.Contains($"Template is missing column for {nameof(Sample.DateField)}")));
        }

        [Test]
        public async Task Validate_Should_NotCheckForMissingPrivateColumns()
        {
            // Arrange
            var columns = new string[] { nameof(Sample.SampleProp) };
            var template = new Template<Sample>(new MemoryStream(Encoding.ASCII.GetBytes(nameof(Sample.SampleProp))));

            // Act
            var errors = await template.Validate();

            // Assert
            Assert.That(!errors.Any(e => e.Message.Contains("PrivateProp")));
        }

        [Test]
        public async Task GetRecords_Should_ReturnCollectionOfSourceFromData()
        {
            // Arrange
            string[][] rows = new string[][]
            { 
                new string[] { nameof(Sample.SampleProp), nameof(Sample.IntegerProp), nameof(Sample.DateField) },
                new string[] { "Sample1", "-1", DateTime.Now.AddDays(-1).ToString() },
                new string[] { "Sample2", "400", DateTime.Now.AddDays(300).ToString() }
            };
            string content = "";
            foreach (var row in rows)
            {
                content = content + (string.Join(',', row)) + "\n";
            }
            Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
            var sut = new Template<Sample>(stream);

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
