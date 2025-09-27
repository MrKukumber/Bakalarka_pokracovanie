using Optepafi.Models.MapMan.MapInterfaces;
using Optepafi.Models.MapRepreMan.Graphs.Specific.CompleteNetIntertwining;
using Optepafi.Models.MapRepreMan.Implementations.Representatives;
using Optepafi.Models.MapRepreMan.Implementations.Representatives.Specific.CompleteIterativelySnapping;
using Optepafi.Models.MapRepreMan.MapRepres;
using Optepafi.Models.TemplateMan;
using Optepafi.Models.TemplateMan.TemplateAttributes;

namespace Optepafi.Models.MapRepreMan.Graphs.Representatives.Specific.CompleteNetIntertwining;

public class CompleteNetIntertwiningBasicGraphRep<TVertexAttributes, TEdgeAttributes> : 
    GraphRepresentative<ICompleteNetIntertwiningBasicGraph<TVertexAttributes, TEdgeAttributes>, ICompleteNetIntertwiningBasicGraph<TVertexAttributes, TEdgeAttributes>.Vertex, ICompleteNetIntertwiningBasicGraph<TVertexAttributes, TEdgeAttributes>.Edge>
    where TVertexAttributes : IVertexAttributes
    where TEdgeAttributes : IEdgeAttributes
{
    
    public static CompleteNetIntertwiningBasicGraphRep<TVertexAttributes, TEdgeAttributes> Instance { get; } = new();
    private CompleteNetIntertwiningBasicGraphRep() { }

    public override IImplementationIndicator<ITemplate, IMap, IMapRepre>[] CreateableImplementationsIndicators { get; } = 
        [
/*Indep*/   CompleteNetIntertwiningBasicElevDataDepOrienteering_ISOM_2017_2RunSpeModSymOmapMapImplementationRep.Instance, 
/*Dep*/     CompleteNetIntertwiningBasicElevDataIndepOrienteering_ISOM_2017_2RunSpeModSymOmapMapImplementationRep.Instance
        ];

    public override TOut GenericTraverse<TOut, TOtherParams>( IFromGraphicMapRepreProcToGraphRepGenericTraverser<TOut, TOtherParams> traverser, TOtherParams otherParams)
        => traverser.AcceptedGenericTraverse(this, otherParams);
}