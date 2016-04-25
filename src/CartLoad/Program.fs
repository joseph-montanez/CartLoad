module main

open System
open Shabb.Store

[<EntryPoint>]
let main argv =

    // Products
    let (apple : Product.Item), (orange : Product.Item), (two_cities : Product.Item) =
        Product.Simple 1u "Apple" "Shinny red apple" "apple" 0.5M,
        Product.Simple 2u "Orange" "Juicy orange" "orange" 1.5M,
        Product.Simple 3u "Two Cities" "A tale of two Cities" "two-cities" 23.95M


    // Bag
    let items = [
        apple, 3
        orange, 1
        two_cities, 2
    ]

    let cart : Cart.Basket = { 
        Id = 1
        Items = items
        Subtotal = 0.00M
        Tax = 0.00M
        Discount = 0.00M
        Total = 0.00M
    }

    Cart.calcSubtotal cart |> printfn "%f"

    printfn "Press any key to continue"
    let ret = Console.Read()
    0 // return an integer exit code