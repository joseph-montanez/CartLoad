module main

open System
open Shabb.Store

type Book(id, name, description, price, pages) =
    inherit Product.Item(id, name, description, price)

    member x.Pages : int = pages


[<EntryPoint>]
let main argv =

    // Products
    let apple, orange, two_cities =
        new Product.Item(1, "Apple", "Shinny red apple", Product.OnePrice 0.5M),
        new Product.Item(2, "Orange", "Juicy orange", Product.OnePrice 1.5M),
        new Book(2, "Two Cities", "A tale of two Cities", Product.OnePrice 23.95M, 255)

    // Bag
    let items = [
        apple, 3
        orange, 1
        two_cities :> Product.Item, 2
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
