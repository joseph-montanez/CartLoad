namespace Shabb.Store

open System
open System.Text.RegularExpressions

module Option =

    type Item = {
        Id : uint32
        Name : string
        PriceEffect : Product.PriceEffectTypes
        Price : Product.PriceTable
        SkuDelimiter : string option
        SkuEffect : Product.SkuEffectTypes
        Sku : string
        Order : sbyte
    }

    type ItemSetDetail = {
        Id : uint32
        ItemId : uint32
        Name : string
        SkuDelimiter : string option
        SkuEffect : Product.SkuEffectTypes
        Required : bool
        Order : sbyte
    }

    type ItemSet = {
        Set : ItemSetDetail
        Options : Item list
    }

    type ItemChoice = {
        Set : ItemSetDetail
        Option: Item
    }

    type Combination = {
        Id : uint32
        ItemId : uint32
        Sku : string
        Enabled : bool
        Options : ItemChoice list
        Order : sbyte
    }

    let (|BreakStr|_|) (s : string) = 
        match s.Split(',') with
        | [|b|] -> Some (b, "")
        | [|b;c|] -> Some (b, c)
        | _ -> None

    let JoinSku (delimiter : string) (acc : Product.SkuCombination) (elem : ItemChoice) = 
        match elem.Option.SkuEffect with
        | Product.SkuEffectTypes.ReplaceAll -> {acc with Replacements = List.append acc.Replacements [(elem.Option.Sku, delimiter)]}
        | Product.SkuEffectTypes.StartOf -> {acc with StartOfs = List.append acc.StartOfs [(elem.Option.Sku, delimiter)]}
        | Product.SkuEffectTypes.EndOf -> {acc with EndOfs = List.append acc.EndOfs [(elem.Option.Sku, delimiter)]}
        | Product.SkuEffectTypes.Before -> {acc with Stack = List.append [(elem.Option.Sku, delimiter)] acc.Stack}
        | Product.SkuEffectTypes.After -> {acc with Stack = List.append acc.Stack [(elem.Option.Sku, delimiter)]}

    let AddSkuStack (acc : string) (elem : (string * string)) = 
        match acc with
        | "" -> fst elem
        | _ -> acc + snd elem + fst elem

    let RenderSkuStack (skus : (string * string) list) = 
        List.fold AddSkuStack "" skus

    let BuildSku (delimiter : string) (sku : string) (choices : ItemChoice list) = 
        let stack : string * string = sku, delimiter
        let stackList = [stack]
        let combinator : Product.SkuCombination = {
            Stack = stackList
            StartOfs = []
            EndOfs = []
            Replacements = []
        } 
        let newCombinator : Product.SkuCombination = List.fold (JoinSku delimiter) combinator choices
        let skuCombined = List.append newCombinator.StartOfs newCombinator.Stack
        let skuCombined2 = List.append skuCombined newCombinator.EndOfs
        // TODO: replacements
        skuCombined2 |> RenderSkuStack
    let BuildSkuDefault = BuildSku "-"
