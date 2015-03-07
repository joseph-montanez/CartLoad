CartLoad
========

An Shopping Cart Library in F#. CartLoad is designed for progressive disclosure of its system. As you want to use more of its feature, then the more you'd unravel its rich composable data structure.

## Simple Products ##

There will be several helper functions to help ease into the data structure. So if you only need a simple product with a fixed cost, you can use Product.Simple

    // Products
    let (apple : Product.Item), (orange : Product.Item), (two_cities : Product.Item) =
        Product.Simple 1u "Apple" "Shinny red apple" "apple" 0.5M,
        Product.Simple 2u "Orange" "Juicy orange" "orange" 1.5M,
        Product.Simple 3u "Two Cities" "A tale of two Cities" "two-cities" 23.95M


## Bulk Pricing ##

There is support for bulk pricing if needed, with start and end dates of when a bulk price will be applicable. This is ideal of holiday sales.

	let expiredBulkPricing =
	    Product.PriceTable.Many [
	        Product.PriceTypes.Bulk ({ Price = 0.5M<Money.dollars>; Start = Some(DateTime.Now); End = None; }, 1, -1)
	        Product.PriceTypes.Bulk ({ Price = 0.4M<Money.dollars>; Start = None; End = Some(DateTime(2014, 1, 1)); }, 2, 6)
	        Product.PriceTypes.Bulk ({ Price = 0.2M<Money.dollars>; Start = None; End = None; }, 7, 200)]

    let apple : Product.Item = {
        Id = 1u
        Name = "Apple"
        Description = "Shinny red apple"
        Sku = "apple"
        Price = expiredBulkPricing }

## Shopping Cart ##

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

## Product Options ##

Product options are possible such as color, size, etc. These are still in process but still support price, weight, and sku effects. 

	let colorRedAmountAfter : Option.Item = {
	    Id = 1u
	    Name = "Red"
	    PriceEffect = Product.PriceEffectTypes.Amount
	    Price = Product.OnePrice 0.5M
	    SkuEffect = Product.SkuEffectTypes.After
	    Sku = "r"
	    Order = 0y
	}
	
	let colorBlueAmountAfter : Option.Item = {
	    Id = 2u
	    Name = "Blue"
	    PriceEffect = Product.PriceEffectTypes.Amount
	    Price = Product.OnePrice 0.4M
	    SkuEffect = Product.SkuEffectTypes.After
	    Sku = "b"
	    Order = 0y
	}
	
	let colorGreenAmountAfter : Option.Item = {
	    Id = 3u
	    Name = "Green"
	    PriceEffect = Product.PriceEffectTypes.Amount
	    Price = Product.OnePrice 0.6M
	    SkuEffect = Product.SkuEffectTypes.After
	    Sku = "g"
	    Order = 0y
	}
	
	let simpleColorOptSet = [colorRedAmountAfter; colorBlueAmountAfter; colorGreenAmountAfter]
	
	let colorDetail : Option.ItemSetDetail = { 
	    Id = 1u
	    ItemId = 1u
	    Name = "Color"
	    SkuDelimiter = None
	    Required = true
	    Order = 0y
	}
	
	let color : Option.ItemSet = {
	    Set = colorDetail
	    Options = simpleColorOptSet
	}

## Product Combinations ##

Combinations is needed for product options that are not available or possible. It is also designed to link to another product for complete control over products that need stock level checking. Lets say you have a tshirt product with colors, sizes, etc, and you need to track the stock of each variable since you can only sale whats in stock. You can specify each combination with another product this way you have greater control over pricing like bulk pricing, weight, sku, etc.

	let blueSmallTshirt : Option.Combination = {
	    Id = 1u
	    ItemId = 1u
	    Sku = ""
	    Enabled = true
	    Options = [
	                { Set = colorDetail; Option = colorBlueAmountAfter }
	                { Set = sizeDetail; Option = sizeSmallAmountAfter }
	    ]
	    Order = 0y
	}