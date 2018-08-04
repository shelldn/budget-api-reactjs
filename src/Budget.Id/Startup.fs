namespace Budget.Id

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open IdentityServer4.Models
open IdentityServer4.Test
open IdentityServer4

type Startup private () =
  new (configuration: IConfiguration) as this =
    Startup() then
    this.Configuration <- configuration

  member __.ConfigureServices(services: IServiceCollection) =

    let allowedScopes =
      [
        IdentityServerConstants.StandardScopes.OpenId
        IdentityServerConstants.StandardScopes.Profile
        "api1"
      ]
      |> List<string>

    let api_resource = ApiResource("api1", "Some API 1")

    let client =
      Client(
        ClientId = "ro.client",
        ClientSecrets = List [ Secret("secret".Sha256()) ],
        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        AllowedScopes = allowedScopes)

    let test_user =
      TestUser(
        SubjectId = "1",
        Username = "shelldn",
        Password = "qwerty123"
      )

    services.AddMvc() |> ignore
    services
      .AddIdentityServer()
      .AddDeveloperSigningCredential()
      .AddInMemoryApiResources([ api_resource ])
      .AddInMemoryClients([ client ])
      .AddTestUsers(List [ test_user ])
    |> ignore

  member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
    app.UseMvc() |> ignore
    app.UseIdentityServer() |> ignore

  member val Configuration : IConfiguration = null with get, set
