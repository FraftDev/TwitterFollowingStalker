using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFollowingStalker.Models
{
    class DB_Following_Model
    {
        public long TwitterId { get; set; }
        public long TwitterFollowingId { get; set; }
        public string TwitterFollowingUsername { get; set; }

        public override int GetHashCode()
        {
            return TwitterId.GetHashCode() ^ TwitterFollowingId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DB_Following_Model model = (DB_Following_Model)obj;
            return TwitterId == model.TwitterId && TwitterFollowingId == model.TwitterFollowingId;
        }
    }
}
