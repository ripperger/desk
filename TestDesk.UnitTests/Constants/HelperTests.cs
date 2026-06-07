using Desk.Constants;
using Desk.Models.TestCategoryA;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestDesk.UnitTests.Constants
{
    public class HelperTests
    {
        [Fact]
        public void GetSelectList_ShouldReturnSelectList()
        {
            // Arrange
            var type = typeof(Dictionaries);

            // Act
            var selectList = Helper.GetSelectList(type);

            // Assert
            Assert.NotNull(selectList);
            Assert.IsType<SelectList>(selectList);
            Assert.Equal("Category", selectList.First().Value);
        }

        [Fact]
        public void GetCategory_ShouldReturnCategory()
        {
            // Arrange
            var type = typeof(TestSpecPropsAA);

            // Act
            var category = Helper.GetCategory(type);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("TestCategoryA.TestSpecPropsAA", category);
        }

        [Fact]
        public void FormatCategory_ShouldReturnFormattedCategory()
        {
            // Arrange
            var category = "TestCategoryA.TestSpecPropsAA";

            // Act
            var formattedCategory = Helper.FormatCategory(category);

            // Assert
            Assert.NotNull(formattedCategory);
            Assert.Equal("Teszt Kategória 'A' > Teszt Alkategória 'AA'", formattedCategory);
        }

        [Fact]
        public void FormatCategoryShort_ShouldReturnShortFormattedCategory()
        {
            // Arrange
            var category = "TestCategoryA.TestSpecPropsAA";

            // Act
            var formattedCategoryShort = Helper.FormatCategoryShort(category);

            // Assert
            Assert.NotNull(formattedCategoryShort);
            Assert.Equal("Teszt Alkategória 'AA'", formattedCategoryShort);
        }

        [Fact]
        public void GetController_ShouldReturnController()
        {
            // Arrange
            var category = "TestCategoryA.TestSpecPropsAA";

            // Act
            var controller = Helper.GetController(category);

            // Assert
            Assert.NotNull(controller);
            Assert.Equal("TestSpecPropsAA", controller);
        }

        [Fact]
        public void IsDevUser_ShouldReturnTrueForDevUser()
        {
            // Arrange
            var userName = Dictionaries.Role["DEV_USER"];

            // Act
            var isDevUser = Helper.IsDevUser(userName);

            // Assert
            Assert.True(isDevUser);
        }

        [Fact]
        public void IsDevUser_ShouldReturnFalseForNonDevUser()
        {
            // Arrange
            var userName = "non_dev_user";

            // Act
            var isDevUser = Helper.IsDevUser(userName);

            // Assert
            Assert.False(isDevUser);
        }
    }
}
