using System.Reflection;
using OrderFlow.Web.Middlewares;

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
        private static List<SidebarMenuItem>? _cachedItems;
        private static readonly object _lock = new();

        public static void Initialize()
        {
            if (_cachedItems != null) return;
            lock (_lock)
            {
                if (_cachedItems == null)
                {
                    _cachedItems = BuildMenuItems();
                }
            }
        }

        public static List<SidebarMenuItem> GetMenuItems()
        {
            if (_cachedItems == null)
            {
                Initialize();
            }
            return _cachedItems!;
        }

        private static List<SidebarMenuItem> BuildMenuItems()
        {
            var items = new List<SidebarMenuItem>
            {
                new SidebarMenuItem {
                    Title = "Dashboard",
                    IconClass = "bi bi-house",
                    Controller = "Home",
                    Action = "Index",
                },
                new SidebarMenuItem {
                    Title = "Orders",
                    IconClass = "bi bi-bag-check-fill",
                    Controller = "Orders",
                    Action = "List",
                },
                new SidebarMenuItem {
                    Title = "Create Order",
                    IconClass = "bi bi-cart4",
                    Controller = "Orders",
                    Action = "Create",
                },
                new SidebarMenuItem {
                    Title = "Track Order",
                    IconClass = "bi bi-box-seam",
                    Controller = "Orders",
                    Action = "Track",
                },
                new SidebarMenuItem {
                    Title = "My Deliveries",
                    IconClass = "bi bi-truck",
                    Controller = "Orders",
                    Action = "MyDeliveries",
                }
            };

            foreach (var item in items)
            {
                item.RequiredRoles = ResolveRolesForAction(item.Controller, item.Action);
            }

            return items;
        }

        private static List<string> ResolveRolesForAction(string controllerName, string actionName)
        {
            // Find controller type in current assembly by convention: {ControllerName}Controller
            var assembly = Assembly.GetExecutingAssembly();
            var targetControllerType = assembly
                .GetTypes()
                .FirstOrDefault(t => string.Equals(t.Name, controllerName + "Controller", StringComparison.OrdinalIgnoreCase));

            if (targetControllerType == null)
            {
                return new List<string>();
            }

            // Start with controller-level roles
            var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var controllerRoleAttrs = targetControllerType
                .GetCustomAttributes(typeof(RoleAuthorizeAttribute), inherit: true)
                .Cast<RoleAuthorizeAttribute>()
                .ToList();

            foreach (var attr in controllerRoleAttrs)
            {
                foreach (var role in GetRolesFromAttribute(attr))
                {
                    roles.Add(role);
                }
            }

            // Then action-level roles, which should override (intersect) if specified
            var method = targetControllerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => string.Equals(m.Name, actionName, StringComparison.OrdinalIgnoreCase));

            if (method != null)
            {
                var actionRoleAttrs = method
                    .GetCustomAttributes(typeof(RoleAuthorizeAttribute), inherit: true)
                    .Cast<RoleAuthorizeAttribute>()
                    .ToList();

                if (actionRoleAttrs.Count > 0)
                {
                    // If action has role attributes, use the union across them
                    roles.Clear();
                    foreach (var attr in actionRoleAttrs)
                    {
                        foreach (var role in GetRolesFromAttribute(attr))
                        {
                            roles.Add(role);
                        }
                    }
                }
            }

            return roles.ToList();
        }

        private static IEnumerable<string> GetRolesFromAttribute(RoleAuthorizeAttribute attribute)
        {
            // Read private field via reflection since roles are stored internally
            var field = typeof(RoleAuthorizeAttribute).GetField("_allowedRoles", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field?.GetValue(attribute) is HashSet<string> set)
            {
                return set;
            }
            return Array.Empty<string>();
        }
    }
}
