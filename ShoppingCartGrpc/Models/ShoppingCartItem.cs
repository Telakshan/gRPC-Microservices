namespace ShoppingCartGrpc.Models;

public class ShoppingCartItem
{
    public int ShoppingCartItemId { get; set; }
    public int Quantity { get; set; } = 0;
    public string? Color { get; set; }
    public float Price { get; set; } = 0;
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}

