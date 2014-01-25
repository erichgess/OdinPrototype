module PerformanceCounters

    open System.Diagnostics
    open System.Linq

    let GetPerformanceCounter category counter =
        let counter =   PerformanceCounterCategory.GetCategories() 
                        |> Array.find ( fun cat -> cat.CategoryName = category) 
        counter.GetCounters("_Total")