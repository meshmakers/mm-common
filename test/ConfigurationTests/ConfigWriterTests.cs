using System.IO.Abstractions;
using Meshmakers.Common.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Meshmakers.Common.ConfigurationTests;

public class UnitTest1
{
    [Fact]
    public void ConfigWriterTests()
    {
        // Arrange
        var fileSystem = Mock.Of<IFileSystem>();
        var file = Mock.Of<IFile>();
        var directory = Mock.Of<IDirectory>();
        Mock.Get(fileSystem).Setup(fs => fs.Directory).Returns(directory);
        Mock.Get(fileSystem).Setup(fs => fs.File).Returns(file);
        Mock.Get(file).Setup(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));
        Mock.Get(directory).Setup(d => d.Exists(It.IsAny<string>())).Returns(true);

        var options = new OptionsWrapper<TestOptions>(new TestOptions());

        // Execute
        var writer = new ConfigWriter(fileSystem);
        writer.AddOptions("UnitTest", options);

        writer.WriteSettings("testapp");

        // Validate
        Mock.Get(file).Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ConfigWriterTestsTwoOptions()
    {
        // Arrange
        var fileSystem = Mock.Of<IFileSystem>();
        var file = Mock.Of<IFile>();
        var directory = Mock.Of<IDirectory>();
        Mock.Get(fileSystem).Setup(fs => fs.Directory).Returns(directory);
        Mock.Get(fileSystem).Setup(fs => fs.File).Returns(file);
        Mock.Get(file).Setup(fs => fs.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));
        Mock.Get(directory).Setup(d => d.Exists(It.IsAny<string>())).Returns(true);

        var options1 = new OptionsWrapper<TestOptions>(new TestOptions { Test = "FirstValueTest" });
        var options2 = new OptionsWrapper<SecondOptions>(new SecondOptions { SecondTest = "SecondValueTest" });

        // Execute
        var writer = new ConfigWriter(fileSystem);
        writer.AddOptions("UnitTest1", options1);
        writer.AddOptions("UnitTest2", options2);

        writer.WriteSettings("testapp");

        // Validate
        Mock.Get(file).Verify(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        Mock.Get(file).Verify(f => f.WriteAllText(It.IsAny<string>(), It.Is<string>(s => s.Contains("FirstValueTest"))),
            Times.Once);
        Mock.Get(file)
            .Verify(f => f.WriteAllText(It.IsAny<string>(), It.Is<string>(s => s.Contains("SecondValueTest"))),
                Times.Once);
    }
}
