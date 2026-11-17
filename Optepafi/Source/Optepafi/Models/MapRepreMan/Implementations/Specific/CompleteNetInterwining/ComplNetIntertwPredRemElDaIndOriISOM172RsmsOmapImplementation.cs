using System.Collections;
using System.Collections.Generic;
using System.IO;
using Optepafi.Models.MapRepreMan.Configurations;
using Optepafi.Models.MapRepreMan.Graphs;
using Optepafi.Models.MapRepreMan.Graphs.Functionalities;
using Optepafi.Models.MapRepreMan.Graphs.Specific;
using Optepafi.Models.MapRepreMan.Graphs.Specific.CompleteNetIntertwining;
using Optepafi.Models.MapRepreMan.Utils;
using Optepafi.Models.TemplateMan;
using Optepafi.Models.TemplateMan.TemplateAttributes;
using Optepafi.Models.TemplateMan.Templates;
using Optepafi.Models.Utils;

namespace Optepafi.Models.MapRepreMan.Implementations.Specific.CompleteIterativelySnapping;

/// <summary>
/// Complete net intertwining graph implementation of OMAP map representation which uses <see cref="Orienteering_ISOM_2017_2_RunSpeModSym"/>
/// tempalte and does not require elevation data for its creation. It does not hold any elevation information at all.
/// </summary>
public abstract class CompleteNetIntertwiningPredecessorsRememberingElevDataIndepOrienteeringIsom20172RunSpeModSymOmapMapImplementation(INearestNeighborsSearchableDataStructure<ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex> searchableVertices, int scale) : 
    ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>, 
    IEnumerable<ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex>
{
    public string Name { get; } = "Complete net intertwining map repre. elev. data independent predecessors remembering implementation for Orienteering (ISOM 2017-2, running speed modifying symbols only) and Omap file format.";

    // public INearestNeighborsSearchableDataStructure<ICompleteNetIntertwiningPredecessorRememberingGraph< Orienteering_ISOM_2017_2.VertexAttributes, Orienteering_ISOM_2017_2.EdgeAttributes>.Vertex> SearchableVertices { get; } = searchableVertices; //for debugging

    public abstract void RestoreConsistency();
    public abstract ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex GetVertexFor(MapCoordinates coords);
    
    public int Scale { get; } = scale;

    public TOut AcceptGeneric<TOut, TOtherParams>(IGraphGenericVisitor<TOut, TOtherParams> genericVisitor, TOtherParams otherParams)
        => genericVisitor.GenericVisit<CompleteNetIntertwiningPredecessorsRememberingElevDataIndepOrienteeringIsom20172RunSpeModSymOmapMapImplementation, ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex,ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Edge>(this, otherParams);
    public TOut Traverse<TOut, TOtherParams>(IFromPromiseToSearchableTraverser<TOut, float, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes, TOtherParams> traverser, TOtherParams otherParams)
        => traverser.AcceptedTraverse(this, otherParams);
    public TOut Traverse<TOut, TOtherParams>(IFromPromiseToPredecessorRememberingTraverser<TOut, float, Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes, TOtherParams> traverser, TOtherParams otherParams) 
        => traverser.AcceptedTraverse(this, otherParams);
    public TOut AcceptGeneric<TOut, TOtherParams>(IMapRepreGenericVisitor<TOut, TOtherParams> genericVisitor, TOtherParams otherParams)
        => genericVisitor.GenericVisit(this, otherParams);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<ICompleteNetIntertwiningPredecessorsRememberingGraph<Orienteering_ISOM_2017_2_RunSpeModSym.VertexAttributes, Orienteering_ISOM_2017_2_RunSpeModSym.EdgeAttributes>.Vertex> GetEnumerator() => searchableVertices.GetEnumerator();
}