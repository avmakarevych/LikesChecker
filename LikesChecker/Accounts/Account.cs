namespace LikesChecker.Accounts;

public class Account
{
    public string Nickname { get; set; }
    public string Foldername { get; set; }
    public string Link { get; set; }
    public string Likes { get; set; }
    public string Messages { get; set; }
    public string LikesWeek { get; set; }
    public DateTime LastSimp { get; set; }
    public bool Error { get; set; }

    public Account(string nickname, string foldername, string link, string likes, string messages, string likesWeek, DateTime lastSimp)
    {
        Nickname = nickname;
        Foldername = foldername;
        Link = link;
        Likes = likes;
        Messages = messages;
        LikesWeek = likesWeek;
        LastSimp = lastSimp;
        Error = false;
    }
}