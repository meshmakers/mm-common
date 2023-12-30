using Meshmakers.Common.CommandLineParser;
using Meshmakers.Common.CommandLineParser.Commands;
using NSubstitute;
using Xunit;

namespace Meshmakers.Common.CommandLineParserTests;

public class CommandParserTests
{
    private readonly ICommandArgument _commandArgument = Substitute.For<ICommandArgument>();

    private readonly ICommand[] _commandList;
    private readonly ICommand _commandT = Substitute.For<ICommand>();
    private readonly ICommandArgumentValue _commandTArgValue = Substitute.For<ICommandArgumentValue>();
    private readonly ICommand _commandU = Substitute.For<ICommand>();
    private readonly ICommandArgumentValue _commandUArgValue = Substitute.For<ICommandArgumentValue>();
    private readonly ICommand _commandV = Substitute.For<ICommand>();
    private readonly ICommandArgumentValue _commandVArgValue = Substitute.For<ICommandArgumentValue>();
    private readonly ICommand _commandW = Substitute.For<ICommand>();
    private readonly ICommandArgumentValue _commandWArgValue = Substitute.For<ICommandArgumentValue>();
    private readonly IParserService _stubParserService = Substitute.For<IParserService>();

    public CommandParserTests()
    {
        _stubParserService.AddCommandArgument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string[]>(),
            Arg.Any<bool>()).Returns(info => _commandArgument);


        _commandT.CommandArgumentValue.Returns(_commandTArgValue);
        _commandU.CommandArgumentValue.Returns(_commandUArgValue);
        _commandV.CommandArgumentValue.Returns(_commandVArgValue);
        _commandW.CommandArgumentValue.Returns(_commandWArgValue);

        _commandTArgValue.Value.Returns("t");
        _commandUArgValue.Value.Returns("u");
        _commandVArgValue.Value.Returns("v");
        _commandWArgValue.Value.Returns("w");

        _commandList = new[]
        {
            _commandT,
            _commandU,
            _commandV,
            _commandW
        };
    }


    [Fact]
    public void CommandParser_ShowUsageInformation_OK()
    {
        var commandParser = new CommandParser(_stubParserService, _commandList);

        commandParser.ShowUsageInformation("Demo.exe");

        _stubParserService.Received().ShowUsageInformation("Demo.exe");
    }

    [Theory]
    [InlineData("t")]
    [InlineData("T")]
    public async Task CommandParser_ParseAndValidateAsync_OK(string param)
    {
        IArgumentValue argumentValue = Substitute.For<IArgumentValue>();
        argumentValue.GetValue<string>(Arg.Is(0)).Returns(param);

        _stubParserService.GetArgumentValue(Arg.Any<ICommandArgument>()).Returns(argumentValue);


        var commandParser = new CommandParser(_stubParserService, _commandList);

        await commandParser.ParseAndValidateAsync();

        _stubParserService.Received(1).ParseAndValidate();

        await _commandT.Received(1).PreValidate();
        await _commandT.Received(1).Execute();
    }

    [Theory]
    [InlineData("z")]
    [InlineData("")]
    [InlineData(null)]
    public async Task CommandParser_ParseAndValidateAsync_InvalidArgument_Fail(string? param)
    {
        IArgumentValue argumentValue = Substitute.For<IArgumentValue>();
        argumentValue.GetValue<string>(0).Returns(param);

        _stubParserService.GetArgumentValue(Arg.Any<ICommandArgument>()).Returns(argumentValue);


        var commandParser = new CommandParser(_stubParserService, _commandList);

        await Assert.ThrowsAsync<InvalidProgramException>(() => commandParser.ParseAndValidateAsync());
    }
}
