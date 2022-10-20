using System;
using Meshmakers.Common.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meshmakers.Common.SharedTests;

[TestClass]
public class ArgumentValidationTests
{
    private const string TestArgument = "test";

    [TestMethod]
    public void ValidateTyped_OK()
    {
        ArgumentValidation.Validate<int>(TestArgument, 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ValidateTyped_Fail()
    {
        ArgumentValidation.Validate<int>(TestArgument, "string");
    }
}