namespace PostManagement.Application.Dtos.Role;

public class RoleDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = new List<string>();
}
