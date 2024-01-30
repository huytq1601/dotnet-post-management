namespace PostManagement.Application.Dtos.User
{
    public class AssignResultDto
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
