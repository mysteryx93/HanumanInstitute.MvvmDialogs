using Moq;
using Xunit.Abstractions;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Tests;

/// <summary>
/// Base class for test classes.
/// </summary>
public class TestsBase
{
    protected ITestOutputHelper Output { get; }

    public TestsBase(ITestOutputHelper output) => Output = output;

    /// <summary>
    /// Allows using a lambda expression after ??= operator.
    /// </summary>
    protected T Init<T>(Func<T> func) => func();

    /// <summary>
    /// Initializes an new object of specified type with parameterless constructor.
    /// </summary>
    /// <param name="action">A lambda expression to configure the object.</param>
    /// <typeparam name="T">The type of object to create.</typeparam>
    /// <returns>The newly-created object.</returns>
    protected T Init<T>(Action<T> action)
        where T : new()
    {
        var result = new T();
        action(result);
        return result;
    }
    
    /// <summary>
    /// Initializes a mock of specified type.
    /// </summary>
    /// <param name="action">A lambda expression to configure the mock.</param>
    /// <typeparam name="T">The type of mock to create.</typeparam>
    /// <returns>The newly-created mock.</returns>
    protected Mock<T> InitMock<T>(Action<Mock<T>> action)
        where T : class
    {
        var mock = new Mock<T>();
        action(mock);
        return mock;
    } 
}
