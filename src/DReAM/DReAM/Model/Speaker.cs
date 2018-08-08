using System;
using System.Collections.Generic;
using System.Text;

namespace DReAM.Model
{
    public class Speaker
    {
        public string SpeakerName { get; set; }
        public string Topic { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }

        public List<Speaker> GetSpeakers()
        {
            List<Speaker> speakers = new List<Speaker>()
            {
                new Speaker() { SpeakerName="AndreiM", Topic="Development", ImageUrl="https://thumbs.dreamstime.com/b/teal-small-polka-dot-pattern-repeat-background-seamless-repeats-40992377.jpg", ShortDescription="This is some placeholder"},
                new Speaker() { SpeakerName="Don", Topic="Development", ImageUrl="https://d2gg9evh47fn9z.cloudfront.net/800px_COLOURBOX11109564.jpg", ShortDescription="This is some placeholder"}
            };
            return speakers;
        }
    }
}
