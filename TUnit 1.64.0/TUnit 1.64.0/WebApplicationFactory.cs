using Microsoft.AspNetCore.Mvc.Testing;
using TUnit.Core.Interfaces;

namespace TUnit_1._64._0;

public class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public Task InitializeAsync()
    {
        _ = Server;

        return Task.CompletedTask;
    }
}
