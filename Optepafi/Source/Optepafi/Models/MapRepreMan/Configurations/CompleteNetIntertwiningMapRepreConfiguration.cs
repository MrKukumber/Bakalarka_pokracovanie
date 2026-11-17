using System;
using System.Collections.Immutable;
using System.Linq;
using Optepafi.Models.Utils.Configurations;
using Optepafi.Views.Utils;

namespace Optepafi.Models.MapRepreMan.Configurations;

/// <summary>
/// Class representing object which holds configuration parameters for creating a graph which follows the architecture
/// defined by <code>ICompleteNetIntertwiningMapRepre</code> map representation.
/// These parameters can be set by user in order to affect how the graph will be constructed.
/// </summary>
/// <param name="defaultStandardEdgeLength">Length of an edge, which represents what length should the edges in a graph mostly have, or which length they should be close to/tend to. </param>
/// <param name="defaultMinBoundaryEdgeRatio">Describes how small edges do we want to create in the process of discretizing the edges of objects to a chain of vertices and edges. It is set as the ratio with respect to the standard edge length. Generally generated graph can have edges smaller than the length defined by this ratio, but these edges are produced when some edge, present in a created graph, is cut to smaller edges.</param>
/// <param name="indexOfDefaultNetType">Index of the type of net, which should be created at the start of a graph creation to cover the whole map space.</param>
public class CompleteNetIntertwiningMapRepreConfiguration(int defaultStandardEdgeLength, float defaultMinBoundaryEdgeRatio, int indexOfDefaultNetType) : IConfiguration
{
    public readonly BoundedIntValueConfigItem standardEdgeLength = new BoundedIntValueConfigItem("Standard edge length", defaultStandardEdgeLength, "micrometers", 500, 20_000);
    public readonly BoundedFloatValueConfigItem minBoundaryEdgeRatio = new BoundedFloatValueConfigItem("Ratio of minimal boundary edge length to standard one", defaultMinBoundaryEdgeRatio, "between 0.01 and 0.5", 0.01f,  0.5f);
    public readonly CategoricalConfigItem typeOfNet = new CategoricalConfigItem("Type of net", [NetTypesEnumeration.Triangular, NetTypesEnumeration.Grid, NetTypesEnumeration.ExtendedGrid], indexOfDefaultNetType);
    // public readonly CategoricalConfigItem<NetTypesEnumeration> typeOfNet = new CategoricalConfigItem<NetTypesEnumeration>("Type of net", NetTypesEnumeration.GetValuesAsUnderlyingType<NetTypesEnumeration>().OfType<NetTypesEnumeration>() .ToArray(), indexOfDefaultNetType);
    public ImmutableList<IConfigItem> ConfigItems => [standardEdgeLength, minBoundaryEdgeRatio, typeOfNet];
    public enum NetTypesEnumeration { Triangular = 0, Grid, ExtendedGrid }
    
    public IConfiguration DeepCopy()
    {
        return new CompleteNetIntertwiningMapRepreConfiguration(standardEdgeLength.Value, minBoundaryEdgeRatio.Value, typeOfNet.IndexOfSelectedValue);
    }
}