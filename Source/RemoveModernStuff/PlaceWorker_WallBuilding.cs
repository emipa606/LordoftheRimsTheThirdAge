using System;
using Verse;

namespace Rimworld
{
    public class PlaceWorker_WallBuilding : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef,
            IntVec3 loc,
            Rot4 rot,
            Map map,
            Thing thingToIgnore = null,
            Thing thing = null)
        {
            var building = loc.GetEdifice(map);

            if (building?.def?.graphicData == null)
            {
                return "Must be placed over walls.";
            }

            if ((building.def.graphicData.linkFlags & (LinkFlags.Wall | LinkFlags.Rock)) == 0)
            {
                return "Must be placed over walls.";
            }

            var facingLoc = loc;
            // IntVec3 facingLoc = loc + rot.FacingCell;       might do the whole work.

            switch (rot.AsInt)
            {
                case 0: // south
                    --facingLoc.z;
                    break;
                case 1: // west
                    --facingLoc.x;
                    break;
                case 2: // north
                    ++facingLoc.z;
                    break;
                case 3: // east
                    ++facingLoc.x;
                    break;
                default:
                    throw new Exception("LotRD: PlaceWorker_BuildingWall " +
                                        "found an invalid rotation for placed thing at {loc}.");
            }

            var _support = facingLoc.GetEdifice(map);
            if (_support != null && building.def?.graphicData != null)
            {
                return "Adjacent facing cell must be clear.";
            }

            return AcceptanceReport.WasAccepted;
        }
    }
}