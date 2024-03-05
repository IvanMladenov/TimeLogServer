using TimeLogs.Interfaces;

namespace TimeLogs.Implementations
{
    public class User 
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required double TimeLogged { get; set; }
        public string Email { get; set; }
    }
}
