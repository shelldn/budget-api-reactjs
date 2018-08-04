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

    let client =
      Client(
        ClientId = "js",
        AllowedGrantTypes = GrantTypes.Implicit,
        AllowAccessTokensViaBrowser = true,
        AllowedScopes = allowedScopes)

    services.AddMvc() |> ignore
    services
      .AddIdentityServer()
      .AddInMemoryClients([client])
      .AddInMemoryApiResources([])
      .AddDeveloperSigningCredential()
    |> ignore

  member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
    app.UseMvc() |> ignore
    app.UseIdentityServer() |> ignore

  member val Configuration : IConfiguration = null with get, set
