namespace Shabb.Store

open System

module Money =
    [<Measure>] type dollars
    [<Measure>] type cents
    [<Measure>] type USD
    [<Measure>] type CAD

    type Price = {
        Price : decimal<dollars>
        Start : DateTime option
        End   : DateTime option
    }
    
    let GetDateStartPrice (startDate : DateTime, price: decimal<dollars>) =
        match startDate <= DateTime.Now with
        | true -> Some(price)
        | false -> None
    
    let GetDateEndPrice (endDate : DateTime, price: decimal<dollars>) =
        match endDate <= DateTime.Now with
        | true -> Some(price)
        | false -> None
    
    let GetDateRangePrice (startDate : DateTime, endDate : DateTime, price: decimal<dollars>) =
        let p1 = GetDateStartPrice (startDate, price)
        let p2 = GetDateEndPrice (endDate, price)
        match (p1, p2) with
        | (Some(startPrice), Some(endPrice)) -> Some(price)
        | _ -> None
        
    let HasEndPriceWithStart (startDate : DateTime, endDate : DateTime option, price: decimal<dollars>) = 
        match endDate with
        | Some(date) -> GetDateRangePrice (startDate, date, price)
        | None -> GetDateStartPrice (startDate, price)
    
    let HasEndPriceWithoutStart (endDate : DateTime option, price: decimal<dollars>) = 
        match endDate with
        | Some(date) -> GetDateEndPrice (date, price)
        | None -> Some(price)

    let HasStartPrice (price : Price) = 
        match price.Start with
        | Some(startDate) -> HasEndPriceWithStart (startDate, price.End, price.Price)
        | None -> HasEndPriceWithoutStart (price.End, price.Price)

    let GetPrice (price : Price) = HasStartPrice price


    let USDrate = 1.0M<USD>
    let CADrate = 1.09M<CAD>
    let toDollars (money : decimal) = money * 1.0M<dollars>
    let fromDollars (money : decimal<dollars>) = decimal money
    let toUSD (money : decimal<dollars>) = money * USDrate
    let fromUSD (money : decimal<USD>) = decimal money
    let toCAD (money : decimal<dollars>) = money * CADrate
    let fromCAD (money : decimal<CAD>) = decimal money
    let toCents (money : decimal<dollars>) = money * 100.0M<cents>

    let Simple (money : decimal) = { Price = toDollars money; Start = None; End = None }
