using Microsoft.Extensions.Logging;
using System;

namespace RoyalERP_IntegrationTests.Infrastructure;

public class FakeLogger<T> : ILogger<T> {
    
    public IDisposable BeginScope<TState>(TState state) {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) {
        return false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        return;
    }
}
