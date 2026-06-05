namespace API_Food_App.DTOs
{
    public class TestNotificationRequestDto
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
    }
}