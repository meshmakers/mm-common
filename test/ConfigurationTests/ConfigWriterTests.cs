using System.IO.Abstractions;
using Meshmakers.Common.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.ConfigurationTests;

public class UnitTest1
{
    [Fact]
    public void ConfigWriterTests()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var file = Substitute.For<IFile>();
        var directory = Substitute.For<IDirectory>();
        fileSystem.Directory.Returns(directory);
        fileSystem.File.Returns(file);

        var options = new OptionsWrapper<TestOptions>(new TestOptions());

        // Execute
        var writer = new ConfigWriter(fileSystem);
        writer.AddOptions("UnitTest", options);

        writer.WriteSettings("testapp");

        // Validate
        file.Received(1).WriteAllText(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void ConfigWriterTestsTwoOptions()
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var file = Substitute.For<IFile>();
        var directory = Substitute.For<IDirectory>();
        fileSystem.Directory.Returns(directory);
        fileSystem.File.Returns(file);

        var options1 = new OptionsWrapper<TestOptions>(new TestOptions { Test = "FirstValueTest" });
        var options2 = new OptionsWrapper<SecondOptions>(new SecondOptions { SecondTest = "SecondValueTest" });

        // Execute
        var writer = new ConfigWriter(fileSystem);
        writer.AddOptions("UnitTest1", options1);
        writer.AddOptions("UnitTest2", options2);

        writer.WriteSettings("testapp");

        // Validate
        file.Received(1).WriteAllText(Arg.Any<string>(), Arg.Any<string>());
        file.Received(1).WriteAllText(Arg.Any<string>(), Arg.Is<string>(s => s.Contains("FirstValueTest")));
        file.Received(1).WriteAllText(Arg.Any<string>(), Arg.Is<string>(s => s.Contains("SecondValueTest")));
    }
}
