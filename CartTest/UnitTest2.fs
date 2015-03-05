module UnitTest2

open NUnit.Framework
open FsUnit
open Shabb.Store
open System


let expiredBulkPricing =
    Product.PriceTable.Many [
        Product.PriceTypes.Bulk ({ Price = 0.5M<Money.dollars>; Start = Some(DateTime.Now); End = None; }, 1, -1)
        Product.PriceTypes.Bulk ({ Price = 0.4M<Money.dollars>; Start = None; End = Some(DateTime(2014, 1, 1)); }, 2, 6)
        Product.PriceTypes.Bulk ({ Price = 0.2M<Money.dollars>; Start = None; End = None; }, 7, 200)]


[<TestFixture>]
type ``Given an expired bulk pricing setup, it should revert to the unlimited option`` ()=
    let apple : Product.Item = {
        Id = 1
        Name = "Apple"
        Description = "Shinny red apple"
        Price = expiredBulkPricing }
        
    [<Test>] member x.
     ``when there is one, the price per unit should be 0.5`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.5M<Money.dollars>)
        
    [<Test>] member x.
     ``when there is two, the price per unit should be 0.5`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.5M<Money.dollars>)

    [<Test>] member x.
     ``when there is twenty, the price per unit should be 0.2`` ()=
        Product.GetPrice (apple, 1) |> should equal <| Some(0.2M<Money.dollars>)