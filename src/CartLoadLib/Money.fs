namespace Shabb.Store

open System

module Money =

    type Price = {
        Price : decimal
        Start : DateTime option
        End   : DateTime option
    }
    
    let GetDateStartPrice (startDate : DateTime, price: decimal) =
        match startDate <= DateTime.Now with
        | true -> Some(price)
        | false -> None
    
    let GetDateEndPrice (endDate : DateTime, price: decimal) =
        match endDate >= DateTime.Now with
        | true -> Some(price)
        | false -> None
    
    let GetDateRangePrice (startDate : DateTime, endDate : DateTime, price: decimal) =
        let p1 = GetDateStartPrice (startDate, price)
        let p2 = GetDateEndPrice (endDate, price)
        match (p1, p2) with
        | (Some(startPrice), Some(endPrice)) -> Some(price)
        | _ -> None
        
    let HasEndPriceWithStart (startDate : DateTime, endDate : DateTime option, price: decimal) = 
        match endDate with
        | Some(date) -> GetDateRangePrice (startDate, date, price)
        | None -> GetDateStartPrice (startDate, price)
    
    let HasEndPriceWithoutStart (endDate : DateTime option, price: decimal) = 
        match endDate with
        | Some(date) -> GetDateEndPrice (date, price)
        | None -> Some(price)

    let HasStartPrice (price : Price) = 
        match price.Start with
        | Some(startDate) -> HasEndPriceWithStart (startDate, price.End, price.Price)
        | None -> HasEndPriceWithoutStart (price.End, price.Price)

    let GetPrice (price : Price) = HasStartPrice price

    let Simple (money : decimal) = { Price = money; Start = None; End = None }
