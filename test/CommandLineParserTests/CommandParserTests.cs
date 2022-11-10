using System;
using System.Threading.Tasks;
using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using Moq;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class CommandParserTests
{
    private readonly Mock<ICommandArgument> _commandArgument = new();

    private readonly ICommand[] _commandList;
    private readonly Mock<ICommand> _commandT = new();
    private readonly Mock<ICommandArgumentValue> _commandTArgValue = new();
    private readonly Mock<ICommand> _commandU = new();
    private readonly Mock<ICommandArgumentValue> _commandUArgValue = new();
    private readonly Mock<ICommand> _commandV = new();
    private readonly Mock<ICommandArgumentValue> _commandVArgValue = new();
    private readonly Mock<ICommand> _commandW = new();
    private readonly Mock<ICommandArgumentValue> _commandWArgValue = new();
    private readonly Mock<IParserService> _stubParserService = new();

    public CommandParserTests()
    {
        _stubParserService
            .Setup(x => x.AddCommandArgument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>(),
                It.IsAny<bool>())).Returns(_commandArgument.Object);
        _commandT.SetupGet(x => x.CommandArgumentValue).Returns(_commandTArgValue.Object);
        _commandU.SetupGet(x => x.CommandArgumentValue).Returns(_commandUArgValue.Object);
        _commandV.SetupGet(x => x.CommandArgumentValue).Returns(_commandVArgValue.Object);
        _commandW.SetupGet(x => x.CommandArgumentValue).Returns(_commandWArgValue.Object);

        _commandTArgValue.SetupGet(x => x.Value).Returns("t");
        _commandUArgValue.SetupGet(x => x.Value).Returns("u");
        _commandVArgValue.SetupGet(x => x.Value).Returns("v");
        _commandWArgValue.SetupGet(x => x.Value).Returns("w");

        _commandList = new[]
        {
            _commandT.Object,
            _commandU.Object,
            _commandV.Object,
            _commandW.Object
        };
    }


    [Fact]
    public void CommandParser_ShowUsageInformation_OK()
    {
        var commandParser = new CommandParser(_stubParserService.Object, _commandList);

        commandParser.ShowUsageInformation("Demo.exe");

        _stubParserService.Verify(service => service.ShowUsageInformation("Demo.exe"));
    }

    [Theory]
    [InlineData("t")]
    [InlineData("T")]
    public async Task CommandParser_ParseAndValidateAsync_OK(string param)
    {
        Mock<IArgumentValue> argumentValue = new();
        argumentValue.Setup(x => x.GetValue<string>(0)).Returns(param);

        _stubParserService.Setup(x => x.GetArgumentValue(It.IsAny<ICommandArgument>()))
            .Returns(argumentValue.Object);

        var commandParser = new CommandParser(_stubParserService.Object, _commandList);

        await commandParser.ParseAndValidateAsync();

        _stubParserService.Verify(x => x.ParseAndValidate(), Times.Once());
        _commandT.Verify(x => x.PreValidate(), Times.Once());
        _commandT.Verify(x => x.Execute(), Times.Once());
    }

    [Theory]
    [InlineData("z")]
    [InlineData("")]
    [InlineData(null)]
    public async Task CommandParser_ParseAndValidateAsync_InvalidArgument_Fail(string param)
    {
        Mock<IArgumentValue> argumentValue = new();
        argumentValue.Setup(x => x.GetValue<string>(0)).Returns(param);

        _stubParserService.Setup(x => x.GetArgumentValue(It.IsAny<ICommandArgument>()))
            .Returns(argumentValue.Object);

        var commandParser = new CommandParser(_stubParserService.Object, _commandList);

        await Assert.ThrowsAsync<InvalidProgramException>(() => commandParser.ParseAndValidateAsync());
    }
}
