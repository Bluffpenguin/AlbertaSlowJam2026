using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[Flags]
internal enum AssemblyPlatform
{
	[InspectorName("Default (Any Platform)")]
	Default = 0,
	[InspectorName("Android™")]
	Android = 1 << 0,
	Editor = 1 << 1,
	EmbeddedLinux = 1 << 2,
	[InspectorName("iOS")]
	iOS = 1 << 3,
	Kepler = 1 << 4,
	LinuxStandalone64 = 1 << 5,
	CloudRendering = 1 << 6,
	macOSStandalone = 1 << 7,
	[InspectorName("Nintendo Switch 2")]
	Switch2 = 1 << 8,
	[InspectorName("Nintendo Switch™")]
	Switch = 1 << 9,
	[InspectorName("PlayStation®4")]
	PS4 = 1 << 10,
	[InspectorName("PlayStation®5")]
	PS5 = 1 << 11,
	[InspectorName("QNX®")]
	QNX = 1 << 12,
	[InspectorName("tvOS")]
	tvOS = 1 << 13,
	[InspectorName("Universal Windows Platform")]
	WSA = 1 << 14,
	[InspectorName("visionOS")]
	VisionOS = 1 << 15,
	WebGL = 1 << 16,
	[InspectorName("Windows 32-bit")]
	WindowsStandalone32 = 1 << 17,
	[InspectorName("Windows 64-bit")]
	WindowsStandalone64 = 1 << 18,
	[InspectorName("Windows Dedicated Server 32-bit")]
	WindowsStandalone32Server = 1 << 19,
	[InspectorName("Windows Dedicated Server 64-bit")]
	WindowsStandalone64Server = 1 << 20,
	[InspectorName("Xbox One")]
	GameCoreXboxOne = 1 << 21,
	[InspectorName("Xbox One (Legacy XDK)")]
	XboxOne = 1 << 22,
	[InspectorName("Xbox Series X|S")]
	GameCoreScarlett = 1 << 23
}

[Flags]
internal enum PrecompiledAssemblies
{
	None = 0,
	[InspectorName("JetBrains.Rider.PathLocator.dll")] JetBrains_Rider_PathLocator_dll = 1 << 0,
	[InspectorName("log4netPlastic.dll")] log4netPlastic_dll = 1 << 1,
	[InspectorName("Mono.Cecil.dll")] Mono_Cecil_dll = 1 << 2,
	[InspectorName("Mono.Cecil.Mdb.dll")] Mono_Cecil_Mdb_dll = 1 << 3,
	[InspectorName("Mono.Cecil.Pdb.dll")] Mono_Cecil_Pdb_dll = 1 << 4,
	[InspectorName("Mono.Cecil.Rocks.dll")] Mono_Cecil_Rocks_dll = 1 << 5,
	[InspectorName("nunit.framework.dll")] nunit_framework_dll = 1 << 6,
	[InspectorName("System.IO.Hashing.dll")] System_IO_Hashing_dll = 1 << 7,
	[InspectorName("System.Runtime.CompilerServices.Unsafe.dll")] System_Runtime_CompilerServices_Unsafe_dll = 1 << 8,
	[InspectorName("Unity.Burst.Cecil.dll")] Unity_Burst_Cecil_dll = 1 << 9,
	[InspectorName("Unity.Burst.Cecil.Mdb.dll")] Unity_Burst_Cecil_Mdb_dll = 1 << 10,
	[InspectorName("Unity.Burst.Cecil.Pdb.dll")] Unity_Burst_Cecil_Pdb_dll = 1 << 11,
	[InspectorName("Unity.Burst.Cecil.Rocks.dll")] Unity_Burst_Cecil_Rocks_dll = 1 << 12,
	[InspectorName("Unity.Burst.Unsafe.dll")] Unity_Burst_Unsafe_dll = 1 << 13,
	[InspectorName("Unity.Collections.LowLevel.ILSupport.dll")] Unity_Collections_LowLevel_ILSupport_dll = 1 << 14,
	[InspectorName("Unity.Plastic.Antlr3.Runtime.dll")] Unity_Plastic_Antlr3_Runtime_dll = 1 << 15,
	[InspectorName("Unity.Plastic.Newtonsoft.Json.dll")] Unity_Plastic_Newtonsoft_Json_dll = 1 << 16,
	[InspectorName("Unity.VisualScripting.Antlr3.Runtime.dll")] Unity_VisualScripting_Antlr3_Runtime_dll = 1 << 17,
	[InspectorName("Unity.VisualScripting.IonicZip.dll")] Unity_VisualScripting_IonicZip_dll = 1 << 18,
	[InspectorName("Unity.VisualScripting.TextureAssets.dll")] Unity_VisualScripting_TextureAssets_dll = 1 << 19,
	[InspectorName("Unity.VisualScripting.YamlDotNet.dll")] Unity_VisualScripting_YamlDotNet_dll = 1 << 20,
	[InspectorName("unityplastic.dll")] unityplastic_dll = 1 << 21
}

[Serializable]
internal class CreateAssemblyOptions
{
	public string path = Application.dataPath;
	public string assemblyName;
	public bool allowUnsafe;
	public bool autoReferenced = true;
	public bool noEngineRefs;
	public bool overrideReferences;
	public string defaultNamespace;
	public AssemblyDefinitionAsset[] assemblyReferences = Array.Empty<AssemblyDefinitionAsset>();
	public PrecompiledAssemblies precompiledAssemblies;
	public AssemblyPlatform platforms;
	public AssemblyDefinitionAsset[] internalAssemblies = Array.Empty<AssemblyDefinitionAsset>();
	public bool useCSharp10;
	public bool nullable;
	public string[] globalUsings = new string[] {
		"System",
		"System.Collections",
		"System.Collections.Generic",
		"UnityEngine",
		"Object = UnityEngine.Object",
		"Random = UnityEngine.Random",
	};
}

internal static class AssemblyFactory
{
	private static readonly System.Text.RegularExpressions.Regex rgx_assemblyName = new("\"name\": (\".*\")");

	public static bool Validate(CreateAssemblyOptions options, out string errors)
	{
		var path = options.path;
		var assemblyName = options.assemblyName;
		var assemblyReferences = options.assemblyReferences;
		var errorsBuilder = new StringBuilder();

		if (string.IsNullOrWhiteSpace(path))
			errorsBuilder.AppendLine("Directory is required.");
		else if (!path.StartsWith(Application.dataPath))
			errorsBuilder.AppendLine("Directory must be a valid asset folder.");
		else if (!Directory.Exists(path))
			errorsBuilder.AppendLine("Directory does not exist.");

		if (string.IsNullOrWhiteSpace(assemblyName))
			errorsBuilder.AppendLine("Assembly Name is required.");

		if (assemblyReferences.Any(static x => x == null))
			errorsBuilder.AppendLine("Some assembly references have not been defined.");

		errors = errorsBuilder.ToString();
		return string.IsNullOrEmpty(errors);
	}

	public static async Task Generate(CreateAssemblyOptions options, IProgress<float> progress)
	{
		if (!Validate(options, out var errors)) {
			throw new ArgumentException(errors, nameof(options));
		}

		string subDirectory = Path.Combine(options.assemblyName.Split('.'));
		string directory = Path.Combine(options.path, subDirectory);
		progress?.Report(0);

		await GenerateAssemblyDefinitionFile(
			directory,
			options.assemblyName,
			options.allowUnsafe,
			options.overrideReferences,
			options.autoReferenced,
			options.noEngineRefs,
			options.defaultNamespace,
			options.assemblyReferences,
			options.platforms,
			options.precompiledAssemblies);
		progress?.Report(1 / 3f);

		await GenerateCsc(directory, options.nullable, options.useCSharp10);
		progress?.Report(2 / 3f);

		await GenerateAssemblyInfoClass(directory,
			options.useCSharp10,
			options.globalUsings,
			options.internalAssemblies);
		progress?.Report(1f);
	}

	private static async Task GenerateAssemblyInfoClass(string directory, bool useCSharp10, string[] globalUsings, AssemblyDefinitionAsset[] internalAssemblies)
	{
		const string
			fmt_globalUsing = "global using {0};",
			replaceAssemblyAttribute = "[assembly: System.Runtime.CompilerServices.InternalsVisibleTo($1)]";

		string filePath = Path.Combine(directory, "AssemblyInfo.cs");
		var sr = File.CreateText(filePath);

		try {
			if (useCSharp10 && globalUsings.Length > 0) {
				await sr.WriteLineAsync("// Global Usings");
				foreach (string v in globalUsings.Distinct()) {
					await sr.WriteLineAsync(string.Format(fmt_globalUsing, v));
				}
			}

			var refs = internalAssemblies;
			if (refs.Length > 0) {
				await sr.WriteLineAsync("// Internal Assemblies");
				for (int i = 0; i < refs.Length; i++) {
					var result = rgx_assemblyName.Match(refs[i].text).Result(replaceAssemblyAttribute);
					await sr.WriteLineAsync(result);
				}
			}

			await sr.WriteLineAsync("// Enables use of the 'init' keyword and primary constructors for records.");
			await sr.WriteLineAsync("namespace System.Runtime.CompilerServices");
			sr.WriteLine('{');
			await sr.WriteLineAsync("    internal sealed class IsExternalInit { }");
			sr.WriteLine('}');
		}
		finally {
			sr.Close();
			sr.Dispose();
		}
	}

	private static async Task GenerateCsc(string directory, bool nullable, bool useCSharp10)
	{
		if (!nullable && !useCSharp10)
			return;

		string filePath = Path.Combine(directory, "csc.rsp");
		var sr = File.CreateText(filePath);
		try {
			if (nullable) {
				await sr.WriteLineAsync("-nullable:enable");
			}
			if (useCSharp10) {
				await sr.WriteLineAsync("-langversion:10");
			}
		}
		finally {
			sr.Close();
			sr.Dispose();
		}
	}

	private static async Task GenerateAssemblyDefinitionFile(string directory, string assemblyName, bool allowUnsafe, bool overrideReferences, bool autoReferenced, bool noEngineRefs, string defaultNamespace, IEnumerable<AssemblyDefinitionAsset> references, AssemblyPlatform platforms, PrecompiledAssemblies precompiledAssemblies)
	{
		Debug.Assert(!string.IsNullOrWhiteSpace(assemblyName));
		const string extension = ".asmdef";

		string filePath = Path.Combine(directory, assemblyName + extension);

		var dir = Path.GetDirectoryName(filePath);

		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);

		string contents = GenerateAssemblyDefinitionText(assemblyName, allowUnsafe, overrideReferences, autoReferenced, noEngineRefs, defaultNamespace, references, platforms, precompiledAssemblies);

		await File.WriteAllTextAsync(filePath, contents);
	}

	private static string GenerateAssemblyDefinitionText(string assemblyName, bool allowUnsafe, bool overrideReferences, bool autoReferenced, bool noEngineRefs, string defaultNamespace, IEnumerable<AssemblyDefinitionAsset> references, AssemblyPlatform platforms, PrecompiledAssemblies precompiledAssemblies)
	{
		var writer = new JsonWriter();

		writer.AppendValue("name", assemblyName);
		writer.AppendValue("rootNamespace", defaultNamespace);

		writer.BeginArray("references");
		foreach (var reference in references) {
			object name = rgx_assemblyName.Match(reference.text).Result("$1");
			writer.AppendValue(name);
		}
		writer.EndArray();

		writer.BeginArray("includePlatforms");
		if (platforms is not AssemblyPlatform.Default) {
			for (int n = 0; n < 24; n++) {
				var flag = (AssemblyPlatform)(1 << n);
				if (platforms.HasFlag(flag)) {
					writer.AppendValue(flag.ToString());
				}
			}
		}
		writer.EndArray();
		writer.BeginArray("excludePlatforms").EndArray();

		writer.AppendValue("allowUnsafeCode", allowUnsafe);
		writer.AppendValue("overrideReferences", overrideReferences);

		writer.BeginArray("precompiledReferences");
		if (overrideReferences && precompiledAssemblies is not PrecompiledAssemblies.None) {
			for (int n = 0; n < 22; n++) {
				var flag = (PrecompiledAssemblies)(1 << n);
				if (precompiledAssemblies.HasFlag(flag)) {
					writer.AppendValue(flag.ToString().Replace('_', '.'));
				}
			}
		}
		writer.EndArray();

		writer.AppendValue("autoReferenced", autoReferenced);

		writer.BeginArray("defineConstraints").EndArray();
		writer.BeginArray("versionDefines").EndArray();

		writer.AppendValue("noEngineReferences", noEngineRefs);

		return writer.ToString();
	}
}

internal class JsonWriter
{
	private readonly StringBuilder m_stringBuilder;
	private int m_depth = 0;

	public char indentChar { get; set; } = ' ';
	public int indentWidth { get; set; } = 4;

	public JsonWriter() : this(new StringBuilder()) { }
	public JsonWriter(int capacity) : this(new StringBuilder(capacity)) { }
	public JsonWriter(StringBuilder stringBuilder)
	{
		m_stringBuilder = stringBuilder;

		m_stringBuilder.AppendLine("{");
		m_depth = 1;
	}

	public JsonWriter AppendValue(bool value) => AppendValue(null, value);
	public JsonWriter AppendValue(string name, bool value)
		=> AppendValue_Internal(name, value.ToString().ToLowerInvariant());
	public JsonWriter AppendValue(int value) => AppendValue(null, value);
	public JsonWriter AppendValue(string name, int value)
		=> AppendValue_Internal(name, value.ToString());
	public JsonWriter AppendValue(float value) => AppendValue(null, value);
	public JsonWriter AppendValue(string name, float value)
		=> AppendValue_Internal(name, value.ToString());
	public JsonWriter AppendValue(string value) => AppendValue(null, value);
	public JsonWriter AppendValue(string name, string value)
		=> AppendValue_Internal(name, $"\"{value}\"");
	public JsonWriter AppendValue(object value) => AppendValue(null, value);
	public JsonWriter AppendValue(string name, object value)
		=> AppendValue_Internal(name, value.ToString());

	private JsonWriter AppendValue_Internal(string name, string valueText)
	{
		string text = string.IsNullOrEmpty(name) ? valueText + ',' : $"\"{name}\": {valueText},";
		return AppendLine(text);
	}

	public JsonWriter BeginArray(string name)
	{
		AppendLine($"\"{name}\": [");
		m_depth++;
		return this;
	}

	public JsonWriter EndArray()
	{
		m_depth--;
		return AppendLine("],");
	}

	public JsonWriter BeginObject(string name)
	{
		AppendLine($"\"{name}\": {{");
		m_depth++;
		return this;
	}

	public JsonWriter EndObject()
	{
		m_depth--;
		return AppendLine("},");
	}

	private JsonWriter AppendLine(string line)
	{
		int totalWidth = m_depth * indentWidth + line.Length;
		m_stringBuilder.AppendLine(line.PadLeft(totalWidth, indentChar));
		return this;
	}

	public override string ToString()
	{
		m_stringBuilder.AppendLine("}");
		var result = m_stringBuilder.ToString();
		// remove trailing commas b/c json hates em
		var pattern = new System.Text.RegularExpressions.Regex(@",(?=\s*[}\]])"); // this isn't perfect but will do.
		result = pattern.Replace(result, string.Empty);
		return result;
	}
}
