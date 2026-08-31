using FluentAssertions;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var result = Result.Failure("Something went wrong.");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Something went wrong.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Failure_ShouldRequireErrorMessage(string? error)
    {
        Action act = () => Result.Failure(error!);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("A failure result must include an error message.*")
            .And.ParamName.Should().Be("error");
    }

    [Fact]
    public void GenericSuccess_ShouldCreateSuccessfulResultWithValue()
    {
        var result = Result<int>.Success(7);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);
        result.Error.Should().BeEmpty();
    }

    [Fact]
    public void GenericFailure_ShouldCreateFailedResultWithDefaultValue()
    {
        var result = Result<string>.Failure("No value available.");

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("No value available.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void GenericFailure_ShouldRequireErrorMessage(string? error)
    {
        Action act = () => Result<string>.Failure(error!);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("A failure result must include an error message.*")
            .And.ParamName.Should().Be("error");
    }
}
