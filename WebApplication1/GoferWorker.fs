module GoferWorker

open Gofer.NET
open System

type Worker() =

    member __.Start() = async {
        let taskQueue = TaskQueue.Redis("127.0.0.1:6379")
        let taskClient = new TaskClient(taskQueue)

        let! task = taskClient.TaskScheduler.AddRecurringTask((fun () -> Console.WriteLine("Hello")), TimeSpan.FromSeconds(1.0), "second-span") |> Async.AwaitTask
        return ()
    }
