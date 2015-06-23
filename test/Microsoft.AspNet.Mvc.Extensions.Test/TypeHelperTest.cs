// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.Internal;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class TypeHelperTests
    {
        public static TheoryData<object, KeyValuePair<string, object>> IgnoreCaseTestData
        {
            get
            {
                return new TheoryData<object, KeyValuePair<string, object>>
                {
                    {
                        new
                        {
                            selected = true,
                            SeLeCtEd = false
                        },
                        new KeyValuePair<string, object>("selected", false)
                    },
                    {
                        new
                        {
                            SeLeCtEd = false,
                            selected = true
                        },
                        new KeyValuePair<string, object>("SeLeCtEd", true)
                    },
                    {
                        new
                        {
                            SelECTeD = false,
                            SeLECTED = true
                        },
                        new KeyValuePair<string, object>("SelECTeD", true)
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(IgnoreCaseTestData))]
        public void ObjectToDictionary_IgnoresPropertyCase(object testObject,
                                                           KeyValuePair<string, object> expectedEntry)
        {
            // Act
            var result = TypeHelper.ObjectToDictionary(testObject);

            // Assert
            var entry = Assert.Single(result);
            Assert.Equal(expectedEntry, entry);
        }

        [Fact]
        public void ObjectToDictionary_WithNullObject_ReturnsEmptyDictionary()
        {
            // Arrange
            object value = null;

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(0, dictValues.Count);
        }

        [Fact]
        public void ObjectToDictionary_WithPlainObjectType_ReturnsEmptyDictionary()
        {
            // Arrange
            var value = new object();

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(0, dictValues.Count);
        }

        [Fact]
        public void ObjectToDictionary_WithPrimitiveType_LooksUpPublicProperties()
        {
            // Arrange
            var value = "test";

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(1, dictValues.Count);
            Assert.Equal(4, dictValues["Length"]);
        }

        [Fact]
        public void ObjectToDictionary_WithAnonymousType_LooksUpProperties()
        {
            // Arrange
            var value = new { test = "value", other = 1 };

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(2, dictValues.Count);
            Assert.Equal("value", dictValues["test"]);
            Assert.Equal(1, dictValues["other"]);
        }

        [Fact]
        public void ObjectToDictionary_ReturnsCaseInsensitiveDictionary()
        {
            // Arrange
            var value = new { TEST = "value", oThEr = 1 };

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(2, dictValues.Count);
            Assert.Equal("value", dictValues["test"]);
            Assert.Equal(1, dictValues["other"]);
        }

        [Fact]
        public void ObjectToDictionary_ReturnsInheritedProperties()
        {
            // Arrange
            var value = new ThreeDPoint() { X = 5, Y = 10, Z = 17 };

            // Act
            var dictValues = TypeHelper.ObjectToDictionary(value);

            // Assert
            Assert.NotNull(dictValues);
            Assert.Equal(3, dictValues.Count);
            Assert.Equal(5, dictValues["X"]);
            Assert.Equal(10, dictValues["Y"]);
            Assert.Equal(17, dictValues["Z"]);
        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private class ThreeDPoint : Point
        {
            public int Z { get; set; }
        }
    }
}
