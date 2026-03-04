namespace StudentsApi.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public double GPA { get; set; }

        // auth

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Role { get; set; }
    }
}
