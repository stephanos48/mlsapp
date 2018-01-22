using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mlsapp.Models
{
    public class WorldUser : IdentityUser
    {

        public DateTime FirstTrip { get; set; }

    }
}
