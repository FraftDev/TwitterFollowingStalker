using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitterFollowingStalker.Models;

namespace TwitterFollowingStalker
{
    class Database
    {
        public static int Total = 0;
        public static int Runthroughs = 0;

        public static HashSet<DB_User_Model> TwitterUsers = new HashSet<DB_User_Model>();
        public static HashSet<DB_Following_Model> TwitterFollowings = new HashSet<DB_Following_Model>();

        public static HashSet<DB_Following_Model> BufferFollowings = new HashSet<DB_Following_Model>();

        public static bool AddFollowing(DB_Following_Model followingModel)
        {
            bool result = TwitterFollowings.Add(followingModel);
            if (!result) 
                return false;

            BufferFollowings.Add(followingModel);
            StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\TwitterFollowings.txt", true);
            sw.WriteLine($"{followingModel.TwitterId}:{followingModel.TwitterFollowingId}:{followingModel.TwitterFollowingUsername}");
            sw.Close();

            Thread.Sleep(100);
            return true;
        }

        public static void LoadUsers()
        {
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\TwitterUsers.txt");
            string line;
            while((line = sr.ReadLine()) != null)
            {
                string[] userSplit = line.Split(':');

                if (userSplit.Length != 2)
                    continue;

                DB_User_Model userModel = new DB_User_Model()
                {
                    TwitterId = long.Parse(userSplit[0]),
                    TwitterUsername = userSplit[1]
                };

                TwitterUsers.Add(userModel);
            }
            sr.Close();
        }

        public static void LoadFollowings()
        {
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\TwitterFollowings.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] userSplit = line.Split(':');

                if (userSplit.Length != 3)
                    continue;

                DB_Following_Model userModel = new DB_Following_Model()
                {
                    TwitterId = long.Parse(userSplit[0]),
                    TwitterFollowingId = long.Parse(userSplit[1]),
                    TwitterFollowingUsername = userSplit[2]
                };

                TwitterFollowings.Add(userModel);
            }
            sr.Close();
        }
    }
}
