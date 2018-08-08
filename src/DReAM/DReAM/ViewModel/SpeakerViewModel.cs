using DReAM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DReAM.ViewModel
{
    public class SpeakerViewModel
    {
        public List<Speaker> Speakers { get; set; }

        public SpeakerViewModel()
        {
            Speakers = new Speaker().GetSpeakers();
        }
    }
}
