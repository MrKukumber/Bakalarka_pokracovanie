using Avalonia.Media;
using Optepafi.Models.GraphicsMan.Objects.MapRepre;
using Optepafi.Models.TemplateMan.Templates;
using Optepafi.Models.Utils;
using Optepafi.ModelViews.Utils;

namespace Optepafi.ViewModels.Data.Graphics.MapRepreObjects;

public class VertexObjectViewModel(VertexObject obj, MapCoordinates mapsTopLeftVertex) : GraphicObjectViewModel
{
    public override CanvasCoordinate Position { get; } = obj.Position.ToCanvasCoordinate(mapsTopLeftVertex) - new CanvasCoordinate(55, 55);
    public override int Priority { get; } = 36;
    public int XCoord { get; } = obj.Position.XPos;
    public int YCoord { get; } = obj.Position.YPos;

    public int Elevation { get; } = (int) obj.Elevation;
    // public float FontSize { get; } = 0.2f; // size for lengths of edge 500
    public float FontSize { get; } = 0.5f;
    public int Diameter { get; } = 100;
    
    public int LineThickness { get; } = 10;
}
public class EdgeObjectViewModel : GraphicObjectViewModel
{
    public override CanvasCoordinate Position { get; } 
    public override int Priority { get; } = 35;

    public CanvasCoordinate StartPoint { get; }
    
    public CanvasCoordinate EndPoint { get; } 
    // public int TopLineThickness { get; } = 24; // size for lengths of edge 500
    public int TopLineThickness { get; } = 80;
    // public int BottomLineThickness { get; } = 27; // size for lengths of edge 500
    public int BottomLineThickness { get; } = 90;

    public EdgeObjectViewModel(EdgeObject obj, MapCoordinates mapsTopLeftVertex)
    {
        Position  = obj.From.ToCanvasCoordinate(mapsTopLeftVertex);
        StartPoint = new CanvasCoordinate(0, 0);
        EndPoint  = obj.To.ToCanvasCoordinate(mapsTopLeftVertex) - obj.From.ToCanvasCoordinate(mapsTopLeftVertex);
        var surroundings = obj.Attributes.LeftSurroundings;
        var lines = obj.Attributes.LineFeatures;

        BoulderColor = surroundings.Item1.boulders switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.GiganticBoulder_206 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.BoulderField_208 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.Boulders.DenseBoulderField_209 => Colors.Black,
            null => Colors.Transparent
        };
        
        GroundColor = surroundings.Item1.ground switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.BrokenGround_113 => Colors.Chocolate,
            Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.VeryBrokenGround_114 => Colors.Chocolate,
            Orienteering_ISOM_2017_2_RunSpeModSym.Grounds.CultivatedLand_412 => Colors.Gold,
            null => Colors.Transparent,
        };
        
        StonesColor = surroundings.Item1.stones switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundSlow_210 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundWalk_211 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.Stones.StonyGroundFight_212 => Colors.Black,
            null => Colors.Transparent
        };

        VegetationManMadeColor = surroundings.Item1.vegetationAndManMade switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.SandyGround_213 => Colors.NavajoWhite,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.BareRock_214 => Colors.Gray,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.OpenLand_401 => Colors.Gold,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.OpenLandWithTrees_402 => Colors.Gold,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOpenLand_403 => Colors.NavajoWhite,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOpenLandWithTrees_404 => Colors.NavajoWhite,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Forest_405 => Colors.White,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationSlow_406 => Colors.DarkSeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationSlowNormalOneDir_406_1 => Colors.DarkSeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalk_408 => Colors.MediumSeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalkNormalOneDir_408_1 => Colors.MediumSeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationWalkSlowOneDir_408_2 => Colors.MediumSeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFight_410 => Colors.SeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightNormalOneDir_410_1 => Colors.SeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightSlowOneDir_410_2 => Colors.SeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.VegetationFightWalkOneDir_410_3 => Colors.SeaGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Orchard_413 => Colors.Gold,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughOrchard_413 => Colors.NavajoWhite,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Vineyard_414 => Colors.Gold,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.RoughVineyard_414 => Colors.NavajoWhite,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.PavedArea_501 => Colors.BurlyWood,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.AreaThatShallNotBeEntered_520 => Colors.YellowGreen,
            Orienteering_ISOM_2017_2_RunSpeModSym.VegetationAndManMade.Building_521 => Colors.Black,
            null => Colors.Transparent
        };

        WaterColor = surroundings.Item1.water switch
        {
            null => Colors.Transparent,
            _ => Colors.DodgerBlue
        };

        VegetationGoodVisColor = surroundings.Item1.vegetationGoodVis switch
        {
            null => Colors.Transparent,
            _ => Colors.DarkSeaGreen
        };

        NaturalObstacleColor = lines.naturalLinearObstacle switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.EarthBank_104 => Colors.Chocolate,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.EarthWall_105 => Colors.Chocolate,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.ErosionGully_107 => Colors.Chocolate,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.ImpassableCliff_201 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.Cliff_202 => Colors.Black,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.CrossableWatercourse_304 => Colors.DodgerBlue,
            Orienteering_ISOM_2017_2_RunSpeModSym.NaturalLinearObstacles.SmallCrossableWatercourse_305 => Colors.DodgerBlue,
            null => Colors.Transparent
        };

        PathsColor = lines.path switch
        {
            Orienteering_ISOM_2017_2_RunSpeModSym.Paths.WideRoad_502 => Colors.BurlyWood,
            null => Colors.Transparent,
            _ => Colors.Black
        };

        ManMadeObstacleColor = lines.manMadeLinearObstacle switch
        {
            null => Colors.Transparent,
            _ => Colors.Black
        };
    }
    public Color BoulderColor { get; }
    public Color GroundColor { get; }
    public Color StonesColor { get; }
    public Color VegetationManMadeColor { get; }
    public Color WaterColor { get; }
    public Color VegetationGoodVisColor { get; }
    public Color NaturalObstacleColor { get; }
    public Color PathsColor { get; }
    public Color ManMadeObstacleColor { get; }
}
