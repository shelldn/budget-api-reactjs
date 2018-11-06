namespace Budget.Id

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open IdentityServer4.Models
open IdentityServer4.Test
open IdentityServer4.Services
open IdentityServer4.Events
open IdentityServer4
open Giraffe
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Authentication

module Views =
  open Giraffe.GiraffeViewEngine

  let login =
    html [] [
      head [] [
        title [] [ encodedText "Budget.io - Sign In" ]
      ]
      body [] [
        p [] [ encodedText "Sign in please:" ]
        form [ _method "POST" ] [
          input [ _type "text"; _name "username"; _placeholder "Username" ]
          input [ _type "password"; _name "password"; _placeholder "Password" ]
          input [ _type "submit" ]
        ]
      ]
    ]

[<CLIMutable>]
type LoginViewModel =
  { Username : string
    Password : string }

type Startup private () =

  let loginHandler (next : HttpFunc) (ctx : HttpContext) =
    task {
      let! vm = ctx.BindFormAsync<LoginViewModel>()

      if vm.Username = "shelldn" && vm.Password = "qwerty123"
      then
        do! ctx.SignInAsync("1", "shelldn", AuthenticationProperties())

        let returnUrl = ctx.TryGetQueryStringValue "returnUrl"

        match returnUrl with
        | Some url -> return! redirectTo false url next ctx
        | None -> return! next ctx

      else
        return! RequestErrors.BAD_REQUEST "Error." next ctx
    }

  let logoutHandler (next : HttpFunc) (ctx : HttpContext) =
    task {
      let interaction = ctx.GetService<IIdentityServerInteractionService>()

      do! ctx.SignOutAsync()

      match ctx.TryGetQueryStringValue "logoutId" with
      | Some id ->
        let! logout = interaction.GetLogoutContextAsync id
        return! redirectTo false logout.PostLogoutRedirectUri next ctx
      | None -> return! next ctx
    }

  let api =
    choose [
      GET >=> route "/account/login" >=> htmlView Views.login
      POST >=> route "/account/login" >=> loginHandler
      route "/account/logout" >=> logoutHandler >=> redirectTo false "http://localhost:3000"
    ]

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
        RequireConsent = false,
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
