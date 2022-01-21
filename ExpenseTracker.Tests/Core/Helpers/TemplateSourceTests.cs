using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ExpenseTracker.Core.Helpers.Templates;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ExpenseTracker.Tests.Core.Helpers.Templates
{
    [TestFixture]
    internal class TemplateSourceTests
    {
        [Test]
        public void GetDataMembers_Should_ReturnAllAndOnlyPublicDataMembers()
        {
            // Arrange
            var sut = new TemplateSource<Sample>();

            // Act
            var dataMembers = sut.GetDataMembers();

            // Assert
            Assert.That(dataMembers.Count(), Is.EqualTo(3));
            Assert.That(dataMembers.Any(m => m.Name == nameof(Sample.PublicProp1)));
            Assert.That(dataMembers.Any(m => m.Name == nameof(Sample.PublicProp2)));
            Assert.That(dataMembers.Any(m => m.Name == nameof(Sample.PublicField)));
        }

        [Test]
        public void GetColumnOrdinals_ReturnsCorrectColumnsOrdinalsForMembers()
        {
            // Arrange
            var sut = new TemplateSource<Sample>();
            string[] columns = { nameof(Sample.PublicProp1), nameof(Sample.PublicField) };
            var dataMembers = sut.GetDataMembers();

            // Act
            var memberOrdinals = sut.GetColumnOrdinals(columns);

            // Assert
            for (int i = 0; i < columns.Count(); i++)
            {
                Assert.That(memberOrdinals[dataMembers.Single(m => m.Name == columns[i])], Is.EqualTo(i));
            }
            foreach (var dataMember in dataMembers.Where(m => !columns.Contains(m.Name)))
            {
                Assert.That(memberOrdinals[dataMember], Is.EqualTo(-1));
            }
        }

        [Test]
        public void CreateSourceInstance_ShouldReturnInstanceWithGivenValues()
        {
            // Arrange
            var columns = new string[] { nameof(Sample.PublicProp1), nameof(Sample.PublicProp2), nameof(Sample.PublicField) };
            var values = new string[] { "Harsh", "1", DateTime.Now.Date.ToString() };
            var sut = new TemplateSource<Sample>();

            // Act
            Sample sample = sut.CreateSourceInstance(columns, values);

            // Arrange
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample.PublicProp1, Is.EqualTo(values[0]));
            Assert.That(sample.PublicProp2, Is.EqualTo(Convert.ToInt32(values[1])));
            Assert.That(sample.PublicField, Is.EqualTo(Convert.ToDateTime(values[2])));
        }

        private class Sample
        {
            public string PublicProp1 { get; set; }

            public int PublicProp2 { get; set; }

            public DateTime PublicField;

            private double PrivateProp1 { get; set; }

            private double PrivateProp2 { get; set; }

            private object PrivateField;
        }
    }
}
