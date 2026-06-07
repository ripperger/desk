using Desk.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestDesk.UnitTests.Validation
{
    public class TaxNumberAttributeTest
    {
        [Theory]
        [InlineData("8412721004")]
        public void MathCheck_ShouldReturnTrue(string value)
        {
            // Arrange

            // Act
            bool result = TaxNumberAttribute.MathCheck(value);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("8412721004", "1980.01.01.")]
        [InlineData("8493990232", "2002.04.02.")]
        public void GetBirthDate_ShouldReturnCorrectDate(string value, string expected)
        {
            // Act
            DateTime birthDate = TaxNumberAttribute.GetBirthDate(value);

            // Assert
            Assert.Equal(expected, birthDate.ToString("yyyy.MM.dd."));
        }

    }
}
