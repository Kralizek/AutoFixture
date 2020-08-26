using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.Kernel;
using Xunit;

namespace AutoFixture.IdiomsUnitTest
{
    public class EqualsSymmetricAssertionTest
    {
        [Fact]
        public void SutIsIdiomaticAssertion()
        {
            // Arrange
            var dummyComposer = new Fixture();
            // Act
            var sut = new EqualsSymmetricAssertion(dummyComposer);
            // Assert
            Assert.IsAssignableFrom<IdiomaticAssertion>(sut);
        }

        [Fact]
        public void ComposerIsCorrect()
        {
            // Arrange
            var expectedComposer = new Fixture();
            var sut = new EqualsSymmetricAssertion(expectedComposer);
            // Act
            ISpecimenBuilder result = sut.Builder;
            // Assert
            Assert.Equal(expectedComposer, result);
        }

        [Fact]
        public void ConstructWithNullComposerThrows()
        {
            // Arrange
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new EqualsSymmetricAssertion(null));
        }

        [Fact]
        public void VerifyNullMethodThrows()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsSymmetricAssertion(dummyComposer);
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                sut.Verify((MethodInfo)null));
        }

        [Fact]
        public void VerifyClassThatDoesNotOverrideObjectEqualsDoesNothing()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsSymmetricAssertion(dummyComposer);
            // Act & Assert
            Assert.Null(Record.Exception(() =>
                sut.Verify(typeof(ClassThatDoesNotOverrideObjectEquals))));
        }

        [Fact]
        public void VerifyWellBehavedEqualsSymmetricOverrideDoesNotThrow()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsSymmetricAssertion(dummyComposer);
            // Act & Assert
            Assert.Null(Record.Exception(() =>
                sut.Verify(typeof(WellBehavedEqualsSymmetricObjectOverride))));
        }

        [Fact]
        public void VerifyIllBehavedEqualsSymmetricBehaviourThrows()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsSymmetricAssertion(dummyComposer);
            // Act & Assert
            Assert.Throws<EqualsOverrideException>(() =>
                sut.Verify(typeof(IllBehavedEqualsSymmetricObjectOverride)));
        }

#pragma warning disable 659
        private class WellBehavedEqualsSymmetricObjectOverride
        {
            public string DummyStringValue { get; set; }

            public int DummyIntValue { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is WellBehavedEqualsSymmetricObjectOverride other)
                {
                    return this.DummyStringValue.Equals(other.DummyStringValue) && this.DummyIntValue.Equals(other.DummyIntValue);
                }

                throw new Exception();
            }
        }

        private class IllBehavedEqualsSymmetricObjectOverride
        {
            private static bool latestResult = false;

            public override bool Equals(object obj)
            {
                if (obj is IllBehavedEqualsSymmetricObjectOverride other)
                {
                    latestResult = !latestResult;
                    return latestResult;
                }

                throw new Exception();
            }
        }
#pragma warning restore 659

        private class ClassThatDoesNotOverrideObjectEquals
        {
        }
    }
}
