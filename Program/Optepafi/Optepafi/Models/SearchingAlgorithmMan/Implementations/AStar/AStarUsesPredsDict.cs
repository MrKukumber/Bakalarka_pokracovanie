using System;
using System.Collections.Generic;
using System.Threading;
using Optepafi.Models.MapRepreMan.Graphs;
using Optepafi.Models.MapRepreMan.Graphs.Functionalities;
using Optepafi.Models.MapRepreMan.Graphs.Representatives;
using Optepafi.Models.MapRepreMan.VerticesAndEdges;
using Optepafi.Models.ReportMan.Reports;
using Optepafi.Models.SearchingAlgorithmMan.Configurations;
using Optepafi.Models.SearchingAlgorithmMan.Paths;
using Optepafi.Models.SearchingAlgorithmMan.Paths.Specific;
using Optepafi.Models.TemplateMan;
using Optepafi.Models.TemplateMan.TemplateAttributes;
using Optepafi.Models.UserModelMan.UserModelReps;
using Optepafi.Models.UserModelMan.UserModels;
using Optepafi.Models.UserModelMan.UserModels.Functionalities;
using Priority_Queue;
using Tmds.DBus.Protocol;

namespace Optepafi.Models.SearchingAlgorithmMan.Implementations.AStar;

/// <summary>
/// Classic implementation of A* algorithm which uses dictionary for remembering current predecessors of processed vertices by algorithm.
///
/// For more general information on the concept of searching algorithm implementations, see <see cref="SearchingAlgorithmImplementation{TConfiguration}"/>   
/// </summary>
public class AStarUsesPredsDict : SearchingAlgorithmImplementation<AStarConfiguration>
{
    public static AStarUsesPredsDict Instance { get; } = new();
    private AStarUsesPredsDict() {}
    
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.DoesRepresentUsableGraph{TVertex,TEdge,TVertexAttributes,TEdgeAttributes}"/> 
    public override bool DoesRepresentUsableGraph<TVertex, TEdge, TVertexAttributes, TEdgeAttributes>(
        IGraphRepresentative<IGraph<TVertex, TEdge>, TVertex, TEdge> graphRepresentative) 
    {
        return graphRepresentative is IGraphRepresentative<IPromisesSearchability<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>, TVertex, TEdge>;
    }
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.DoesRepresentUsableUserModel{TTemplate,TVertexAttributes,TEdgeAttributes}"/>
    public override bool DoesRepresentUsableUserModel<TTemplate, TVertexAttributes, TEdgeAttributes>(IUserModelType<IComputing<TTemplate, TVertexAttributes, TEdgeAttributes>, TTemplate> userModelType) 
    {
        return userModelType 
            is IUserModelType<IAStarHeuristicsComputing<TTemplate, TVertexAttributes, TEdgeAttributes> , TTemplate> 
            and IUserModelType<IWeightComputing<TTemplate, TVertexAttributes, TEdgeAttributes>, TTemplate> ;
    }
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.IsUsableGraph{TVertex,TEdge,TVertexAttributes,TEdgeAttributes}"/>
    public override bool IsUsableGraph<TVertex, TEdge, TVertexAttributes, TEdgeAttributes>(IGraph<TVertex, TEdge> graph) 
    {
        return graph is IPromisesSearchability<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>;
    }
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.IsUsableUserModel{TVertexAttributes,TEdgeAttributes}"/>
    public override bool IsUsableUserModel<TVertexAttributes, TEdgeAttributes>(IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel) 
    {
        return userModel 
            is IAStarHeuristicsComputing<ITemplate<TVertexAttributes,TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> 
            and IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes>;
    }
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.SearchForPaths{TVertexAttributes,TEdgeAttributes,TVertex,TEdge}"/>
    public override IPath<TVertexAttributes, TEdgeAttributes>[] SearchForPaths<TVertexAttributes, TEdgeAttributes, TVertex, TEdge>(Leg[] track, IGraph<TVertex, TEdge> graph, IList<IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>> userModels,
        AStarConfiguration configuration, IProgress<ISearchingReport>? progress, CancellationToken? cancellationToken) 
    {
        List<IPath<TVertexAttributes, TEdgeAttributes>> paths = new();
        foreach (var userModel in userModels)
        {
            paths.Add(ExecutorSearch(track, graph, userModel, configuration, progress, cancellationToken));
            graph.RestoreConsistency();
        }
        return paths.ToArray();
    }

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}.ExecutorSearch{TVertexAttributes,TEdgeAttributes,TVertex,TEdge}"/>
    protected override IPath<TVertexAttributes, TEdgeAttributes> ExecutorSearch<TVertexAttributes, TEdgeAttributes, TVertex, TEdge>(Leg[] track, IGraph<TVertex, TEdge> graph,
        IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel, AStarConfiguration configuration, IProgress<ISearchingReport>? progress, CancellationToken? cancellationToken) 
        => Algorithm<TVertexAttributes, TEdgeAttributes>.Instance.Search(track, graph, userModel, configuration, progress, cancellationToken); 
    
    /// <summary>
    /// Helper class, used mainly for remembering generic type parameters received by calling traverse function of searchability promising graph.
    /// </summary>
    /// <typeparam name="TVertexAttributes">Vertex attributes used in algorithm execution.</typeparam>
    /// <typeparam name="TEdgeAttributes">Edge attributes used in algorithm execution.</typeparam>
    private class Algorithm<TVertexAttributes, TEdgeAttributes> :
        IFromPromiseToSearchableTraverser<SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>, float, TVertexAttributes, TEdgeAttributes, (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken? )> 
        where TVertexAttributes : IVertexAttributes
        where TEdgeAttributes : IEdgeAttributes
    {
        public static Algorithm<TVertexAttributes, TEdgeAttributes> Instance { get; } = new();
        private Algorithm() { }
        
        /// <summary>
        /// How ofen is the cancellation of path searching checked.
        /// </summary>
        private const int CancellationCheckDensity = 5000;
        
        public IPath<TVertexAttributes, TEdgeAttributes> Search<TVertex, TEdge>(Leg[] track, IGraph<TVertex, TEdge> graph, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel, AStarConfiguration configuration, IProgress<ISearchingReport>? progress, CancellationToken? cancellationToken)
        where TVertex : IVertex
        where TEdge : IEdge
        {
            if (graph is IPromisesSearchability<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>
                searchableGraph)
            {
                SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> path = new();
                foreach (var leg in track)
                {
                    if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested) return path;
                    path.MergeWith(searchableGraph.Traverse(this, (leg, userModel, configuration, progress, cancellationToken)));
                }
                return path;
            }
            throw new ArgumentException("Graph was not convertible to IRemembersPredecessorsGenericVisitor even though it should be checked before call of this method.");
        }

        SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> IFromPromiseToSearchableTraverser<SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>, float, TVertexAttributes, TEdgeAttributes, (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken? )>.
            AcceptedTraverse<TVertex, TEdge>(ISearchable<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes> searchableGraph,
            (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken?) otherParams)
        {
            var (leg, userModel, configuration, progress, cancellationToken) = otherParams;
            if (userModel is IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> weightComputingUserModel
                && userModel is IAStarHeuristicsComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> aStarHeuristicsComputingUserModel)
            {
                TVertex startVertex = searchableGraph.GetVertexFor(leg.Start);
                TVertex finishVertex = searchableGraph.GetVertexFor(leg.Finish);
                SimplePriorityQueue<TVertex, float> frontier = new();
                Dictionary<TVertex, float> frontierVerticesCosts = new();
                HashSet<TVertex> visited = new();
                Dictionary<TVertex, TVertex> predecessors = new();
                frontier.Enqueue(startVertex, aStarHeuristicsComputingUserModel.GetWeightFromHeuristics(startVertex.Attributes, finishVertex.Attributes, searchableGraph.Scale) + 0);
                frontierVerticesCosts.Add(startVertex, 0);
                int timeSinceLastCancellationCheck = 0;
                while (frontier.TryDequeue(out TVertex vertex))
                {
                    if (++timeSinceLastCancellationCheck >= CancellationCheckDensity && cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
                        break;
                    visited.Add(vertex);
                    if (vertex.Equals(finishVertex))
                        return GetPathToFinish<TVertex, TEdge>(vertex, predecessors);
                    foreach (TEdge edge in vertex.GetEdges())
                        if (!visited.Contains(edge.To))
                        {
                            float g = frontierVerticesCosts[vertex] + GetEdgeWeight(vertex, edge, weightComputingUserModel, searchableGraph.Scale);
                            if (g is float.NaN)
                            {
                                Console.WriteLine();
                            }
                            float f = g + aStarHeuristicsComputingUserModel.GetWeightFromHeuristics(edge.To.Attributes, finishVertex.Attributes, searchableGraph.Scale);
                            if (frontier.Contains(edge.To))
                                if (frontier.GetPriority(edge.To) < f) continue;
                                else frontier.UpdatePriority(edge.To, f);
                            else frontier.Enqueue(edge.To, f);
                            frontierVerticesCosts[edge.To] = g;
                            predecessors[edge.To] = vertex;
                        }
                    frontierVerticesCosts.Remove(vertex);
                }
                return new SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>();
            }
            throw new ArgumentException("User model did not provide required functionality.");
        }

        private float GetEdgeWeight<TVertex, TEdge>(TVertex vertex, TEdge edge, IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> weightComputingUserModel, int mapScale)
            where TVertex : IBasicVertex<TEdge, TVertexAttributes>
            where TEdge : IBasicEdge<TVertex, TEdgeAttributes, float>
        {
            if (edge.GetWeight() is not float.NaN)
                return edge.GetWeight(); 
            var computedWeight = weightComputingUserModel.ComputeWeight(vertex.Attributes, edge.Attributes, edge.To.Attributes, mapScale);
            edge.SetWeight(computedWeight);
            return computedWeight;
        }

        private SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> GetPathToFinish<TVertex, TEdge >(TVertex lastVertex, Dictionary<TVertex, TVertex> predecessors)
            where TVertex : IBasicVertex<TEdge, TVertexAttributes>
            where TEdge : IBasicEdge<TVertex, TEdgeAttributes, float>
        {
            List<TVertexAttributes> reversedPathCoordinates = new();
            reversedPathCoordinates.Add(lastVertex.Attributes);
            while(predecessors.TryGetValue(lastVertex, out lastVertex))
                reversedPathCoordinates.Add(lastVertex.Attributes);
            reversedPathCoordinates.Reverse();
            return new SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>(reversedPathCoordinates);
        }
    }
}