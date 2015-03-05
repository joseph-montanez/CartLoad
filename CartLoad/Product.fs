namespace Shabb.Store

open System

module Product =
    type PriceTypes =
        | Simple of Money.Price
        | Bulk of Money.Price * int * int

    type PriceEffectTypes =
        | Replace
        | Amount
        | Percent
        
    type PriceTable =
        | One of PriceTypes
        | Many of list<PriceTypes>
        
    type Item = {
        Id : int
        Name : string
        Description : string
        Price : PriceTable
    }

    type ItemOption = {
        Id : int
        Name : string
        PriceEffect : PriceEffectTypes
        Price : PriceTable
        SKU : string
        Order : int
    }

    type ItemOptionSet = {
        Id : int
        ItemId : int
        Name : string
        Required : bool
        Options : ItemOption[]
        Order : int
    }

    type ItemCombination = {
        Id : int
        ItemId : int
        Enabled : bool
        Options : ItemOption[]
        Order : int
    }

    type ValidDatePrice = DateTime option * decimal<Money.dollars> * int * int

    let SimplePrice = PriceTypes.Simple << Money.Simple
    let OnePrice = PriceTable.One << SimplePrice

    /// Exclusive 'between' operator:
    let (><) x (min, max) =
        (x > min) && (x < max)

    /// Inclusive 'between' operator:
    let (>=<) x (min, max) =
        (x >= min) && (x <= max)

    let GetMinPriceDate (itemStart : DateTime option, itemEnd : DateTime option) =
        match itemStart, itemEnd with
        | (Some(startDate), Some(endDate)) -> itemStart
        | (Some(startDate), None) -> itemStart
        | (None, Some(endDate)) -> itemEnd
        | (None, None) -> Some(DateTime.Now)

    let GetPriceFromSimplePrice simplePrice =
        match Money.GetPrice simplePrice with
        | Some(money) -> (GetMinPriceDate (simplePrice.Start, simplePrice.End), simplePrice.Price, 1, 1)
        | None -> (None, simplePrice.Price, 1, -1)

    let GetPriceFromPriceTable (priceItem : PriceTypes) = 
        match priceItem with
        | Simple(simplePrice) -> GetPriceFromSimplePrice simplePrice
        | Bulk(simplePrice, min, max) -> 
            match Money.GetPrice simplePrice with
            | Some(money) -> (GetMinPriceDate (simplePrice.Start, simplePrice.End), simplePrice.Price, min, max)
            | None -> (None, simplePrice.Price, min, max)
    
    let CompareRange (min : int) (max : int) (qty : int) =
        if qty >=< (min, max) then
            true
        else
            false

    let CompareDates (d1 : DateTime) (d2 : DateTime) =
        if d1.CompareTo(d2) <= 0 then
            true
        else
            false
    
    let ComparePrices (p1 : ValidDatePrice) (p2 : ValidDatePrice) (qty : int) =
        let odate1, price1, min1, max1 = p1
        let odate2, price2, min2, max2 = p2
        match odate1, odate2, min2, max2 with
        | Some(date1), Some(date2), min, max when date1 < date2 && max > -1 && CompareRange min max qty -> p2
        | Some(date1), Some(date2), min, max when date1 < date2 && max = -1 -> p2
        | None, Some(date2), min, max when max = -1 -> p2
        | None, Some(date2), min, max when CompareRange min max qty -> p2
        | None, None, min, max when max > -1 && CompareRange min max qty -> p2
        | _, _, _, _-> p1

    // Down the line this function will do a lot more then returning Item.Price
    let GetPrice (item : Item, qty : int) =
        match item.Price with
        | One priceItem -> 
            match priceItem with
            | Simple(price) -> Money.GetPrice price
            | _ -> None
        | Many priceList -> 
            let mappedPrices : list<ValidDatePrice> = List.map GetPriceFromPriceTable priceList
            let basePrice = None, 0.0M<Money.dollars>, 1, -1
            let reduced = List.fold (fun a b -> ComparePrices a b qty) basePrice mappedPrices
            match reduced with
            | None, _, _, _ -> None
            | _, money, _, _ -> Some(money)