using FluentAssertions;

namespace TaskApi.Tests;

public class TaskTests
{
  [Fact] // This tells xUnit: "This is a test!"
  public void TaskItem_Should_Be_Uncompleted_By_Default()
  {
    // 1. Arrange
    var task = new TaskItem { Title = "Test Task" };

    // 2. Act (None needed for this simple property check)

    // 3. Assert (Using FluentAssertions)
    task.IsCompleted.Should().BeFalse();
    task.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
  }
}