namespace Shabb.Store

module Cart = 
    type CartItem = Product.Item * int

    [<NoComparison>]
    type Basket = {
        Id       : int
        Items    : list<CartItem>
        Subtotal : decimal<Money.dollars>
        Tax      : decimal<Money.dollars>
        Discount : decimal<Money.dollars>
        Total    : decimal<Money.dollars>
    }

    let calcItemSubtotal (item : CartItem) = 
        let price = Product.GetPrice item
        match price with
        | Some(price) -> price * (decimal <| snd item)
        | None -> 0.00M<Money.dollars>

    let calcSubtotal (cart : Basket) = cart.Items |> List.sumBy calcItemSubtotal

