namespace PostManagement.Core.Common;

public class Permissions
{
    public static class Users
    {
        public const string CanRead = "Users.CanRead";
        public const string CanWrite = "Users.CanWrite";
        public const string CanDelete = "Users.CanDelete";
    }

    public static class Posts
    {
        public const string CanRead = "Posts.CanRead";
        public const string CanWrite = "Posts.CanWrite";
        public const string CanDelete = "Posts.CanDelete";
    }

    public static class Tags
    {
        public const string CanRead = "Tags.CanRead";
        public const string CanWrite = "Tags.CanWrite";
        public const string CanDelete = "Tags.CanDelete";
    }

    public static class Roles
    {
        public const string CanRead = "Roles.CanRead";
        public const string CanWrite = "Roles.CanWrite";
        public const string CanDelete = "Roles.CanDelete";
    }

    public static IEnumerable<string> GetAllPermissions()
    {
        var permissions = new List<string>();

        foreach (var nestedClass in typeof(Permissions).GetNestedTypes())
        {
            foreach (var field in nestedClass.GetFields())
            {
                var fieldValue = field.GetValue(null);
                if (fieldValue != null)
                {
                    permissions.Add((string)fieldValue);
                }
            }
        }

        return permissions;
    }

    public static IEnumerable<string> GetPermissionsByClass(string className)
    {
        var permissions = new List<string>();

        var nestedClass = typeof(Permissions).GetNestedType(className);
        if (nestedClass != null)
        {
            permissions.AddRange(nestedClass.GetFields()
                .Select(field => (string?)field.GetValue(null))
                .Where(permission => permission != null)
                .Select(permission => permission!));
        }

        return permissions;
    }
}
