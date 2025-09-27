using Optepafi.Models.MapRepreMan.VerticesAndEdges;

namespace Optepafi.Models.MapRepreMan.Graphs.Representatives;

public interface IFromGraphicMapRepreProcToGraphRepGenericTraverser<TOut, TOtherParams>
{
    TOut AcceptedGenericTraverse<TVertex, TEdge>(IGraphRepresentative<IGraph<TVertex, TEdge>, TVertex, TEdge> graphRepresentative, TOtherParams otherParams)
        where TVertex: IVertex 
        where TEdge : IEdge;
}