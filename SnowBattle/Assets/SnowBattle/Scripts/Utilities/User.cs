public class User
{
    public string username;
    public string email;
    public string password;
    public string uID;
    public int level;
    public int won;
    public int lose;
    public float money;
    public int levelUser;

    public User()
    {
    }

    public User(string username, string email, string password,string uID,int level, int won, int lose, float money)
    {
        this.username = username;
        this.email = email;
        this.password = password;
        this.uID = uID;
        this.level = level;
        this.won = won;
        this.lose = lose;
        this.money = money;
        this.levelUser = 0;
    }
}
