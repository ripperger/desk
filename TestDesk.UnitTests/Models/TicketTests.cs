using Desk.Models;
using Desk.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xunit;

namespace TestDesk.UnitTests.Models
{
    public class TicketTests
    {
        /// <summary>
        /// Testing the ReferenceId method
        /// </summary>
        [Fact]
        public void ReferenceId_ShouldReturnCorrectFormat()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 123,
                Categorization = "TestCategoryA.TestSpecPropsAA",
                Timestamp = new DateTime(2023, 10, 10)
            };

            // Act
            var referenceId = ticket.ReferenceId();

            // Assert
            Assert.Equal("T-231010-123", referenceId);
        }

        /// <summary>
        /// Testing the GetSummary method
        /// </summary>
        [Fact]
        public void GetSummary_ShouldReturnCorrectSummary()
        {
            // Arrange
            var ticket = new Ticket
            {
                Categorization = "TestCategoryA.TestSpecPropsAA",
                Description = "Test Description"
            };

            // Act
            var summary = ticket.GetSummary();

            // Assert
            Assert.Equal("Teszt Kategória 'A' > Teszt Alkategória 'AA' - Test Description", summary);
        }

        /// <summary>
        /// Testing the IsOverdue method with a late timestamp
        /// </summary>
        /// <param name="days">Number of days in the past, e.g. -1 = yesterday, -2 = two day</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        public void IsOverdue_ShouldReturnTrueIfOverdue(int days)
        {
            // Arrange
            var ticket = new Ticket
            {
                Timestamp = DateTime.Now.AddDays(days)
            };

            // Act
            var isOverdue = ticket.IsOverdue();

            // Assert
            Assert.True(isOverdue);
        }

        /// <summary>
        /// Testing the IsImportant method with a high and VIP priority
        /// </summary>
        [Theory]
        [InlineData("HIGH")]
        [InlineData("VIP")]
        public void IsImportant_ShouldReturnTrueIfPriorityIsHighOrVIP(string priority)
        {
            // Arrange
            var ticket = new Ticket
            {
                Priority = priority
            };

            // Act
            var isImportant = ticket.IsImportant();

            // Assert
            Assert.True(isImportant);
        }

        /// <summary>
        /// Testing the IsClosed method
        /// </summary>
        /// <param name="status">As in the Dictionaries.TicketStatus dictionary</param>
        [Theory]
        [InlineData("RESOLVED")]
        [InlineData("REJECTED")]
        [InlineData("CLOSED_BY_USER")]
        public void IsClosed_ShouldReturnTrueIfStatusIsClosed(string status)
        {
            // Arrange
            var ticket = new Ticket
            {
                Status = status
            };

            // Act
            var isClosed = ticket.IsClosed();

            // Assert
            Assert.True(isClosed);
        }

        /// <summary>
        /// Testing the IsNew method with a NEW or REOPENED status.
        /// </summary>
        [Theory]
        [InlineData("NEW")]
        [InlineData("REOPENED")]
        public void IsNew_ShouldReturnTrueIfStatusIsNewOrReopened(string status)
        {
            // Arrange
            var ticket = new Ticket
            {
                Status = status
            };

            // Act
            var isNew = ticket.IsNew();

            // Assert
            Assert.True(isNew);
        }

        /// <summary>
        /// Testing the GetPriorityStyle method.
        /// </summary>
        [Fact]
        public void GetPriorityStyle_ShouldReturnCorrectStyle()
        {
            // Arrange
            var ticket = new Ticket
            {
                Priority = "HIGH"
            };

            // Act
            var style = ticket.GetPriorityStyle();

            // Assert
            Assert.Equal("fw-medium text-danger ", style);

            // Arrange
            ticket.Priority = "LOW";

            // Act
            style = ticket.GetPriorityStyle();

            // Assert
            Assert.Equal("", style);
        }

        /// <summary>
        /// Testing the GetStatusStyle method.
        /// </summary>
        [Fact]
        public void GetStatusStyle_ShouldReturnCorrectStyle()
        {
            // Arrange
            var ticket = new Ticket
            {
                Status = "NEW"
            };

            // Act
            var style = ticket.GetStatusStyle();

            // Assert
            Assert.Equal("fw-medium ", style);

            // Arrange
            ticket.Status = "RESOLVED";

            // Act
            style = ticket.GetStatusStyle();

            // Assert
            Assert.Equal("", style);
        }

        /// <summary>
        /// Testing the GetListItemStyle method.
        /// </summary>
        [Fact]
        public void GetListItemStyle_ShouldReturnCorrectStyle()
        {
            // Arrange
            var ticket = new Ticket
            {
                Status = "NEW",
                Timestamp = DateTime.Now.AddDays(-2)
            };

            // Act
            var style = ticket.GetListItemStyle();

            // Assert
            Assert.Equal("fw-medium bg-danger bg-opacity-10 ", style);

            // Arrange
            ticket.Status = "RESOLVED";

            // Act
            style = ticket.GetListItemStyle();

            // Assert
            Assert.Equal("opacity-50 ", style);
        }
    }
}
