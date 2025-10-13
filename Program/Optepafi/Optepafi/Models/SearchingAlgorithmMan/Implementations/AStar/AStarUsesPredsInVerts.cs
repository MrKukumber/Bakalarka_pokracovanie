using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.Controls.Primitives;
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
using Optepafi.Models.Utils;
using Optepafi.Models.Utils.Configurations;
using Optepafi.Models.Utils.Shapes.Segments;
using Priority_Queue;

namespace Optepafi.Models.SearchingAlgorithmMan.Implementations.AStar;

/// <summary>
/// General implementation of A* algorithm.
/// It does not use predecessor list for remembering found paths, it requires from vertices of map representation to be
/// able to remember their pre own predecessors.
/// At this moment, it does not report progress of search. Also, it does not use any configuration of its behaviour.
/// </summary>
public class AStarUsesPredsInVerts : 
    SearchingAlgorithmImplementation<AStarConfiguration>
{
    public static AStarUsesPredsInVerts Instance { get; } = new AStarUsesPredsInVerts();
    private AStarUsesPredsInVerts() {}
    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
    public override bool DoesRepresentUsableGraph<TVertex, TEdge, TVertexAttributes, TEdgeAttributes>(IGraphRepresentative<IGraph<TVertex, TEdge>, TVertex, TEdge> graphRepresentative) 
    {
        return graphRepresentative is IGraphRepresentative<IRemembersPredecessors<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>, TVertex, TEdge>;
    }

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
    public override bool DoesRepresentUsableUserModel<TTemplate, TVertexAttributes, TEdgeAttributes>(IUserModelType<IComputing<TTemplate, TVertexAttributes, TEdgeAttributes>, TTemplate> userModelType) 
    {
        return userModelType 
            is IUserModelType<IAStarHeuristicsComputing<TTemplate, TVertexAttributes, TEdgeAttributes> , TTemplate> 
            and IUserModelType<IWeightComputing<TTemplate, TVertexAttributes, TEdgeAttributes>, TTemplate> ;
    }

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
    public override bool IsUsableGraph<TVertex, TEdge, TVertexAttributes, TEdgeAttributes>(IGraph<TVertex, TEdge> graph) 
    {
        return graph is IRemembersPredecessors<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>;
    }

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
    public override bool IsUsableUserModel<TVertexAttributes, TEdgeAttributes>(IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel) 
    {
        return userModel 
            is IAStarHeuristicsComputing<ITemplate<TVertexAttributes,TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> 
            and IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes>;
    }

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
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

    /// <inheritdoc cref="SearchingAlgorithmImplementation{TConfiguration}"/> 
    protected override IPath<TVertexAttributes, TEdgeAttributes> ExecutorSearch<TVertexAttributes, TEdgeAttributes, TVertex, TEdge>(
        Leg[] track, IGraph<TVertex, TEdge> graph,
        IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel, AStarConfiguration configuration,
        IProgress<ISearchingReport>? progress, CancellationToken? cancellationToken)
        => Algorithm<TVertexAttributes, TEdgeAttributes>.Instance.Search(track, graph, userModel, configuration, progress, cancellationToken); 

    private class Algorithm<TVertexAttributes, TEdgeAttributes> :
        IFromRemembersPredecessorsToPredecessorRememberingTraverser<SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>, float, TVertexAttributes, TEdgeAttributes, (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken? )> 
        where TVertexAttributes : IVertexAttributes
        where TEdgeAttributes : IEdgeAttributes
    {
        public static Algorithm<TVertexAttributes, TEdgeAttributes> Instance { get; } = new();
        private Algorithm() { }
        private const int CancellationCheckDensity = 5000;
        
        public IPath<TVertexAttributes, TEdgeAttributes> Search<TVertex, TEdge>(Leg[] track, IGraph<TVertex, TEdge> graph, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>> userModel, AStarConfiguration configuration, IProgress<ISearchingReport>? progress, CancellationToken? cancellationToken)
        where TVertex : IVertex
        where TEdge : IEdge
        {
            if (graph is IRemembersPredecessors<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes>
                graphWhichRemembersPredecessors)
            {
                SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> path = new();
                foreach (var leg in track)
                {
                    if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested) return path;
                    path.MergeWith(graphWhichRemembersPredecessors.Traverse(this, (leg, userModel, configuration, progress, cancellationToken)));
                }
                return path;
            }
            throw new ArgumentException("Graph was not convertible to IRemembersPredecessorsGenericVisitor even though it should be checked before call of this method.");
        }

        SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> IFromRemembersPredecessorsToPredecessorRememberingTraverser<SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>, float, TVertexAttributes, TEdgeAttributes, (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken? )>.
            AcceptedTraverse<TVertex, TEdge>(IPredecessorRemembering<TVertex, TEdge, float, TVertexAttributes, TEdgeAttributes> predecessorRememberingGraph,
            (Leg, IUserModel<ITemplate<TVertexAttributes, TEdgeAttributes>>, AStarConfiguration, IProgress<ISearchingReport>?, CancellationToken?) otherParams)
        {
            var (leg, userModel, configuration, progress, cancellationToken) = otherParams;
            if (userModel is IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> weightComputingUserModel
                && userModel is IAStarHeuristicsComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> aStarHeuristicsComputingUserModel)
            {
                TVertex startVertex = predecessorRememberingGraph.GetVertexFor(leg.Start);
                TVertex finishVertex = predecessorRememberingGraph.GetVertexFor(leg.Finish);
                SimplePriorityQueue<TVertex, float> frontier = new();
                Dictionary<TVertex, float> frontierVerticesCosts = new();
                HashSet<TVertex> visited = new();
                frontier.Enqueue(startVertex, aStarHeuristicsComputingUserModel.GetWeightFromHeuristics(startVertex.Attributes, finishVertex.Attributes, predecessorRememberingGraph.Scale) + 0);
                frontierVerticesCosts.Add(startVertex, 0);
                int timeSinceLastCancellationCheck = 0;
                while (frontier.TryDequeue(out TVertex vertex))
                {
                    if (++timeSinceLastCancellationCheck >= CancellationCheckDensity && cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
                        break;
                    visited.Add(vertex);
                    if (vertex.Equals(finishVertex))
                        return GetPathToFinish(vertex);
                    foreach (TEdge edge in vertex.GetEdges())
                        if (!visited.Contains(edge.To))
                        {
                            float g = frontierVerticesCosts[vertex] + GetEdgeWeight(vertex, edge, weightComputingUserModel, predecessorRememberingGraph.Scale);
                            float f = g + aStarHeuristicsComputingUserModel.GetWeightFromHeuristics(edge.To.Attributes, finishVertex.Attributes, predecessorRememberingGraph.Scale);
                            if (frontier.Contains(edge.To))
                                if (frontier.GetPriority(edge.To) < f) continue;
                                else frontier.UpdatePriority(edge.To, f);
                            else frontier.Enqueue(edge.To, f);
                            frontierVerticesCosts[edge.To] = g;
                            edge.To.Predecessor = vertex;
                        }
                    frontierVerticesCosts.Remove(vertex);
                }
                return new SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>();
            }
            throw new ArgumentException("User model does not provided required functionality.");
        }

        private float GetEdgeWeight<TVertex, TEdge>(TVertex vertex, TEdge edge, IWeightComputing<ITemplate<TVertexAttributes, TEdgeAttributes>, TVertexAttributes, TEdgeAttributes> weightComputingUserModel, int mapScale)
            where TVertex : IPredecessorRememberingVertex<TVertexAttributes>, IBasicVertex<TEdge, TVertexAttributes>
            where TEdge : IBasicEdge<TVertex, TEdgeAttributes, float>
        {
            if (edge.GetWeight() is not float.NaN)
                return edge.GetWeight(); 
            var computedWeight = weightComputingUserModel.ComputeWeight(vertex.Attributes, edge.Attributes, edge.To.Attributes, mapScale);
            edge.SetWeight(computedWeight);
            return computedWeight;
        }

        private SegmentedLinesPath<TVertexAttributes, TEdgeAttributes> GetPathToFinish(IPredecessorRememberingVertex<TVertexAttributes>? lastVertex)
        {
            List<TVertexAttributes> reversedPathCoordinates = new();
            while (lastVertex != null)
            {
                reversedPathCoordinates.Add(lastVertex.Attributes);
                lastVertex = lastVertex.Predecessor;
            }
            reversedPathCoordinates.Reverse();
            return new SegmentedLinesPath<TVertexAttributes, TEdgeAttributes>(reversedPathCoordinates);
        }
    }
}