namespace Tourism.Entitiy
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string UserRole { get; set; }
    }
}
