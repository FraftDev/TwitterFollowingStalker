using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFollowingStalker.Models
{
    class DB_User_Model
    {
        public long TwitterId { get; set; }
        public string TwitterUsername { get; set; }

        public override int GetHashCode()
        {
            return TwitterId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DB_User_Model model = (DB_User_Model)obj;
            return TwitterId == model.TwitterId;
        }
    }
}
