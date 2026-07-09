using Bootstrap;
using Domain;
using FastEndpoints;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Presentation.Endpoints.Auth;
using Presentation.IntegrationTests.Builder;
using TUnit.AspNetCore;

namespace Presentation.IntegrationTests;

public abstract class TestBase : WebApplicationTest<WebApiFactory, Program>
{
    [ClassDataSource<TestDatabase>(Shared = SharedType.PerTestSession)]
    public TestDatabase Database { get; init; } = null!;

    public static UserEntity GetTestUser()
    {
        return new UserEntity()
        {
            Username = $"testuser-{Guid.NewGuid()}",
            Password = "password",
            UserRoles =
            [
                new UserRoleEntity { Role = new RoleEntity { Name = Constants.Roles.User } },
            ],
        };
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(Database.DbContainer.GetConnectionString())
                .Options
        );
    }

    public async Task<(HttpClient, UserEntity)> CreateAuthenticatedClientAsync(
        UserEntity user,
        AppDbContext dbContext
    )
    {
        var client = Factory.CreateClient();
        var userBuilder = new UserBuilder(dbContext);
        var userData = userBuilder.WithUser(user).Build();
        var userEntity = userData.Users[0];

        var loginResponse = await client.POSTAsync<
            LoginEndpoint,
            LoginEndpointRequest,
            LoginEndpointResponse
        >(new LoginEndpointRequest(user.Username, user.Password));

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {loginResponse.Result.Token}");
        return (client, userEntity);
    }
}
