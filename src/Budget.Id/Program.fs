namespace Budget.Id

open Giraffe
open Microsoft.AspNetCore.Hosting

module Program =

  [<EntryPoint>]
  let main args =
    WebHostBuilder()
      .UseKestrel()
      .UseStartup<Startup>()
      .UseUrls("http://*:5000")
      .Build()
      .Run()

    0
