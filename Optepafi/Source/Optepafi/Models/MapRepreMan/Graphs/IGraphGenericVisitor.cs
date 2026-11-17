using Optepafi.Models.MapRepreMan.VerticesAndEdges;
using Optepafi.Models.TemplateMan;
using Optepafi.Models.TemplateMan.TemplateAttributes;

namespace Optepafi.Models.MapRepreMan.Graphs;

//TODO: comment
public interface IGraphGenericVisitor<TOut, TOtherParams>
{
    public TOut GenericVisit<TGraph, TVertex, TEdge>(TGraph graph, TOtherParams otherParams)
        where TGraph : IGraph<TVertex, TEdge>
        where TVertex : IVertex
        where TEdge : IEdge;
}