using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using Optepafi.Models.GraphicsMan.Objects.Map;
using Optepafi.Models.TemplateMan.Templates;
using Optepafi.Models.UserModelMan.UserModels.Functionalities;
using Optepafi.Models.UserModelMan.Utils;
using Optepafi.Models.Utils;
using Splat;

namespace Optepafi.Models.UserModelMan.UserModels.Specific;

//TODO: comment
public class Orienteering_ISOM_2017_2RunSpeModSymUserModel : 
    IWeightComputing<Orienteering_ISOM_2017_2_RunSpeModSym, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>,
    IAStarHeuristicsComputing<Orienteering_ISOM_2017_2_RunSpeModSym, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>,
    IPositionRetrieving<Orienteering_ISOM_2017_2_RunSpeModSym, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>
{
    [JsonIgnore]
    public Orienteering_ISOM_2017_2_RunSpeModSym AssociatedTemplate => Orienteering_ISOM_2017_2_RunSpeModSym.Instance;
    [JsonIgnore]
    public string FilePath { get; set; } = "";

    public Orienteering_ISOM_2017_2RunSpeModSymUserModel() { }
    public Orienteering_ISOM_2017_2RunSpeModSymUserModel(bool newUserModel)
    {
        if(newUserModel)
        {
            BrokenGroundCoefficient = new(0.95f);
            VeryBrokenGroundCoefficient = new(0.8f);
            CultivatedLandCoefficient = new(0.8f);
            GiganticBoulderCoefficient = new(0);
            BoulderFieldCoefficient = new(0.7f);
            DenseBoulderFieldCoefficient = new(0.55f);
            StonyGroundSlowCoefficient = new(0.65f);
            StonyGroundWalkCoefficient = new(0.5f);
            StonyGroundFightCoefficient = new(0.35f);
            SandyGroundCoefficient = new(0.7f);
            BareRockCoefficient = new(0.95f);
            UncrossableBodyOfWaterCoefficient =new (0);
            ShallowBodyOfWaterCoefficient = new(0.5f);
            UncrossableMarshCoefficient = new(0);
            MarshCoefficient = new(0.8f);
            IndistinctMarshCoefficient = new(0.9f);
            OpenLandCoefficient = new(0.97f);
            OpenLandWithTreesCoefficient = new(0.93f);
            RoughOpenLandCoefficient = new(0.77f);
            RoughOpenLandWithTreesCoefficient = new(0.7f);
            ForestCoefficient = new(0.9f);
            VegetationSlowCoefficient = new(0.7f);
            VegetationWalkCoefficient = new(0.45f);
            VegetationFightCoefficient = new(0.2f);
            OrchardCoefficient = new(0.93f);
            RoughOrchardCoefficient = new(0.8f);
            VineyardCrossingCoefficient = new(0.3f); 
            PavedAreaCoefficient = new (1);
            AreaThatShallNotBeEnteredCoefficient = new (0);
            BuildingCoefficient = new(0);
            VegetationSlowGoodVisibilityCoefficient = new(0.8f);
            VegetationWalkGoodVisibilityCoefficient = new(0.6f);
            EarthBankOvercomeCoefficient = new(2);
            EarthWallOvercomeCoefficient = new(2);
            ErosionGullyOvercomeCoefficient = new(5);
            ImpassableCliffOvercomeCoefficient = new (1000_000);
            CliffOvercomeCoefficient = new(7);
            CrossableWaterCourseOvercomeCoefficient = new(3);
            SmallCrossableWaterCourseOvercomeCoefficient = new(2);
            WideRoadCoefficient = new(1);
            RoadCoefficient = new(1);
            VehicleTrackCoefficient = new(1);
            FootpathCoefficient = new (0.99f);
            SmallFootpathCoefficient = new (0.98f);
            LessDistinctSmallFootpathCoefficient = new (1.07f);
            NarrowRideCoefficient = new(1.07f);
            StairwayCoefficient = new(0.93f);
            TrenchOvercomeCoefficient = new(1);
            WallOvercomeCoefficient = new(2);
            ImpassableWallOvercomeCoefficient = new (1000_000);
            FenceOvercomeCoefficient = new(2);
            ImpassableFenceOvercomeCoefficient = new (1000_000);
            ProminentLineFeatureOvercomeCoefficient = new (1.5f);
            ImpassableProminentLineFeatureOvercomeCoefficient = new (1000_000);
        }
    }
    
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals});
    }

    public void SerializeTo(Stream stream)
    {
        JsonSerializer.Serialize(stream, this);
    }

    private const float coefficientLowerBound = 0.15f;
    // private const float topPaceInSecondsPerCentimeter = 0.0024f;
    public float ComputeWeight(Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes from, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes through, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes to, int scale = 1)
    {
        var surroundingsCoef = ComputeCoefficientOfSurroundings(through.LeftSurroundings, to.Position - from.Position);
        var secondSurroundingsCoef = ComputeCoefficientOfSurroundings(through.RightSurroundings, to.Position - from.Position);
        var moreEfficientSurroundingsCoef = Math.Max(surroundingsCoef, secondSurroundingsCoef);
        if (moreEfficientSurroundingsCoef < 0.000000001) return float.PositiveInfinity; 
        
        var moreEfficientSurroundingsWithPathCoef = ApplyPathToSurroundingsCoef(through.LineFeatures, moreEfficientSurroundingsCoef);
        if (moreEfficientSurroundingsWithPathCoef < coefficientLowerBound) moreEfficientSurroundingsWithPathCoef = coefficientLowerBound;

        var pace = ComputePaceForElevatedPositions(from, to, scale);
        // var pace = 0.24;

        var timeInMoreEfficientSurroundingsWithPath = (float)(pace / moreEfficientSurroundingsWithPathCoef * (to.Position - from.Position).Length() * scale / 1_000_000);
        return AddTimeForLinearObstacles(through.LineFeatures, timeInMoreEfficientSurroundingsWithPath);
    }

    private float ComputeCoefficientOfSurroundings(((Orienteering_ISOM_2017_2_RunSpeModSym.Grounds?, Orienteering_ISOM_2017_2_RunSpeModSym.Boulders?, Orienteering_ISOM_2017_2_RunSpeModSym.Stones?, Orienteering_ISOM_2017_2_RunSpeModSym.Water?, Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade?, Orienteering_ISOM_2017_2_RunSpeModSym.VegetationGoodVis?), float) surroundings, MapCoordinates fromToVector)
    {
        //TODO: zakomponvat rotation if shape of vegetation is dependent of it
        var ((ground, boulders, stones, water, vegetationAndManMade, vegetationGoodVis), vegetationAndManMadeRotation) = surroundings;
        return (ground is not null ? GroundCoefficients[ground.Value]: 1) *
               (boulders is not null ? BoulderCoefficients[boulders.Value]: 1) *
               (stones is not null ? StoneCoefficients[stones.Value]: 1) *
               (water is not null ? WaterCoefficients[water.Value]: 1) *
               (vegetationAndManMade is not null ? VegetationAndManMadeCoefficients[vegetationAndManMade.Value]((vegetationAndManMadeRotation, fromToVector)): 1) *
               (vegetationGoodVis is not null ? VegetationGoodVisCoefficients[vegetationGoodVis.Value]: 1);
    }

    private float ApplyPathToSurroundingsCoef((Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles?, Orienteering_ISOM_2017_2_RunSpeModSym.Paths?, Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles?) linearFeatures, float moreEfficientSurroundingsCoef)
    {
        var (_, path, _) = linearFeatures; 
        if (path is null) return moreEfficientSurroundingsCoef;
        if (path is Orienteering_ISOM_2017_2_RunSpeModSym.Paths.LessDistinctSmallFootpath_507 || path is Orienteering_ISOM_2017_2_RunSpeModSym.Paths.NarrowRide_508)
            return moreEfficientSurroundingsCoef * PathsCoefficients[path.Value];
        return PathsCoefficients[path.Value];
    }

    private float AddTimeForLinearObstacles((Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles?, Orienteering_ISOM_2017_2_RunSpeModSym.Paths?, Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles?) linearFeatures, float moreEfficientSurroundingsWithPathTime)
    {
        var (naturalLinearObstacles, _, manMadeLinearObstacles) = linearFeatures; 
        return moreEfficientSurroundingsWithPathTime + 
               (naturalLinearObstacles is null ? 0 : NaturalLinearObstaclesCoefficients[naturalLinearObstacles.Value]) + 
               (manMadeLinearObstacles is null ? 0 : ManMadeLinearObstaclesCoefficients[manMadeLinearObstacles.Value]);
    }

    private double ComputePaceForElevatedPositions(Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes from, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes to, int mapScale)
    {
        var horizontalDistance = (to.Position - from.Position).Length() * mapScale / 1_000_000;
        var verticalDistance = to.Elevation - from.Elevation;
        var gradient = verticalDistance / horizontalDistance;
        var paceInSecPerMeter = AnthonyKaysPaceForHillRunnersModel.GetPace(gradient, false);
        return paceInSecPerMeter;
    }

    public float GetWeightFromHeuristics(Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes from, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes to, int scale)
    {
        float surroundingsMaxCoefficient = 0;
        foreach (var (_,adjustable) in GroundCoefficients)
             surroundingsMaxCoefficient = Math.Max(adjustable, surroundingsMaxCoefficient);
        foreach (var (_,adjustable) in BoulderCoefficients)
             surroundingsMaxCoefficient = Math.Max(adjustable, surroundingsMaxCoefficient);
        foreach (var (_,adjustable) in StoneCoefficients)
             surroundingsMaxCoefficient = Math.Max(adjustable, surroundingsMaxCoefficient);
        foreach (var (_,adjustable) in WaterCoefficients)
             surroundingsMaxCoefficient = Math.Max(adjustable, surroundingsMaxCoefficient);
        foreach (var (vegetationAndManMade,coefficient) in VegetationAndManMadeCoefficients)
             if (vegetationAndManMade != Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.PavedArea_501) surroundingsMaxCoefficient = Math.Max(coefficient((0,new MapCoordinates(1,1))), surroundingsMaxCoefficient); 
        foreach (var (_,adjustable) in VegetationGoodVisCoefficients)
             surroundingsMaxCoefficient = Math.Max(adjustable, surroundingsMaxCoefficient);
        
        float pathMaxCoefficient = VegetationAndManMadeCoefficients[Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.PavedArea_501]((0, new MapCoordinates(1,1)));
        foreach (var (path, adjustable) in PathsCoefficients)
            if (path == Orienteering_ISOM_2017_2_RunSpeModSym.Paths.LessDistinctSmallFootpath_507 || path == Orienteering_ISOM_2017_2_RunSpeModSym.Paths.NarrowRide_508)
                pathMaxCoefficient = Math.Max(adjustable* surroundingsMaxCoefficient, pathMaxCoefficient);
            else pathMaxCoefficient = Math.Max(adjustable, pathMaxCoefficient);

        var pace = AnthonyKaysPaceForHillRunnersModel.GetMinimalPace();
        // var pace = 0.24;
        return (float)(pace / Math.Max(surroundingsMaxCoefficient, pathMaxCoefficient) * (to.Position - from.Position).Length() * scale / 1_000_000);
    }
    
    public MapCoordinates RetrievePosition(Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes vertexAttributes)
        => vertexAttributes.Position;
    

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Grounds, float>? _groundCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Grounds, float> GroundCoefficients
    {
        get
        {
            if (_groundCoefficients is null) 
                _groundCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.BrokenGround_113, BrokenGroundCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.VeryBrokenGround_114, VeryBrokenGroundCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.CultivatedLand_412, CultivatedLandCoefficient.Value}
                };
            return _groundCoefficients;
        }
    }
    private class BrokenGroundAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public BrokenGroundAdjustable(float value) : this() => Value = value; }
    private class VeryBrokenGroundAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VeryBrokenGroundAdjustable(float value) : this() => Value = value; }
    private class CultivatedLandAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public CultivatedLandAdjustable(float value) : this() => Value = value; }

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Boulders, float>? _boulderCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Boulders, float> BoulderCoefficients
    {
        get
        {
            if (_boulderCoefficients is null) 
                _boulderCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.GiganticBoulder_206, GiganticBoulderCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.BoulderField_208, BoulderFieldCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.DenseBoulderField_209, DenseBoulderFieldCoefficient.Value},
                };
            return _boulderCoefficients;
        }
    } 
    
    private class GiganticBoulderAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public GiganticBoulderAdjustable(float value) : this() => Value = value;}
    private class BoulderFieldAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public BoulderFieldAdjustable(float value) : this() => Value = value;}
    private class DenseBoulderFieldAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public DenseBoulderFieldAdjustable(float value) : this() => Value = value;}

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Stones, float>? _stoneCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Stones, float> StoneCoefficients
    {
        get
        {
            if (_stoneCoefficients is null) 
                _stoneCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundSlow_210, StonyGroundSlowCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundWalk_211, StonyGroundWalkCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundFight_212, StonyGroundFightCoefficient.Value},
                };
            return _stoneCoefficients;
        }
    } 
    private class StonyGroundSlowAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public StonyGroundSlowAdjustable(float value) : this() => Value = value;}
    private class StonyGroundWalkAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public StonyGroundWalkAdjustable(float value) : this() => Value = value;}
    private class StonyGroundFightAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public StonyGroundFightAdjustable(float value) : this() => Value = value;}
    private class SandyGroundAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public SandyGroundAdjustable(float value) : this() => Value = value;}
    private class BareRockAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1){ public BareRockAdjustable(float value) : this() => Value = value;}

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Water, float>? _waterCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Water, float> WaterCoefficients
    {
        get
        {
            if (_waterCoefficients is null) 
                _waterCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Water.UncrossableBodyOfWater_301, UncrossableBodyOfWaterCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Water.ShallowBodyOfWater_302, ShallowBodyOfWaterCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Water.UncrossableMarsh_307, UncrossableMarshCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Water.Marsh_308, MarshCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Water.IndistinctMarsh_310, IndistinctMarshCoefficient.Value}
                };
            return _waterCoefficients;
        }
    }
    
    private class UncrossableBodyOfWaterAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public UncrossableBodyOfWaterAdjustable(float value) : this() => Value = value; }
    private class ShallowBodyOfWaterAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public ShallowBodyOfWaterAdjustable(float value) : this() => Value = value; }
    private class UncrossableMarshAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public UncrossableMarshAdjustable(float value) : this() => Value = value; }
    private class MarshAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public MarshAdjustable(float value) : this() => Value = value; }
    private class IndistinctMarshAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public IndistinctMarshAdjustable(float value) : this() => Value = value; }
    
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade, Func<(float, MapCoordinates), float>>? _vegetationAndManMadeCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade, Func<(float, MapCoordinates), float>> VegetationAndManMadeCoefficients
    {
        get
        {
            if (_vegetationAndManMadeCoefficients is null) 
                _vegetationAndManMadeCoefficients = new()
                {
                    
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.SandyGround_213, _ => SandyGroundCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.BareRock_214, _ => BareRockCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.OpenLand_401, _ => OpenLandCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.OpenLandWithTrees_402, _ => OpenLandWithTreesCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOpenLand_403, _ => RoughOpenLandCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOpenLandWithTrees_404, _ => RoughOpenLandWithTreesCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Forest_405, _ => ForestCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationSlow_406, _ => VegetationSlowCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationSlowNormalOneDir_406_1, symbolRotationAndFromToVector => 
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationSlowCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos)+ ForestCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalk_408, _ => VegetationWalkCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalkNormalOneDir_408_1, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationWalkCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + ForestCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalkSlowOneDir_408_2, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationWalkCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + VegetationSlowCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFight_410, _ => VegetationFightCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightNormalOneDir_410_1, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationFightCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + ForestCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightSlowOneDir_410_2, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationFightCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + VegetationSlowCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightWalkOneDir_410_3, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VegetationFightCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + VegetationWalkCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Orchard_413, _ => OrchardCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOrchard_413, _ => RoughOrchardCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Vineyard_414, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VineyardCrossingCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + OpenLandCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughVineyard_414, symbolRotationAndFromToVector =>
                    {
                        var rotatedFromToVector = symbolRotationAndFromToVector.Item2.RotateClockwise(symbolRotationAndFromToVector.Item1);
                        var positiveRotatedFromToVector = new MapCoordinates(Math.Abs(rotatedFromToVector.XPos), Math.Abs(rotatedFromToVector.YPos));
                        return VineyardCrossingCoefficient.Value * positiveRotatedFromToVector.XPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos) + RoughOpenLandCoefficient.Value * positiveRotatedFromToVector.YPos / (positiveRotatedFromToVector.XPos + positiveRotatedFromToVector.YPos);
                    }},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.PavedArea_501, _ => PavedAreaCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.AreaThatShallNotBeEntered_520, _ => AreaThatShallNotBeEnteredCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Building_521, _ => BuildingCoefficient.Value}
                };
            return _vegetationAndManMadeCoefficients;
        }
    }

    private class OpenLandAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public OpenLandAdjustable(float value) : this() => Value = value; }
    private class OpenLandWithTreesAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public OpenLandWithTreesAdjustable(float value) : this() => Value = value; }
    private class RoughOpenLandAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public RoughOpenLandAdjustable(float value) : this() => Value = value; }
    private class RoughOpenLandWithTreesAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public RoughOpenLandWithTreesAdjustable(float value) : this() => Value = value; } 
    private class ForestAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public ForestAdjustable(float value) : this() => Value = value;} 
    private class VegetationSlowAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VegetationSlowAdjustable(float value) : this() => Value = value;} 
    private class VegetationWalkAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VegetationWalkAdjustable(float value) : this() => Value = value;} 
    private class VegetationFightAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VegetationFightAdjustable(float value) : this() => Value = value;} 
    private class OrchardAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public OrchardAdjustable(float value) : this() => Value = value;} 
    private class RoughOrchardAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public RoughOrchardAdjustable(float value) : this() => Value = value;} 
    private class VineyardCrossingAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VineyardCrossingAdjustable(float value) : this() => Value = value; } 
    private class PavedAreaAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public PavedAreaAdjustable(float value) : this() => Value = value;} 
    private class AreaThatShallNotBeEnteredAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public AreaThatShallNotBeEnteredAdjustable(float value) : this() => Value = value;} 
    private class BuildingAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public BuildingAdjustable(float value) : this() => Value = value;}

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.VegetationGoodVis, float>? _vegetationGoodVisCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.VegetationGoodVis, float> VegetationGoodVisCoefficients
    {
        get
        {
            if (_vegetationGoodVisCoefficients is null)
                _vegetationGoodVisCoefficients = new ()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationGoodVis.VegetationSlowGoodVis_407, VegetationSlowGoodVisibilityCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.VegetationGoodVis.VegetationWalkGoodVis_409, VegetationWalkGoodVisibilityCoefficient.Value}
                };
            return _vegetationGoodVisCoefficients;
        }
    }

    private class VegetationSlowGoodVisAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VegetationSlowGoodVisAdjustable(float value) : this() => Value = value;}
    private class VegetationWalkGoodVisAdjustable() : BoundedFloatValueAdjustable("", null, 0, 1) { public VegetationWalkGoodVisAdjustable(float value) : this() => Value = value;}

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles, float>? _naturalLinearObstaclesCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles, float> NaturalLinearObstaclesCoefficients
    {
        get
        {
            if (_naturalLinearObstaclesCoefficients is null)
                _naturalLinearObstaclesCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.EarthBank_104, EarthBankOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.EarthWall_105, EarthWallOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.ErosionGully_107, ErosionGullyOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.ImpassableCliff_201, ImpassableCliffOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.Cliff_202, CliffOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.CrossableWatercourse_304, CrossableWaterCourseOvercomeCoefficient.Value },
                    { Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.SmallCrossableWatercourse_305, SmallCrossableWaterCourseOvercomeCoefficient.Value },
                };
            return _naturalLinearObstaclesCoefficients;
        }
    }

    private class EarthBankAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public EarthBankAdjustable(float value) : this() => Value = value;}
    private class EarthWallAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public EarthWallAdjustable(float value) : this() => Value = value;}
    private class ErosionGullyAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ErosionGullyAdjustable(float value) : this() => Value = value;}
    private class ImpassableCliffAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ImpassableCliffAdjustable(float value) : this() => Value = value;}
    private class CliffAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public CliffAdjustable(float value) : this() => Value = value;}
    private class CrossableWatercourseAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public CrossableWatercourseAdjustable(float value) : this() => Value = value;}
    private class SmallCrossableWatercourseAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public SmallCrossableWatercourseAdjustable(float value) : this() => Value = value;}

    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Paths, float>? _pathsCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.Paths, float> PathsCoefficients
    {
        get
        {
            if (_pathsCoefficients is null)
                _pathsCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.WideRoad_502, WideRoadCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.Road_503, RoadCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.VehicleTrack_504, VehicleTrackCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.Footpath_505, FootpathCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.SmallFootpath_506, SmallFootpathCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.LessDistinctSmallFootpath_507, LessDistinctSmallFootpathCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.NarrowRide_508, NarrowRideCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.Paths.Stairway_532, StairwayCoefficient.Value},
                };
            return _pathsCoefficients;
        }
    }

    private class WideRoadAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public WideRoadAdjustable(float value) : this() => Value = value;}
    private class RoadAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public RoadAdjustable(float value) : this() => Value = value;}
    private class VehicleTrackAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public VehicleTrackAdjustable(float value) : this() => Value = value;}
    private class FootpathAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public FootpathAdjustable(float value) : this() => Value = value;}
    private class SmallFootpathAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public SmallFootpathAdjustable(float value) : this() => Value = value;}
    private class LessDistinctSmallFootpathAdjustable() : BoundedFloatValueAdjustable("", null, 1, 2) { public LessDistinctSmallFootpathAdjustable(float value) : this() => Value = value;}
    private class NarrowRideAdjustable() : BoundedFloatValueAdjustable("", null, 1, 2) { public NarrowRideAdjustable(float value) : this() => Value = value;}
    private class StairwayAdjustable() : BoundedFloatValueAdjustable("", null, 0.1f, 1) { public StairwayAdjustable(float value) : this() => Value = value;}
    
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles, float>? _manMadeLinearObstaclesCoefficients;
    private Dictionary<Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles, float> ManMadeLinearObstaclesCoefficients
    {
        get
        {
            if (_manMadeLinearObstaclesCoefficients is null)
                _manMadeLinearObstaclesCoefficients = new()
                {
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.Trench_215, TrenchOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.Wall_513, WallOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.ImpassableWall_515, ImpassableWallOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.Fence_516, FenceOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.ImpassableFence_518, ImpassableFenceOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.ProminentLineFeature_528, ProminentLineFeatureOvercomeCoefficient.Value},
                    { Orienteering_ISOM_2017_2_RunSpeModSym.ManMadeLinearObstacles.ImpassableProminentLineFeature_529, ImpassableProminentLineFeatureOvercomeCoefficient.Value},
                };
            return _manMadeLinearObstaclesCoefficients;
        }
    }
    
    private class TrenchAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public TrenchAdjustable(float value) : this() => Value = value;}
    private class WallAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public WallAdjustable(float value) : this() => Value = value;}
    private class ImpassableWallAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ImpassableWallAdjustable(float value) : this() => Value = value;}
    private class FenceAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public FenceAdjustable(float value) : this() => Value = value;}
    private class ImpassableFenceAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ImpassableFenceAdjustable(float value) : this() => Value = value;}
    private class ProminentLineFeatureAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ProminentLineFeatureAdjustable(float value) : this() => Value = value;}
    private class ImpassableProminentLineFeatureAdjustable() : BoundedFloatValueAdjustable("", "seconds", 0, float.PositiveInfinity) { public ImpassableProminentLineFeatureAdjustable(float value) : this() => Value = value;}
        
    [JsonInclude] private BrokenGroundAdjustable BrokenGroundCoefficient { get; set; } 
    [JsonInclude] private VeryBrokenGroundAdjustable VeryBrokenGroundCoefficient { get; set; }
    
    [JsonInclude] private GiganticBoulderAdjustable GiganticBoulderCoefficient { get; set; }
    [JsonInclude] private BoulderFieldAdjustable BoulderFieldCoefficient { get; set; }
    [JsonInclude] private DenseBoulderFieldAdjustable DenseBoulderFieldCoefficient { get; set; }
    
    [JsonInclude] private StonyGroundSlowAdjustable StonyGroundSlowCoefficient { get; set; }
    [JsonInclude] private StonyGroundWalkAdjustable StonyGroundWalkCoefficient { get; set; }
    [JsonInclude] private StonyGroundFightAdjustable StonyGroundFightCoefficient { get; set; }
    
    [JsonInclude] private UncrossableBodyOfWaterAdjustable UncrossableBodyOfWaterCoefficient { get; set; }
    [JsonInclude] private ShallowBodyOfWaterAdjustable ShallowBodyOfWaterCoefficient { get; set; }
    [JsonInclude] private UncrossableMarshAdjustable UncrossableMarshCoefficient { get; set; }
    [JsonInclude] private MarshAdjustable MarshCoefficient { get; set; }
    [JsonInclude] private IndistinctMarshAdjustable IndistinctMarshCoefficient { get; set; }
    
    [JsonInclude] private SandyGroundAdjustable SandyGroundCoefficient { get; set; }
    [JsonInclude] private BareRockAdjustable BareRockCoefficient { get; set; }
    [JsonInclude] private OpenLandAdjustable OpenLandCoefficient { get; set; }
    [JsonInclude] private OpenLandWithTreesAdjustable OpenLandWithTreesCoefficient { get; set; }
    [JsonInclude] private RoughOpenLandAdjustable RoughOpenLandCoefficient { get; set; }
    [JsonInclude] private RoughOpenLandWithTreesAdjustable RoughOpenLandWithTreesCoefficient { get; set; }
    [JsonInclude] private ForestAdjustable ForestCoefficient { get; set; }
    [JsonInclude] private VegetationSlowAdjustable VegetationSlowCoefficient { get; set; }
    [JsonInclude] private VegetationSlowGoodVisAdjustable VegetationSlowGoodVisibilityCoefficient { get; set; }
    [JsonInclude] private VegetationWalkAdjustable VegetationWalkCoefficient { get; set; }
    [JsonInclude] private VegetationWalkGoodVisAdjustable VegetationWalkGoodVisibilityCoefficient { get; set; }
    [JsonInclude] private VegetationFightAdjustable VegetationFightCoefficient { get; set; }
    [JsonInclude] private CultivatedLandAdjustable CultivatedLandCoefficient { get; set; }
    [JsonInclude] private OrchardAdjustable OrchardCoefficient { get; set; }
    [JsonInclude] private RoughOrchardAdjustable RoughOrchardCoefficient { get; set; }
    [JsonInclude] private VineyardCrossingAdjustable VineyardCrossingCoefficient { get; set; }
    
    [JsonInclude] private PavedAreaAdjustable PavedAreaCoefficient { get; set; }
    [JsonInclude] private WideRoadAdjustable WideRoadCoefficient { get; set; }
    [JsonInclude] private RoadAdjustable RoadCoefficient { get; set; }
    [JsonInclude] private VehicleTrackAdjustable VehicleTrackCoefficient { get; set; }
    [JsonInclude] private FootpathAdjustable FootpathCoefficient { get; set; }
    [JsonInclude] private SmallFootpathAdjustable SmallFootpathCoefficient { get; set; }
    [JsonInclude] private LessDistinctSmallFootpathAdjustable LessDistinctSmallFootpathCoefficient { get; set; }
    [JsonInclude] private NarrowRideAdjustable NarrowRideCoefficient { get; set; }
    
    [JsonInclude] private AreaThatShallNotBeEnteredAdjustable AreaThatShallNotBeEnteredCoefficient { get; set; }
    [JsonInclude] private BuildingAdjustable BuildingCoefficient { get; set; }
    [JsonInclude] private StairwayAdjustable StairwayCoefficient { get; set; }
    
    [JsonInclude] private EarthBankAdjustable EarthBankOvercomeCoefficient { get; set; } 
    [JsonInclude] private EarthWallAdjustable EarthWallOvercomeCoefficient { get; set; }
    [JsonInclude] private ErosionGullyAdjustable ErosionGullyOvercomeCoefficient { get; set; }
    [JsonInclude] private ImpassableCliffAdjustable ImpassableCliffOvercomeCoefficient { get; set; }
    [JsonInclude] private CliffAdjustable CliffOvercomeCoefficient { get; set; }
    [JsonInclude] private TrenchAdjustable TrenchOvercomeCoefficient { get; set; }
    [JsonInclude] private CrossableWatercourseAdjustable CrossableWaterCourseOvercomeCoefficient { get; set; }
    
    [JsonInclude] private SmallCrossableWatercourseAdjustable SmallCrossableWaterCourseOvercomeCoefficient { get; set; }
    [JsonInclude] private WallAdjustable WallOvercomeCoefficient { get; set; }
    [JsonInclude] private ImpassableWallAdjustable ImpassableWallOvercomeCoefficient { get; set; }
    [JsonInclude] private FenceAdjustable FenceOvercomeCoefficient { get; set; }
    [JsonInclude] private ImpassableFenceAdjustable ImpassableFenceOvercomeCoefficient { get; set; }
    [JsonInclude] private ProminentLineFeatureAdjustable ProminentLineFeatureOvercomeCoefficient { get; set; }
    [JsonInclude] private ImpassableProminentLineFeatureAdjustable ImpassableProminentLineFeatureOvercomeCoefficient { get; set; }
    
    public TOut AcceptGeneric<TOut, TOtherParams>(IUserModelGenericVisitor<TOut, TOtherParams> genericVisitor, TOtherParams otherParams)
        => genericVisitor.GenericVisit<Orienteering_ISOM_2017_2RunSpeModSymUserModel, Orienteering_ISOM_2017_2_RunSpeModSym>(this, otherParams);
    public TOut AcceptGeneric<TOut>(IUserModelGenericVisitor<TOut> genericVisitor)
        => genericVisitor.GenericVisit<Orienteering_ISOM_2017_2RunSpeModSymUserModel, Orienteering_ISOM_2017_2_RunSpeModSym>(this);
    public void AcceptGeneric(IUserModelGenericVisitor genericVisitor)
        => genericVisitor.GenericVisit<Orienteering_ISOM_2017_2RunSpeModSymUserModel, Orienteering_ISOM_2017_2_RunSpeModSym>(this);
}