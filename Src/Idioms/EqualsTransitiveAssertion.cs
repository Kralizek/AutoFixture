using System;
using System.Globalization;
using System.Reflection;
using AutoFixture.Kernel;

namespace AutoFixture.Idioms
{
    /// <summary>
    /// Encapsulates a unit test that verifies that a type which overrides the
    /// <see cref="object.Equals(object)"/> method is implemented correctly with
    /// respect to the transitive property of equality: if `x.Equals(y) == true` and `y.Equals(z) == true`, then `x.Equals(z) == true`.
    /// </summary>
    public class EqualsTransitiveAssertion : IdiomaticAssertion
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
        public EqualsTransitiveAssertion(ISpecimenBuilder builder)
        {
            this.Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        /// <summary>
        /// Gets the builder supplied by the constructor.
        /// </summary>
        public ISpecimenBuilder Builder { get; }

        /// <summary>
        /// If the supplied method is an override of the <see cref="object.Equals(object)"/>,
        /// verifies that `Equals` respects the transitive property of equality.
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
            var thirdInstance = this.Builder.CreateAnonymous(methodInfo.ReflectedType);

            var xEqualsY = firstInstance.Equals(secondInstance);
            var yEqualsZ = secondInstance.Equals(thirdInstance);
            var xEqualsZ = firstInstance.Equals(thirdInstance);

            if (xEqualsY == yEqualsZ && xEqualsY == xEqualsZ)
            {
                // Transitive property is respected, do nothing.
            }
            else
            {
                throw new EqualsOverrideException(string.Format(CultureInfo.CurrentCulture,
                    "The type '{0}' overrides the object.Equals(object) method incorrectly, " +
                    "Equals should respect the transitive property of equality: if (a == b && b == c) then (a == c).",
                    methodInfo.ReflectedType.FullName));
            }
        }
    }
}
