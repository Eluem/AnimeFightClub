using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnimeFightClub001
{
    class TrapSpikeHazard : HazardObj
    {
        private long placedBy;
        public TrapSpikeHazard(PlayerObj placedBy)
            : base(placedBy.ObjectID, "Player", 15, -1, 8) //Should be allowed to be placed by world??
        {
            
        }
    }
}
