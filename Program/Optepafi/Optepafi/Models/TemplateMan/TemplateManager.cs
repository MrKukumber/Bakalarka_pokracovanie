using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Controls.Shapes;
using HarfBuzzSharp;
using Optepafi.Models.TemplateMan.Templates;
using Path = System.IO.Path;

namespace Optepafi.Models.TemplateMan;

/// <summary>
/// Singleton class that manages and provides usable template instances.
///
/// It represents main channel between operations on templates and applications logic (ModelView/ViewModel).
/// </summary>
public class TemplateManager
{
    public static TemplateManager Instance { get; } = new();
    private TemplateManager(){}
    public ISet<ITemplate> Templates { get; } = ImmutableHashSet.Create<ITemplate>(Orienteering_ISOM_2017_2_RunSpeModSym.Instance, BlankTemplate.Instance, NotUsableTemplate.Instance);

    // public void LoadPlugins()
    // {
        // var assemblies = Directory
            // .GetFiles(Path.Combine("Plugins", "Template"), "*.dll")
            // .Select(Assembly.LoadFrom)
            // .ToArray();
    // }
}