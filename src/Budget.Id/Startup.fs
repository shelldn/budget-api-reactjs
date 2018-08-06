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
open Giraffe

type Startup private () =
  let api =
    route "/account/login" >=> text "Hello, user thas is trying to authorize."

  new (configuration: IConfiguration) as this =
    Startup() then
    this.Configuration <- configuration

  member __.ConfigureServices(services: IServiceCollection) =
    
    services.AddGiraffe() |> ignore

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
        ClientId = "js",
        ClientName = "JavaScript Client",
        AllowedGrantTypes = GrantTypes.Implicit,
        AllowAccessTokensViaBrowser = true,
        RedirectUris = List [ "http://localhost:3000/callback.html" ],
        PostLogoutRedirectUris = List [ "http://localhost:3000/index.html" ],
        AllowedCorsOrigins = List [ "http://localhost:3000" ],
        AllowedScopes = allowedScopes)

    let test_user =
      TestUser(
        SubjectId = "1",
        Username = "shelldn",
        Password = "qwerty123"
      )

    services
      .AddIdentityServer()
      .AddDeveloperSigningCredential()
      .AddInMemoryIdentityResources([ IdentityResources.OpenId() :> IdentityResource; IdentityResources.Profile() :> IdentityResource ])
      .AddInMemoryApiResources([ api_resource ])
      .AddInMemoryClients([ client ])
      .AddTestUsers(List [ test_user ])
    |> ignore

  member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
    app.UseGiraffe api
    app.UseCors(fun b -> b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() |> ignore) |> ignore
    app.UseIdentityServer() |> ignore

  member val Configuration : IConfiguration = null with get, set
