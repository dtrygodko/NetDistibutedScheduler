module BackgroundWorker

open Hangfire
open System

type Worker() =
    
    member __.StartJob() =
        RecurringJob.AddOrUpdate((fun () -> Console.WriteLine("Hello")), "* * * * * *")
        ()