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
    public class EqualsTransitiveAssertionTest
    {
        [Fact]
        public void SutIsIdiomaticAssertion()
        {
            // Arrange
            var dummyComposer = new Fixture();
            // Act
            var sut = new EqualsTransitiveAssertion(dummyComposer);
            // Assert
            Assert.IsAssignableFrom<IdiomaticAssertion>(sut);
        }

        [Fact]
        public void ComposerIsCorrect()
        {
            // Arrange
            var expectedComposer = new Fixture();
            var sut = new EqualsTransitiveAssertion(expectedComposer);
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
                new EqualsTransitiveAssertion(null));
        }

        [Fact]
        public void VerifyNullMethodThrows()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsTransitiveAssertion(dummyComposer);
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                sut.Verify((MethodInfo)null));
        }

        [Fact]
        public void VerifyClassThatDoesNotOverrideObjectEqualsDoesNothing()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsTransitiveAssertion(dummyComposer);
            // Act & Assert
            Assert.Null(Record.Exception(() =>
                sut.Verify(typeof(ClassThatDoesNotOverrideObjectEquals))));
        }

        [Fact]
        public void VerifyWellBehavedEqualsTransitiveOverrideDoesNotThrow()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsTransitiveAssertion(dummyComposer);
            // Act & Assert
            Assert.Null(Record.Exception(() =>
                sut.Verify(typeof(WellBehavedEqualsTransitiveObjectOverride))));
        }

        [Fact]
        public void VerifyIllBehavedEqualsTransitiveBehaviourThrows()
        {
            // Arrange
            var dummyComposer = new Fixture();
            var sut = new EqualsTransitiveAssertion(dummyComposer);
            // Act & Assert
            Assert.Throws<EqualsOverrideException>(() =>
                sut.Verify(typeof(IllBehavedEqualsTransitiveObjectOverride)));
        }

#pragma warning disable 659
        private class WellBehavedEqualsTransitiveObjectOverride
        {
            public string DummyStringValue { get; set; }

            public int DummyIntValue { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is WellBehavedEqualsTransitiveObjectOverride other)
                {
                    return this.DummyStringValue.Equals(other.DummyStringValue) && this.DummyIntValue.Equals(other.DummyIntValue);
                }

                throw new Exception();
            }
        }

        private class IllBehavedEqualsTransitiveObjectOverride
        {
            private static bool latestResult = false;

            public override bool Equals(object obj)
            {
                if (obj is IllBehavedEqualsTransitiveObjectOverride other)
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
