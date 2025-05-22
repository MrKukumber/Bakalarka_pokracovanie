using System;
using Optepafi.Models.TemplateMan.TemplateAttributes;
using Optepafi.Models.Utils;

namespace Optepafi.Models.TemplateMan.Templates;

//TODO: comment
public class Orienteering_ISOM_2017_2_RunSpeModSym : ITemplate<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>
{
    public static Orienteering_ISOM_2017_2_RunSpeModSym Instance { get; } = new();
    private Orienteering_ISOM_2017_2_RunSpeModSym() {}
    public string TemplateName { get; } = "Orienteering (ISOM 2017-2, running speed modifying symbols only)";

    public TOut AcceptGeneric<TOut, TGenericParam, TConstraint, TOtherParams>(ITemplateGenericVisitor<TOut, TConstraint, TOtherParams> genericVisitor,
        TGenericParam genericParam, TOtherParams otherParams) where TGenericParam : TConstraint
    {
        return genericVisitor.GenericVisit<Orienteering_ISOM_2017_2_RunSpeModSym, VertexAttributes, EdgeAttributes, TGenericParam>(
            this, genericParam, otherParams);
    }
    
    public TOut AcceptGeneric<TOut, TOtherParams>(ITemplateGenericVisitor<TOut, TOtherParams> genericVisitor,
        TOtherParams otherParams)
    {
        return  genericVisitor.GenericVisit<Orienteering_ISOM_2017_2_RunSpeModSym,VertexAttributes, EdgeAttributes>(this, otherParams);
    }

    public TOut AcceptGeneric<TOut>(ITemplateGenericVisitor<TOut> genericVisitor)
    {
        return genericVisitor.GenericVisit<Orienteering_ISOM_2017_2_RunSpeModSym,VertexAttributes,EdgeAttributes>(this);
    }

    public void AcceptGeneric(ITemplateGenericVisitor genericVisitor)
    {
        genericVisitor.GenericVisit<Orienteering_ISOM_2017_2_RunSpeModSym,VertexAttributes,EdgeAttributes>(this);
    }

    
    // 2 bits
    public enum Grounds {BrokenGround_113 = 1, VeryBrokenGround_114, CultivatedLand_412}
    // 2 bits
    public enum Boulders {GiganticBoulder_206 = 1, BoulderField_208, DenseBoulderField_209}
    // 2 bits
    public enum Stones {StonyGroundSlow_210 = 1, StonyGroundWalk_211, StonyGroundFight_212}
    // 3 bits
    public enum Water {UncrossableBodyOfWater_301 = 1, ShallowBodyOfWater_302, UncrossableMarsh_307, Marsh_308, IndistinctMarsh_310}
    // 5 bits
    public enum VegetationAndManMade {SandyGround_213 = 1, BareRock_214, OpenLand_401, OpenLandWithTrees_402, RoughOpenLand_403, RoughOpenLandWithTrees_404, Forest_405, VegetationSlow_406, VegetationSlowNormalOneDir_406_1, VegetationWalk_408, VegetationWalkNormalOneDir_408_1, VegetationWalkSlowOneDir_408_2, VegetationFight_410, VegetationFightNormalOneDir_410_1, VegetationFightSlowOneDir_410_2, VegetationFightWalkOneDir_410_3, Orchard_413, RoughOrchard_413, Vineyard_414, RoughVineyard_414, PavedArea_501, AreaThatShallNotBeEntered_520, Building_521}
    // 2 bits
    public enum VegetationGoodVis {VegetationSlowGoodVis_407 = 1, VegetationWalkGoodVis_409}
    
    // 3 bits
    public enum NaturalLinearObstacles {EarthBank_104 = 1, EarthWall_105, ErosionGully_107, ImpassableCliff_201, Cliff_202, CrossableWatercourse_304, SmallCrossableWatercourse_305}
    // 4 bits
    public enum Paths {WideRoad_502 = 1, Road_503, VehicleTrack_504, Footpath_505, SmallFootpath_506, LessDistinctSmallFootpath_507, NarrowRide_508, Stairway_532}
    // 3 bits
    public enum ManMadeLinearObstacles {Trench_215 = 1, Wall_513, ImpassableWall_515, Fence_516, ImpassableFence_518, ProminentLineFeature_528, ImpassableProminentLineFeature_529}

    public readonly struct Surroundings((Grounds? ground, Boulders? boulders, Stones? stones, Water? water, VegetationAndManMade? vegetationAndManMade, VegetationGoodVis? vegetationGoodVis) attributes, float vegetationAndManMadeRotation) : IEquatable<Surroundings>
    {
        public static bool operator ==(Surroundings left, Surroundings right) => left._surroundings == right._surroundings && left._vegetationAndManMadeRotation == right._vegetationAndManMadeRotation; 
        public static bool operator!=(Surroundings left, Surroundings right) => !(left == right);
        public override bool Equals(object? other) => other is Surroundings otherSurroundings ? this == otherSurroundings : false;
        public override int GetHashCode() => (_surroundings, _vegetationAndManMadeRotation).GetHashCode();
        public bool Equals(Surroundings other) => this == other;

        
        private readonly ushort _surroundings = (ushort)(((((((((((int)(attributes.ground ?? 0) << 2) + 
                                                                 (int)(attributes.boulders ?? 0)) << 2) + 
                                                                (int)(attributes.stones ?? 0)) << 3) + 
                                                               (int)(attributes.water ?? 0)) << 5) + 
                                                              (int)(attributes.vegetationAndManMade ?? 0)) << 2) + 
                                                             (int)(attributes.vegetationGoodVis ?? 0));
        
        private readonly short _vegetationAndManMadeRotation = (short)(vegetationAndManMadeRotation * 360 / (2 * Math.PI));
         
        public VegetationGoodVis? VegetationGoodVis => (_surroundings & 0b11) != 0 ? (VegetationGoodVis)(_surroundings & 0b11) : null;
        public VegetationAndManMade? VegetationAndManMade => ((_surroundings >> 2) & 0b11111) != 0 ? (VegetationAndManMade)((_surroundings >> 2) & 0b11111) : null;
        public Water? Water => ((_surroundings >> 7) & 0b111) != 0 ? (Water)((_surroundings >> 7) & 0b111) : null;
        public Stones? Stones => ((_surroundings >> 10) & 0b11) != 0 ? (Stones)((_surroundings >> 10) & 0b11) : null;
        public Boulders? Boulders => ((_surroundings >> 12) & 0b11) != 0 ? (Boulders)((_surroundings >> 12) & 0b11) : null;
        public Grounds? Ground =>  ((_surroundings >> 14) & 0b11) != 0 ? (Grounds)((_surroundings >> 14) & 0b11) : null;

        public float VegetationAndManMadeRotation => (float) (_vegetationAndManMadeRotation * 2 * Math.PI / 360); 
    }

    public readonly struct LinearFeatures(NaturalLinearObstacles? naturalLinearObstacle, Paths? path, ManMadeLinearObstacles? manMadeLinearObstacle) : IEquatable<LinearFeatures>
    {
        public static bool operator ==(LinearFeatures left, LinearFeatures right) => left._linearFeatures == right._linearFeatures; 
        public static bool operator!=(LinearFeatures left, LinearFeatures right) => !(left == right);
        public override bool Equals(object? other) => other is LinearFeatures otherLinearFeatures ? this == otherLinearFeatures : false;
        public override int GetHashCode() => _linearFeatures.GetHashCode();
        public bool Equals(LinearFeatures other) => this == other;

        
        private readonly ushort _linearFeatures = (ushort)(((((int)(naturalLinearObstacle ?? 0) << 4) + 
                                                           (int)(path ?? 0)) << 3) + 
                                                         (int)(manMadeLinearObstacle ?? 0));
        
        public ManMadeLinearObstacles? ManMadeLinearObstacle =>  (_linearFeatures & 0b111) != 0 ? (ManMadeLinearObstacles?)(_linearFeatures & 0b111) : null;
        public Paths? Path => ((_linearFeatures >> 3) & 0b1111) != 0 ? (Paths?)((_linearFeatures >> 3) & 0b1111) : null;
        public NaturalLinearObstacles? NaturalLinearObstacle => ((_linearFeatures >> 7) & 0b111) != 0 ? (NaturalLinearObstacles?)((_linearFeatures >> 7) & 0b111) : null;
    }

    public struct VertexAttributes(MapCoordinates position, double elevation = 0) : IVertexAttributes
    {
        public readonly MapCoordinates Position = position;
        public readonly double Elevation = elevation;
    }

    public struct EdgeAttributes(Surroundings leftSurroundings, LinearFeatures linearFeatures, Surroundings rightSurroundings) : IEdgeAttributes, IEquatable<EdgeAttributes>
    {
        public static bool operator==(EdgeAttributes left, EdgeAttributes right) => left.LeftSurroundings == right.LeftSurroundings && left.RightSurroundings == right.RightSurroundings && left.LineFeatures == right.LineFeatures;
        public static bool operator!=(EdgeAttributes left, EdgeAttributes right) => !(left == right);
        public override bool Equals(object? other) => other is EdgeAttributes otherEdgeAttribute ? this == otherEdgeAttribute : false;
        public override int GetHashCode() => (leftSurroundings, rightSurroundings, linearFeatures).GetHashCode();
        public bool Equals(EdgeAttributes other) => this == other;

        public EdgeAttributes(Surroundings bothSurroundings, LinearFeatures linearFeatures) : this(bothSurroundings, linearFeatures, bothSurroundings){}
        public EdgeAttributes(Surroundings bothSurroundings) : this(bothSurroundings, new LinearFeatures(null, null, null)){}
        public EdgeAttributes(Surroundings leftSurrounding, Surroundings rightSurroundings) : this(leftSurrounding, new(null, null, null), rightSurroundings){}
        public EdgeAttributes():this(new ((null, null, null, null, null, null), 0), new LinearFeatures(null, null, null)){}
        
        public Surroundings LeftSurroundings => leftSurroundings;
        public Surroundings RightSurroundings => rightSurroundings;
        public LinearFeatures LineFeatures => linearFeatures;
    }
}