module UnitTest

open NUnit.Framework
open FsUnit
open Shabb.Store
open System



[<Test>]
let ``when there are two overlapping bulk prices`` () = 
    let p1 = Some(DateTime(2014, 1, 15)), 10.0M, 1, 1
    let p2 = Some(DateTime(2014, 2, 10)), 15.0M, 1, 1
    let result = Product.ComparePrices p1 p2 1
    let b, price, min, max = result
    price |> should equal <| 15.0M

[<Test>]
let ``Product_ComparePrices_TwoDatesSwitched`` () = 
    let p1 = Some(DateTime(2014, 1, 15)), 10.0M, 1, 1
    let p2 = Some(DateTime(2014, 2, 10)), 15.0M, 1, 1
    let result = Product.ComparePrices p2 p1 1
    let b, price, min, max = result
    price |> should equal <| 15.0M
[<Test>]
let ``Product_ComparePrices_OneDates`` () = 
    // The start of a compare - empty value
    let p1 = None, 0.0M, 1, 1
    let p2 = Some(DateTime(2014, 2, 10)), 15.0M, 1, 1
    let result = Product.ComparePrices p1 p2 1
    let b, price, min, max = result
    price |> should equal <| 15.0M
[<Test>]
let ``Product_ComparePrices_ZeroDates`` () = 
    // The start of a compare - empty value
    let p1 = None, 0.0M, 1, 1
    let p2 = None, 15.0M, 1, 1
    let result = Product.ComparePrices p1 p2 1
    let b, price, min, max = result
    price |> should equal <| 15.0M
[<Test>]
let ``Product_ComparePrices_BulkDates_InRange`` () = 
    // The start of a compare - empty value
    let p1 = Some(DateTime(2014, 1, 15)), 10.0M, 1, 1
    let p2 = Some(DateTime(2014, 2, 10)), 15.0M, 1, 6
    let result = Product.ComparePrices p1 p2 5
    let b, price, min, max = result
    price |> should equal <| 15.0M
[<Test>]
let ``Product_ComparePrices_BulkDates_OutRange`` () = 
    // The start of a compare - empty value
    let p1 = Some(DateTime(2014, 1, 15)), 10.0M, 1, 1
    let p2 = Some(DateTime(2014, 2, 10)), 15.0M, 1, 6
    let result = Product.ComparePrices p1 p2 14
    let b, price, min, max = result
    price |> should equal <| 15.0M

[<Test>]
let ``Product_CompareRange_Inside`` () = 
    Product.CompareRange 1 4 1 |> should equal <| true
    Product.CompareRange 1 4 4 |> should equal <| true
[<Test>]
let ``Product_CompareRange_Outside`` () = 
    Product.CompareRange 1 4 5 |> should equal <| false
    Product.CompareRange 1 4 0 |> should equal <| false

[<Test>]
let ``Product_CompareDates`` () = 
    Product.CompareDates (DateTime(2014, 1, 15)) (DateTime(2014, 2, 20)) |> should equal <| true

[<Test>]
let ``Product_GetPrice_Simple`` () =
    let apple : Product.Item = { Id = 1u; Name = "Apple"; Description = "Shinny red apple";  Sku = "apple"; Price = Product.OnePrice 0.5M }
    Product.GetPrice (apple, 1) |> should equal <| Some(0.5M)
    

let expiredBulkPricing =
    Product.PriceTable.Many [
        Product.PriceTypes.Bulk ({ Price = 0.5M; Start = Some(DateTime.Now); End = None; }, 1, -1)
        Product.PriceTypes.Bulk ({ Price = 0.4M; Start = None; End = Some(DateTime(2014, 1, 1)); }, 2, 6)
        Product.PriceTypes.Bulk ({ Price = 0.2M; Start = None; End = None; }, 7, 200)]
[<TestFixture>]
type ``Given an expired bulk pricing setup, it should revert to the unlimited option`` ()=
    let apple : Product.Item = {
        Id = 1u
        Name = "Apple"
        Description = "Shinny red apple"
        Sku = "apple"
        Price = expiredBulkPricing }
        
    [<Test>] member x.
     ``when there is one, the price per unit should be 0.5`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.5M)
        
    [<Test>] member x.
     ``when there is two, the price per unit should be 0.5`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.5M)

    [<Test>] member x.
     ``when there is twenty, the price per unit should be 0.2`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.2M)