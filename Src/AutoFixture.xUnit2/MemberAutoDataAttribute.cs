﻿using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace AutoFixture.Xunit2
{
    /// <summary>
    /// Provides a data source for a data theory, with the data coming from one of the following sources
    /// and combined with auto-generated data specimens generated by AutoFixture:
    /// 1. A static property
    /// 2. A static field
    /// 3. A static method (with parameters)
    /// The member must return something compatible with IEnumerable&lt;object[]&gt; with the test data.
    /// </summary>
    [DataDiscoverer(
        typeName: "AutoFixture.Xunit2.NoPreDiscoveryDataDiscoverer",
        assemblyName: "AutoFixture.Xunit2")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [CLSCompliant(false)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is the root of a potential attribute hierarchy.")]
    public class MemberAutoDataAttribute : CompositeDataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        public MemberAutoDataAttribute(string memberName, params object[] parameters)
            : this(new AutoDataAttribute(), memberName, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute"/> class.
        /// </summary>
        /// <param name="autoDataAttribute">An <see cref="DataAttribute"/>.</param>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        /// <remarks>
        /// <para>
        /// This constructor overload exists to enable a derived attribute to
        /// supply a custom <see cref="DataAttribute" /> that again may
        /// contain custom behavior.
        /// </para>
        /// </remarks>
        protected MemberAutoDataAttribute(DataAttribute autoDataAttribute, string memberName, params object[] parameters)
            : base(new MemberDataAttribute(memberName, parameters), autoDataAttribute)
        {
            this.AutoDataAttribute = autoDataAttribute ?? throw new ArgumentNullException(nameof(autoDataAttribute));
            this.MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Gets the attribute used to automatically generate the remaining theory parameters, which are not fixed.
        /// </summary>
        public DataAttribute AutoDataAttribute { get; }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets the parameters passed to the member. Only supported for static methods.
        /// </summary>
        public IEnumerable<object> Parameters { get; }
    }
}