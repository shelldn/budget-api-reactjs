namespace Budget.Id

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

module Program =
    let BuildWebHost args =
        WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseUrls("http://*:5001")
            .Build()

    [<EntryPoint>]
    let main args =
        BuildWebHost(args).Run()

        0
