using EMLeaderboard.Common.ExceptionHandlers;
using EMLeaderboard.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace EMLeaderboard.UnitTests.ExceptionHandlers
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private readonly GlobalExceptionHandler _exceptionHandler;

        public GlobalExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _exceptionHandler = new GlobalExceptionHandler(_loggerMock.Object);
        }

        private class TestNotFoundException : NotFoundException
        {
            public TestNotFoundException(string entityName, object identifier) : base(entityName, identifier) {}
        }

        [Fact]
        public async Task TryHandleAsync_WhenNotFoundException_Returns404()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new TestNotFoundException("Resource", "123");

            // Act
            var result = await _exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_WhenArgumentException_Returns400()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new ArgumentException("Invalid argument");

            // Act
            var result = await _exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_WhenUnexpectedException_Returns500()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Unexpected error");

            // Act
            var result = await _exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_WhenException_LogsError(){
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test error");

            // Act
            await _exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None); 

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals("An error occurred: Test error", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
} 