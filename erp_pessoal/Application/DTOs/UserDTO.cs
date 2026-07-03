namespace Application.DTOs
{
    public class UserDTO
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
