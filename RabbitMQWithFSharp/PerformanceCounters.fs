module PerformanceCounters

    open System.Diagnostics
    open System.Linq

    let GetPerformanceCounter category counterName =
        let counter = PerformanceCounterCategory.GetCategories() 
                        |> Array.find ( fun cat -> cat.CategoryName = category) 
        let counter = counter.GetCounters("_Total")
                        |> Array.find ( fun cnt -> cnt.CounterName = counterName )
        ( fun () -> counter.NextValue() )