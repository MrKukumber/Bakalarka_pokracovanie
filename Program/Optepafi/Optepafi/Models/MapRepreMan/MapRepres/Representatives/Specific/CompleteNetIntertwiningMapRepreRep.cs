using Optepafi.Models.MapRepreMan.Configurations;
using Optepafi.Models.MapRepreMan.Graphs.Representatives;
using Optepafi.Models.MapRepreMan.Graphs.Representatives.Specific.CompleteNetIntertwining;
using Optepafi.Models.MapRepreMan.MapRepres.Specific;

namespace Optepafi.Models.MapRepreMan.MapRepres.Representatives.Specific;

//TODO: comment
public class CompleteNetIntertwiningMapRepreRep : MapRepreRepresentative<ICompleteNetIntertwiningMapRepre, CompleteNetIntertwiningMapRepreConfiguration>
{
    public static CompleteNetIntertwiningMapRepreRep Instance { get; } = new();
    private CompleteNetIntertwiningMapRepreRep() { }
    
    ///<inheritdoc cref="IMapRepreRepresentative{TMapRepre}.MapRepreName"/>
    public override string MapRepreName => "Complete, net intertwining map representation";
    
    ///<inheritdoc cref="IMapRepreRepresentative{TMapRepre}.GetCorrespondingGraphCreator{TVertexAttributes,TEdgeAttributes}"/>
    public override IGraphicMapRepreProcessor<ICompleteNetIntertwiningMapRepre>[] GetMapRepreProcessors<TVertexAttributes, TEdgeAttributes>() 
        => [
            CompleteNetIntertwiningBasicGraphRep<TVertexAttributes, TEdgeAttributes>.Instance, 
            CompleteNetIntertwiningPredecessorRememberingGraphRep<TVertexAttributes, TEdgeAttributes>.Instance
        ];
    protected override CompleteNetIntertwiningMapRepreConfiguration DefaultConfiguration { get; } = new (2_000, 0.25f, 0);
}