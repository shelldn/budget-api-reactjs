namespace Budget.Id

open Giraffe
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging

module Program =

  let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
    |> ignore

  [<EntryPoint>]
  let main args =
    WebHostBuilder()
      .UseKestrel()
      .UseStartup<Startup>()
      .UseUrls("http://*:5000")
      .ConfigureLogging(configureLogging)
      .Build()
      .Run()

    0
