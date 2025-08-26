namespace OrderFlow.Web.Helpers.Concrete
{
    public class SidebarMenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public List<string> RequiredRoles { get; set; } = new();
    }

    public static class SidebarMenuConfig
    {
        public static List<SidebarMenuItem> GetMenuItems() => new()
        {
            new SidebarMenuItem {
                Title = "Dashboard",
                IconClass = "bi bi-house",
                Controller = "Home",
                Action = "Index",
                RequiredRoles = new List<string> { "Admin", "Customer", "Courier" }
            },
            new SidebarMenuItem {
                Title = "Orders",
                IconClass = "bi bi-bag-check-fill",
                Controller = "Orders",
                Action = "List",
                RequiredRoles = new List<string> { "Admin", "Customer" }
            },
            new SidebarMenuItem {
                Title = "Create Order",
                IconClass = "bi bi-cart4",
                Controller = "Orders",
                Action = "Create",
                RequiredRoles = new List<string> { "Admin", "Customer" }
            },
            new SidebarMenuItem {
                Title = "Track Order",
                IconClass = "bi bi-box-seam",
                Controller = "Orders",
                Action = "Track",
                RequiredRoles = new List<string> { "Admin", "Customer" }
            },
            new SidebarMenuItem {
                Title = "My Deliveries",
                IconClass = "bi bi-truck",
                Controller = "Orders",
                Action = "MyDeliveries",
                RequiredRoles = new List<string> { "Admin", "Courier" }
            }
        };
    }
}
