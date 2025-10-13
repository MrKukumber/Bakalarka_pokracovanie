using Optepafi.Models.MapMan.MapInterfaces;
using Optepafi.Models.MapRepreMan.Graphs.Specific;
using Optepafi.Models.MapRepreMan.Graphs.Specific.CompleteNetIntertwining;
using Optepafi.Models.MapRepreMan.Implementations.Representatives;
using Optepafi.Models.MapRepreMan.Implementations.Representatives.Specific.CompleteIterativelySnapping;
using Optepafi.Models.MapRepreMan.MapRepres;
using Optepafi.Models.SearchingAlgorithmMan;
using Optepafi.Models.SearchingAlgorithmMan.SearchingAlgorithms;
using Optepafi.Models.TemplateMan;
using Optepafi.Models.TemplateMan.TemplateAttributes;
using Optepafi.Models.UserModelMan.UserModelReps;
using Optepafi.Models.UserModelMan.UserModels.Functionalities;

namespace Optepafi.Models.MapRepreMan.Graphs.Representatives.Specific.CompleteNetIntertwining;

//TODO: comment
public class CompleteNetIntertwiningPredecessorRememberingGraphRep<TVertexAttributes, TEdgeAttributes> : 
    GraphRepresentative<ICompleteNetIntertwiningPredecessorsRememberingGraph<TVertexAttributes, TEdgeAttributes>, ICompleteNetIntertwiningPredecessorsRememberingGraph<TVertexAttributes, TEdgeAttributes>.Vertex, ICompleteNetIntertwiningPredecessorsRememberingGraph<TVertexAttributes, TEdgeAttributes>.Edge>
    where TVertexAttributes : IVertexAttributes
    where TEdgeAttributes : IEdgeAttributes
{
    public static CompleteNetIntertwiningPredecessorRememberingGraphRep<TVertexAttributes, TEdgeAttributes> Instance { get; } = new();
    private CompleteNetIntertwiningPredecessorRememberingGraphRep() { }
    
    public override IImplementationIndicator<ITemplate, IMap, IMapRepre>[] CreateableImplementationsIndicators { get; } = [CompleteNetIntertwiningPredecessorRememberingElevDataIndepOrienteering_ISOM_2017_2RunSpeModSymOmapMapImplementationRep.Instance];

    public override TOut Traverse<TOut, TOtherParams>( IFromGraphicMapRepreProcToGraphRepTraverser<TOut, TOtherParams> traverser, TOtherParams otherParams)
        => traverser.AcceptedTraverse(this, otherParams);
}