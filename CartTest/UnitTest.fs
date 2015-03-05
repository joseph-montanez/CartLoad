namespace UnitTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Shabb.Store

[<TestClass>]
type UnitTest() = 
    [<TestMethod>]
    member x.Product_ComparePrices_TwoDates () = 
        let p1 = Some(DateTime(2014, 1, 15)), 10.0M<Money.dollars>, 1, 1
        let p2 = Some(DateTime(2014, 2, 10)), 15.0M<Money.dollars>, 1, 1
        let result = Product.ComparePrices p1 p2 1
        let b, price, min, max = result
        Assert.AreEqual(15.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_ComparePrices_TwoDatesSwitched () = 
        let p1 = Some(DateTime(2014, 1, 15)), 10.0M<Money.dollars>, 1, 1
        let p2 = Some(DateTime(2014, 2, 10)), 15.0M<Money.dollars>, 1, 1
        let result = Product.ComparePrices p2 p1 1
        let b, price, min, max = result
        Assert.AreEqual(15.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_ComparePrices_OneDates () = 
        // The start of a compare - empty value
        let p1 = None, 0.0M<Money.dollars>, 1, 1
        let p2 = Some(DateTime(2014, 2, 10)), 15.0M<Money.dollars>, 1, 1
        let result = Product.ComparePrices p1 p2 1
        let b, price, min, max = result
        Assert.AreEqual(15.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_ComparePrices_ZeroDates () = 
        // The start of a compare - empty value
        let p1 = None, 0.0M<Money.dollars>, 1, 1
        let p2 = None, 15.0M<Money.dollars>, 1, 1
        let result = Product.ComparePrices p1 p2 1
        let b, price, min, max = result
        Assert.AreEqual(15.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_ComparePrices_BulkDates_InRange () = 
        // The start of a compare - empty value
        let p1 = Some(DateTime(2014, 1, 15)), 10.0M<Money.dollars>, 1, 1
        let p2 = Some(DateTime(2014, 2, 10)), 15.0M<Money.dollars>, 1, 6
        let result = Product.ComparePrices p1 p2 5
        let b, price, min, max = result
        Assert.AreEqual(15.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_ComparePrices_BulkDates_OutRange () = 
        // The start of a compare - empty value
        let p1 = Some(DateTime(2014, 1, 15)), 10.0M<Money.dollars>, 1, 1
        let p2 = Some(DateTime(2014, 2, 10)), 15.0M<Money.dollars>, 1, 6
        let result = Product.ComparePrices p1 p2 14
        let b, price, min, max = result
        Assert.AreEqual(10.0M<Money.dollars>, price)
    [<TestMethod>]
    member x.Product_CompareRange_Inside () = 
        Assert.AreEqual(true, Product.CompareRange 1 4 1)
        Assert.AreEqual(true, Product.CompareRange 1 4 4)
    [<TestMethod>]
    member x.Product_CompareRange_Outside () = 
        Assert.AreEqual(false, Product.CompareRange 1 4 5)
        Assert.AreEqual(false, Product.CompareRange 1 4 0)
    [<TestMethod>]
    member x.Product_CompareDates () = 
        Assert.AreEqual(true, Product.CompareDates (DateTime(2014, 1, 15)) (DateTime(2014, 2, 20)))
    [<TestMethod>]
    member x.Product_GetPrice_Simple () =
        let apple : Product.Item = { Id = 1; Name = "Apple"; Description = "Shinny red apple"; Price = Product.OnePrice 0.5M }
        Assert.AreEqual(Some(0.5M<Money.dollars>), Product.GetPrice (apple, 1))
    [<TestMethod>]
    member x.Product_GetPrice_Bulk () =
        let apple : Product.Item = {
            Id = 1
            Name = "Apple"
            Description = "Shinny red apple"
            Price = Product.PriceTable.Many [
                        Product.PriceTypes.Bulk ({ Price = 0.5M<Money.dollars>; Start = Some(DateTime.Now); End = None; }, 1, -1)
                        Product.PriceTypes.Bulk ({ Price = 0.4M<Money.dollars>; Start = None; End = None; }, 2, 6)
                        Product.PriceTypes.Bulk ({ Price = 0.2M<Money.dollars>; Start = None; End = None; }, 7, 200)]}
        Assert.AreEqual(Some(0.5M<Money.dollars>), Product.GetPrice (apple, 1))
        Assert.AreEqual(Some(0.4M<Money.dollars>), Product.GetPrice (apple, 2))
        Assert.AreEqual(Some(0.2M<Money.dollars>), Product.GetPrice (apple, 20))
    [<TestMethod>]
    member x.Product_GetPrice_Bulk_Expired () =
        let apple : Product.Item = {
            Id = 1
            Name = "Apple"
            Description = "Shinny red apple"
            Price = Product.PriceTable.Many [
                        Product.PriceTypes.Bulk ({ Price = 0.5M<Money.dollars>; Start = Some(DateTime.Now); End = None; }, 1, -1)
                        Product.PriceTypes.Bulk ({ Price = 0.4M<Money.dollars>; Start = None; End = Some(DateTime(2014, 1, 1)); }, 2, 6)
                        Product.PriceTypes.Bulk ({ Price = 0.2M<Money.dollars>; Start = None; End = None; }, 7, 200)]}
        Assert.AreEqual(Some(0.5M<Money.dollars>), Product.GetPrice (apple, 1))
        Assert.AreEqual(Some(0.5M<Money.dollars>), Product.GetPrice (apple, 2))
        Assert.AreEqual(Some(0.2M<Money.dollars>), Product.GetPrice (apple, 20))