namespace PostManagement.Application.Dtos.User
{
    public class UserWithPermissionsDto: UserDto
    {
        public ICollection<string> Permissions { get; set; } = new List<string>();
    }
}
