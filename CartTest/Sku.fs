module Sku

open NUnit.Framework
open FsUnit
open Shabb.Store
open System

//-- Color
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

//-- Size
let sizeSmallAmountAfter : Option.Item = {
    Id = 1u
    Name = "Small"
    PriceEffect = Product.PriceEffectTypes.Amount
    Price = Product.OnePrice 1.0M
    SkuEffect = Product.SkuEffectTypes.After
    Sku = "s"
    Order = 0y
}
let sizeSmallAmountBefore : Option.Item = {
    Id = 1u
    Name = "Small"
    PriceEffect = Product.PriceEffectTypes.Amount
    Price = Product.OnePrice 1.0M
    SkuEffect = Product.SkuEffectTypes.Before
    Sku = "s"
    Order = 0y
}
let sizeMediumAmountAfter : Option.Item = {
    Id = 1u
    Name = "Medium"
    PriceEffect = Product.PriceEffectTypes.Amount
    Price = Product.OnePrice 1.0M
    SkuEffect = Product.SkuEffectTypes.After
    Sku = "m"
    Order = 0y
}
let sizeLargeAmountAfter : Option.Item = {
    Id = 1u
    Name = "Large"
    PriceEffect = Product.PriceEffectTypes.Amount
    Price = Product.OnePrice 1.0M
    SkuEffect = Product.SkuEffectTypes.After
    Sku = "l"
    Order = 0y
}
let sizeExtraLargeAmountAfter : Option.Item = {
    Id = 1u
    Name = "Extra Large"
    PriceEffect = Product.PriceEffectTypes.Amount
    Price = Product.OnePrice 1.0M
    SkuEffect = Product.SkuEffectTypes.After
    Sku = "xl"
    Order = 0y
}

let simpleSizeOptSet = [sizeSmallAmountAfter; sizeMediumAmountAfter; sizeLargeAmountAfter; sizeExtraLargeAmountAfter]

let sizeDetail : Option.ItemSetDetail = { 
    Id = 1u
    ItemId = 1u
    Name = "Size"
    SkuDelimiter = None
    Required = true
    Order = 0y
}

let size : Option.ItemSet = {
    Set = sizeDetail
    Options = simpleSizeOptSet
}

let tshirt : Product.Item = {
    Id = 1u
    Name = "T-Shirt"
    Description = "Best Ever!"
    Sku = "tshirt"
    Price = Product.OnePrice 19.95M }

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


[<Test>]
let ``when there is a blue small shirt the sku added after is tshirt-b-s`` () = 
    let sku = Option.BuildSkuDefault tshirt.Sku blueSmallTshirt.Options
    sku |> should equal <| "tshirt-b-s"
    
[<Test>]
let ``when there is a blue small shirt and the small sku is prefixed it should be s-tshirtb`` () =
    let chosenOptions : Option.ItemChoice list = [
                { Set = colorDetail; Option = colorBlueAmountAfter }
                { Set = sizeDetail; Option = sizeSmallAmountBefore }
    ]
    let newBlueShirt = {blueSmallTshirt with Options = chosenOptions}
    let sku = Option.BuildSkuDefault tshirt.Sku newBlueShirt.Options
    sku |> should equal <| "s-tshirt-b"

[<Test>]
let ``when there is a blue small shirt and the small + blue sku is prefixed it should be b-s-tshirt`` () =
    let colorBlueAmountStartOf = {colorBlueAmountAfter with SkuEffect = Product.SkuEffectTypes.StartOf}
    let sizeSmallAmountStartOf = {sizeSmallAmountAfter with SkuEffect = Product.SkuEffectTypes.StartOf}
    let chosenOptions : Option.ItemChoice list = [
                { Set = colorDetail; Option = colorBlueAmountStartOf }
                { Set = sizeDetail; Option = sizeSmallAmountStartOf }
    ]
    let newBlueShirt = {blueSmallTshirt with Options = chosenOptions}
    let sku = Option.BuildSkuDefault tshirt.Sku newBlueShirt.Options
    sku |> should equal <| "b-s-tshirt"