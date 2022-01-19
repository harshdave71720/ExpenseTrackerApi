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
