using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace ParkAssistGent.Classes
{
    public class PushPin
    {
        public string Name { get; set; }
        public Geopoint Location { get; set; }

    }
}
