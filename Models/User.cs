using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public enum Access { Anonymous, View, Write, Admin }

    public class User : Record
    {
        public User()
        {
            Id = 0;
            Blocked = false;
            Access = Access.View;
            Online = false;
            Verified = false;
            Notify = true;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Access Access { get; set; }
        public bool Blocked { get; set; }
        public bool Verified { get; set; }
        public bool Notify { get; set; }

        const string Avatars_Folder = @"/App_Assets/Users/";
        const string Default_Avatar = @"no_avatar.png";

        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;

        [JsonIgnore]
        public bool Online
        {
            get
            {
                return GetOnlineUser().IndexOf(Id) > -1;
            }
            set
            {
                if (value)
                {
                    if (GetOnlineUser().IndexOf(Id) == -1)
                        GetOnlineUser().Add(Id);
                }
                else
                {
                    GetOnlineUser().Remove(Id);
                }
            }
        }

        [JsonIgnore]
        public bool IsAdmin { get { return Access == Access.Admin; } }

        [JsonIgnore]
        public bool CanWrite { get { return Access >= Access.Write; } }

        [JsonIgnore]
        public bool IsBlocked { get { return Blocked; } }

        [JsonIgnore]
        public bool IsOnline { get { return Online; } }

        [JsonIgnore]
        public List<Login> Logins
        {
            get { return DB.Logins.ToList().Where(l => l.UserId == Id).ToList(); }
        }

        public void DeleteLogins()
        {
            foreach (Login login in Logins.Copy())
            {
                DB.Logins.Delete(login.Id);
            }
        }

        private static List<int> GetOnlineUser()
        {
            if (HttpRuntime.Cache["onlineUsers"] == null)
                HttpRuntime.Cache["onlineUsers"] = new List<int>();

            return (List<int>)HttpRuntime.Cache["onlineUsers"];
        }

        public static User ConnectedUser
        {
            get
            {
                if (HttpContext.Current?.Session["ConnectedUser"] != null)
                {
                    if (DB.Users.IsMarkedChanged)
                    {
                        User connectedUser = (User)HttpContext.Current.Session["ConnectedUser"];

                        if (connectedUser != null)
                            connectedUser = DB.Users.Get(connectedUser.Id);

                        HttpContext.Current.Session["ConnectedUser"] = connectedUser;
                    }

                    User user = (User)HttpContext.Current.Session["ConnectedUser"];

                    if (user != null)
                        return user.Copy();
                }

                return null;
            }
            set
            {
                HttpContext.Current.Session["ConnectedUser"] = value;
            }
        }
    }
}