using System;
using System.Globalization;
using System.Reflection;
using AutoFixture.Kernel;

namespace AutoFixture.Idioms
{
    /// <summary>
    /// Encapsulates a unit test that verifies that a type which overrides the
    /// <see cref="object.Equals(object)"/> method is implemented correctly with
    /// respect to the symmetric property of equality: `x.Equals(y)` should return same as `y.Equals(x)`.
    /// </summary>
    public class EqualsSymmetricAssertion : IdiomaticAssertion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualsSymmetricAssertion"/> class.
        /// </summary>
        /// <param name="builder">
        /// A composer which can create instances required to implement the idiomatic unit test,
        /// such as the owner of the property, as well as the value to be assigned and read from
        /// the member.
        /// </param>
        /// <remarks>
        /// <para>
        /// <paramref name="builder" /> will typically be a <see cref="Fixture" /> instance.
        /// </para>
        /// </remarks>
        public EqualsSymmetricAssertion(ISpecimenBuilder builder)
        {
            this.Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        /// <summary>
        /// Gets the builder supplied by the constructor.
        /// </summary>
        public ISpecimenBuilder Builder { get; }

        /// <summary>
        /// Verifies that calling `x.Equals(y)` on an instance of the type returns same as `y.Equals(x)`
        /// if the supplied method is an override of the <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="methodInfo">The method to verify.</param>
        public override void Verify(MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            if (methodInfo.ReflectedType == null ||
                !methodInfo.IsObjectEqualsOverrideMethod())
            {
                // The method is not an override of the Object.Equals(object) method
                return;
            }

            var firstInstance = this.Builder.CreateAnonymous(methodInfo.ReflectedType);

            var secondInstance = this.Builder.CreateAnonymous(methodInfo.ReflectedType);

            var xEqualsY = firstInstance.Equals(secondInstance);

            var yEqualsX = secondInstance.Equals(firstInstance);

            if (xEqualsY != yEqualsX)
            {
                throw new EqualsOverrideException(string.Format(CultureInfo.CurrentCulture,
                    "The type '{0}' overrides the object.Equals(object) method incorrectly, " +
                    "calling x.Equals(y) should return same as y.Equals(x).",
                    methodInfo.ReflectedType.FullName));
            }
        }
    }
}
