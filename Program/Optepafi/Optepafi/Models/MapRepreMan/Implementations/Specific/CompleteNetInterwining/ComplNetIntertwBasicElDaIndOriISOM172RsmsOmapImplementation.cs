using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Optepafi.Models.MapRepreMan.Graphs;
using Optepafi.Models.MapRepreMan.Graphs.Functionalities;
using Optepafi.Models.MapRepreMan.Graphs.Specific.CompleteNetIntertwining;
using Optepafi.Models.MapRepreMan.Utils;
using Optepafi.Models.TemplateMan.Templates;
using Optepafi.Models.Utils;

namespace Optepafi.Models.MapRepreMan.Implementations.Specific.CompleteIterativelySnapping;

public abstract class CompleteNetIntertwiningBasicElevDataIndepOrienteering_ISOM_2017_2RunSpeModSymOmapMapImplementation(INearestNeighborsSearchableDataStructure<ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes,Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex> searchableVertices, int scale) :
    ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>, 
    IEnumerable<ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex>
{
    public string Name { get; } = "Complete net intertwining map repre. elev. data independent basic implementation for Orienteering (ISOM 2017-2) and Omap file format.";

    public INearestNeighborsSearchableDataStructure<ICompleteNetIntertwiningBasicGraph< Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex> SearchableVertices { get; } = searchableVertices; //for debugging
    public abstract void RestoreConsistency();
    public abstract ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex GetVertexFor(MapCoordinates coords);
    
    
    public int Scale { get; } = scale;
    
    public TOut AcceptGeneric<TOut,TOtherParams>( IGraphGenericVisitor<TOut, TOtherParams> genericVisitor, TOtherParams otherParams) 
        => genericVisitor.GenericVisit<CompleteNetIntertwiningBasicElevDataIndepOrienteering_ISOM_2017_2RunSpeModSymOmapMapImplementation, ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex,ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Edge>(this, otherParams);
    public TOut Traverse<TOut, TOtherParams>(IFromCanBeSearchedToSearchableTraverser<TOut, float, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes, TOtherParams> traverser, TOtherParams otherParams)
        => traverser.AcceptedTraverse(this, otherParams);
    public TOut AcceptGeneric<TOut, TOtherParams>(IMapRepreGenericVisitor<TOut, TOtherParams> genericVisitor, TOtherParams otherParams)
        => genericVisitor.GenericVisit(this, otherParams);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<ICompleteNetIntertwiningBasicGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex> GetEnumerator() => searchableVertices.GetEnumerator();
}