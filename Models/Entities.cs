using System;
using System.Collections.Generic;

namespace SmartAutoCashSystem.Models;

public class Role
{
    public int RoleID { get; set; }
    public string RoleName { get; set; } = string.Empty;
}

public class User
{
    public int UserID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int RoleID { get; set; }
    public DateTime CreatedAt { get; set; }
    public Role? Role { get; set; }
}

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int? CategoryID { get; set; }
    public bool IsActive { get; set; }
    public Category? Category { get; set; }
}

public class Order
{
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "Created";
    public int CreatedByUserID { get; set; }
    public User? CreatedBy { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public Payment? Payment { get; set; }
}

public class OrderItem
{
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public Product? Product { get; set; }
}

public class Payment
{
    public int PaymentID { get; set; }
    public int OrderID { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Pending";
}

