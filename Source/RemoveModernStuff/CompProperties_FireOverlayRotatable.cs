using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompProperties_FireOverlayRotatable : CompProperties
    {
        public readonly bool aboveThing = true;
        public readonly DependencyType dependency = DependencyType.None;

        public Vector2 fireSize = new Vector2(1f, 1f);
        public Vector3 offset_east;
        public Vector3 offset_north;
        public Vector3 offset_south;
        public Vector3 offset_west;
        public string path = null;
        public int ticks = 60;

        public CompProperties_FireOverlayRotatable()
        {
            compClass = typeof(CompFireOverlayRotatable);
        }
    }
}