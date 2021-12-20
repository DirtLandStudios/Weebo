using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weebo_lib
{
    public class Anime
    {
        record node
        {
            string id;
            string title;
            record main_picture
            {
                string small;
                string medium;
                string large;
            }
        }
        record list_status
        {
            string status;
            int score;
            int num_watched_episodes;
            bool is_rewatching;
            //updated_at : "updated_at": "2017-11-11T19:52:16+00:00"

        }
    }
    
}
