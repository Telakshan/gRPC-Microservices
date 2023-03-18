namespace ShoppingCartGrpc.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userName)
    {
        UserName = userName;
    }

    public float TotalPrice => Items.Select(x => x.Price * x.Quantity).Sum();
    
}

