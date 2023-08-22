namespace ShoppingCartGrpc.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public virtual List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    public double TotalPrice => ShoppingCartItems.Select(x => x.Price * x.Quantity).Sum();

    /*    public ShoppingCart()
        {
        }*/

    /*    public ShoppingCart(string userName)
        {
            UserName = userName;
        }*/

}

