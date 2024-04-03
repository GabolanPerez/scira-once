using System.Collections.Generic;

namespace SCIRA.Utilidades
{
    public static class Globals
    {
        public static string ConnString { get; set; }
        public static string SystemKey { get; set; }
        public static Dictionary<string, string> UserLangs { get; set; }
        public static bool StartOnActiveDIrectory { get; set; }

        public static string GetLan(int id)
        {
            if (UserLangs != null)
            {
                if (UserLangs.ContainsKey("lan" + id))
                {
                    return UserLangs["lan" + id];
                }
                else
                {
                    UserLangs.Add("lan" + id, "es");
                    return "es";
                }
            }
            else
            {
                UserLangs = new Dictionary<string, string>();
                UserLangs.Add("lan" + id, "es");
                return "es";
            }
        }

        public static void SetLan(int id, string lan)
        {
            if (UserLangs != null)
            {
                if (UserLangs.ContainsKey("lan" + id))
                {
                    UserLangs["lan" + id] = lan;
                }
                else
                {
                    UserLangs.Add("lan" + id, lan);
                }
            }
            else
            {
                UserLangs = new Dictionary<string, string>();
                UserLangs.Add("lan" + id, lan);
            }
        }
    }
}