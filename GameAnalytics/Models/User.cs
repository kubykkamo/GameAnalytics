namespace GameAnalytics.Models
{
    public class User
    {

        public int Id { get; set; }

        public string Username { get; set; }

        public string DiscordId { get; set; }


        public User(string username, string discordId = null)
        {
            
            Username = username;
            DiscordId = discordId;
        }



    }
}
