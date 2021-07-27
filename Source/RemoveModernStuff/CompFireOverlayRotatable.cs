using System;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CompFireOverlayRotatable : ThingComp
    {
        // This need to be readonly, otherwise the game will complain about the thread it's being loaded.
        private static readonly GraphicRotatable FireGraphic = new GraphicRotatable(new GraphicRequest(null,
            "Things/Special/Fire",
            ShaderDatabase.TransparentPostLight,
            Vector2.one, Color.white, Color.white,
            null, 0, null, null));

        private ThingDef def;
        private int mem1;
        private int mem2;
        private CompRefuelable refuelableComp;

        private int scramble;

        private ThingWithComps thing;

        static CompFireOverlayRotatable()
        {
        }

        private CompProperties_FireOverlayRotatable Props =>
            (CompProperties_FireOverlayRotatable) props;

        public override void Initialize(CompProperties props)
        {
            this.props = props;
            thing = parent;

            if (thing != null)
            {
                def = thing.def;
            }


            //LoadGraphics();
        }


        /*
        public void LoadGraphics()
        {
            try
            {
                GraphicRequest gr = new GraphicRequest(null, 
                    "Things/Special/Fire", 
                    ShaderDatabase.TransparentPostLight, 
                    Vector2.one, 
                    Color.white, 
                    Color.white, null, 0, null);
                FireGraphic = new GraphicRotatable(gr);
            }
            catch (Exception e)
            {
                Log.Message($"TTA: CompFire caught exception {e}.");
            }
        }
*/
        public override void PostDraw()
        {
            try
            {
                if (parent == null)
                {
                    return;
                }

                if (def == null)
                {
                    return;
                }

                if (!ShouldRender())
                {
                    return;
                }

                Vector3 offset;

                switch (thing.Rotation.AsInt)
                {
                    case 0: // south
                        offset = Props.offset_south;
                        break;
                    case 1: // west
                        offset = Props.offset_west;
                        break;
                    case 2: // north
                        offset = Props.offset_north;
                        break;
                    case 3: // east
                        offset = Props.offset_east;
                        break;
                    default:
                        throw new Exception($"TTA: CompFire found thing {thing} with invalid rotation.");
                }

                var drawPosRotated = thing.DrawPos + offset;
                var drawSizeRotated = new Vector3(Props.fireSize.x, 1f, Props.fireSize.y);
                var quaternion = Quaternion.identity;

                var y = (int) def.altitudeLayer + (Props.aboveThing ? 1 : -1);
                drawPosRotated.y = ((AltitudeLayer) y).AltitudeFor();

                FireFlicker(drawPosRotated, drawSizeRotated, quaternion);
            }
            catch (Exception e)
            {
                Log.Message($"TTA: CompFire caught exception {e}.");
            }
        }

        private void FireFlicker(Vector3 drawPosRotated, Vector3 drawSizeRotated, Quaternion quaternion)
        {
            if (scramble == 0)
            {
                scramble = Random.Range(0, 24735);
            }

            var timeTicks = Find.TickManager.TicksGame;
            var timeTicksScrambled = timeTicks + scramble;
            var interval = timeTicksScrambled / Props.ticks;
            var thisIndex = interval % FireGraphic.SubGraphics.Length;
            var _scramble = Mathf.Abs(thing.thingIDNumber ^ 7419567) / 15;

            if (thisIndex != mem1)
            {
                mem2 = Random.Range(0, FireGraphic.SubGraphics.Length);
                mem1 = thisIndex;
            }

            var radial = GenRadial
                .RadialPattern[_scramble % GenRadial.RadialPattern.Length]
                .ToVector3() / GenRadial.MaxRadialPatternRadius;
            radial *= 0.05f;

            var newPosRotated = drawPosRotated + (radial * drawSizeRotated.x);
            var graphic = FireGraphic.SubGraphics[mem2];
            Matrix4x4 matrix = default;
            matrix.SetTRS(newPosRotated, quaternion, drawSizeRotated);
            Graphics.DrawMesh(MeshPool.plane10, matrix, graphic.MatSingle, 0);
        }

        private bool ShouldRender()
        {
            if (Props.dependency == DependencyType.None)
            {
                return true;
            }

            if (Props.dependency != DependencyType.Fuel)
            {
                return false;
            }

            if (refuelableComp == null)
            {
                return false;
            }

            return refuelableComp.HasFuel;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelableComp = parent.GetComp<CompRefuelable>();
        }
    }
}