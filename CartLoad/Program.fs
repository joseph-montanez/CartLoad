module main

open System
open Shabb.Store

[<EntryPoint>]
let main argv =

    // Products
    let (apple : Product.Item), (orange : Product.Item), (two_cities : Product.Item) =
        { Id = 1; Name = "Apple"; Description = "Shinny red apple"; Price = Product.OnePrice 0.5M },
        { Id = 2; Name = "Orange"; Description = "Juicy orange"; Price = Product.OnePrice 1.5M },
        { Id = 3; Name = "Two Cities"; Description = "A tale of two Cities"; Price = Product.OnePrice 23.95M }


    // Bag
    let items = [
        apple, 3
        orange, 1
        two_cities, 2
    ]

    let cart : Cart.Basket = { 
        Id = 1
        Items = items
        Subtotal = 0.00M<Money.dollars>
        Tax = 0.00M<Money.dollars>
        Discount = 0.00M<Money.dollars>
        Total = 0.00M<Money.dollars> 
    }

    Cart.calcSubtotal cart |> Money.fromDollars |> printfn "%f"

    printfn "Press any key to continue"
    let ret = Console.Read()
    0 // return an integer exit code
