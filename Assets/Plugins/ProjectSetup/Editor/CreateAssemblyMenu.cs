using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

internal class CreateAssemblyMenu : EditorWindow
{
	internal const string menuName = "Create Assembly";

	const float spacing = 15;

	internal const string
		name_directoryContainer = "container-directory",
		name_directoryField = "field-directory",
		name_directorySelectButton = "button-select-directory",

		name_assemblyNameField = "field-assembly-name",
		name_allowUnsafeField = "field-allow-unsafe",
		name_autoReferencedField = "field-auto-referenced",
		name_noEngineRefsField = "field-no-engine-references",
		name_overrideReferencesField = "field-override-references",
		name_namespaceField = "field-default-namespace",

		name_referencesField = "field-assembly-references",
		name_precompiledAssembliesField = "field-precompiled-assemblies",
		name_platformField = "field-platform",

		name_nullableField = "field-nullable",
		name_enableCS10Field = "field-enable-csharp-10",
		name_globalUsingsList = "field-global-usings",
		name_internalAssembliesList = "field-internal-assemblies",

		name_errorList = "info-errors",

		name_buttonContainer = "container-buttons",
		name_createButton = "button-confirm",
		name_cancelButton = "button-cancel";

	internal const string
		prop_options = nameof(options) + ".",
		prop_path = prop_options + nameof(CreateAssemblyOptions.path),
		prop_assemblyName = prop_options + nameof(CreateAssemblyOptions.assemblyName),
		prop_allowUnsafe = prop_options + nameof(CreateAssemblyOptions.allowUnsafe),
		prop_autoReferenced = prop_options + nameof(CreateAssemblyOptions.autoReferenced),
		prop_noEngineRefs = prop_options + nameof(CreateAssemblyOptions.noEngineRefs),
		prop_overrideReferences = prop_options + nameof(CreateAssemblyOptions.overrideReferences),
		prop_defaultNamespace = prop_options + nameof(CreateAssemblyOptions.defaultNamespace),
		prop_assemblyReferences = prop_options + nameof(CreateAssemblyOptions.assemblyReferences),
		prop_precompiledAssemblies = prop_options + nameof(CreateAssemblyOptions.precompiledAssemblies),
		prop_platforms = prop_options + nameof(CreateAssemblyOptions.platforms),
		prop_internalAssemblies = prop_options + nameof(CreateAssemblyOptions.internalAssemblies),
		prop_useCSharp10 = prop_options + nameof(CreateAssemblyOptions.useCSharp10),
		prop_nullable = prop_options + nameof(CreateAssemblyOptions.nullable),
		prop_globalUsings = prop_options + nameof(CreateAssemblyOptions.globalUsings);

	public CreateAssemblyOptions options = new();

	[MenuItem("Tools/Project Setup/" + menuName + "...")]
	public static void ShowExample()
	{
		_ = GetWindow<CreateAssemblyMenu>(utility: true, title: menuName, focus: true);
	}

	public void CreateGUI()
	{
		VisualElement root = rootVisualElement;

		var contentContainer = new ScrollView(ScrollViewMode.Vertical) {
			style = {
				flexShrink = 1,
			}
		};
		root.Add(contentContainer);

		root = contentContainer;

		var directoryContainer = new VisualElement() {
			name = name_directoryContainer,
			style = {
				flexDirection = FlexDirection.Row,
				width = Length.Percent(100f),
			},
		};
		root.Add(directoryContainer);

		var directoryField = new TextField() {
			name = name_directoryField,
			label = "Directory",
			tooltip = "The location of the assembly folder.",
			bindingPath = prop_path,
			enabledSelf = false,
			style = {
				flexGrow = 1,
				flexShrink = 1,
			}
		};
		directoryField.AddToClassList(TextField.alignedFieldUssClassName);
		directoryContainer.Add(directoryField);

		var selectDirectoryButton = new Button() {
			name = name_directorySelectButton,
			text = "Select Folder...",
		};
		directoryContainer.Add(selectDirectoryButton);

		selectDirectoryButton.clicked += delegate {
			string newValue = EditorUtility.OpenFolderPanel(selectDirectoryButton.text, options.path, string.Empty);
			if (!string.IsNullOrEmpty(newValue))
				directoryField.value = newValue;
		};

		var nameField = new TextField() {
			name = name_assemblyNameField,
			label = "Assembly Name",
			tooltip = "The assembly name is used to create a .dll file on your disk.",
			bindingPath = prop_assemblyName,
		};
		nameField.AddToClassList(TextField.alignedFieldUssClassName);
		root.Add(nameField);

		var generalOptionsContainer = new Foldout() {
			text = "General Options",
			value = true,
			style = {
				marginTop = Length.Pixels(spacing),
			}
		};
		root.Add(generalOptionsContainer);

		root = generalOptionsContainer;

		var allowUnsafeField = new Toggle() {
			name = name_allowUnsafeField,
			label = "Allow 'unsafe' Code",
			tooltip = "When enabled, the C# compiler for this assembly includes types or members that have the 'unsafe' keyword.",
			bindingPath = prop_allowUnsafe,
		};
		allowUnsafeField.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(allowUnsafeField);

		var autoReferencedField = new Toggle() {
			name = name_autoReferencedField,
			label = "Auto Referenced",
			tooltip = "When enabled, this assembly definition is automatically referenced in predefined assemblies.",
			bindingPath = prop_autoReferenced,
		};
		autoReferencedField.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(autoReferencedField);

		var noEngineRefsField = new Toggle() {
			name = name_noEngineRefsField,
			label = "No Engine References",
			tooltip = "When enabled, references to UnityEngine/UnityEditor will not be added when compiling this assembly.",
			bindingPath = prop_noEngineRefs,
		};
		noEngineRefsField.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(noEngineRefsField);

		var overrideReferencesField = new Toggle() {
			name = name_overrideReferencesField,
			label = "Override References",
			tooltip = "When enabled, you can select which specific precompiled assemblies to reefer to via a dropdown list that appears. When not enabled, this assembly definition refers to all auto-referenced precompiled assemblies.",
			bindingPath = prop_overrideReferences,
		};
		overrideReferencesField.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(overrideReferencesField);

		var namespaceField = new TextField() {
			name = name_namespaceField,
			label = "Default Namespace",
			tooltip = "Specify the root namespace of the assembly.",
			bindingPath = prop_defaultNamespace,
		};
		namespaceField.AddToClassList(TextField.alignedFieldUssClassName);
		root.Add(namespaceField);

		root = contentContainer;

		var referencesField = new PropertyField() {
			name = name_referencesField,
			label = "Assembly Definition References",
			tooltip = "The list of assembly files that this assembly definition should reference.",
			bindingPath = prop_assemblyReferences,
			style = {
				marginTop = Length.Pixels(spacing),
			}
		};
		root.Add(referencesField);

		var precompiledAssembliesField = new PropertyField() {
			name = name_precompiledAssembliesField,
			label = "Assembly References",
			tooltip = "The list of precompiled assemblies that this assembly definition should reference.",
			bindingPath = prop_precompiledAssemblies,
			style = {
				display = options.overrideReferences ? DisplayStyle.Flex : DisplayStyle.None,
				marginTop = Length.Pixels(spacing),
			}
		};
		root.Add(precompiledAssembliesField);

		overrideReferencesField.RegisterValueChangedCallback(evt => {
			precompiledAssembliesField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
		});

		root = contentContainer;

		var platformField = new PropertyField() {
			name = name_platformField,
			tooltip = "Select which platforms include this assembly when built.",
			bindingPath = prop_platforms,
			style = {
				marginTop = Length.Pixels(spacing),
			}
		};
		root.Add(platformField);

		var foldout = new Foldout() {
			text = "More Options",
			value = false,
			style = {
				marginTop = Length.Pixels(spacing),
			}
		};
		root.Add(foldout);

		root = foldout;

		var nullableField = new Toggle() {
			name = name_nullableField,
			label = "Nullability",
			tooltip = "When enabled, reference types must explicitly be declared as nullable.",
			bindingPath = prop_nullable,
		};
		nullableField.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(nullableField);

		var enableCS10Field = new Toggle() {
			name = name_enableCS10Field,
			label = "Enable C#10",
			tooltip = "When enabled, this assembly allows use of features added in C#10, such as global usings and record structs.",
			bindingPath = prop_useCSharp10,
		};
		enableCS10Field.AddToClassList(Toggle.alignedFieldUssClassName);
		root.Add(enableCS10Field);

		var globalUsingsField = new PropertyField() {
			name = name_globalUsingsList,
			tooltip = "List of global usings for this assembly.",
			bindingPath = prop_globalUsings
		};
		root.Add(globalUsingsField);

		enableCS10Field.RegisterValueChangedCallback(evt => {
			globalUsingsField.style.display = evt.newValue
				? DisplayStyle.Flex : DisplayStyle.None;
		});

		var internalAssembliesField = new PropertyField() {
			name = name_internalAssembliesList,
			tooltip = "List of assemblies with special access to internal types and members of this assembly.",
			bindingPath = prop_internalAssemblies
		};
		root.Add(internalAssembliesField);

		root = rootVisualElement;

		var spacer = new VisualElement() {
			style = {
				flexGrow = 1,
			}
		};
		root.Add(spacer);

		// footer
		var buttonContainer = new VisualElement() {
			name = name_buttonContainer,
			style = {
				flexDirection = FlexDirection.RowReverse,
				alignItems = Align.FlexEnd,
				minHeight = Length.Pixels(21),
			}
		};
		root.Add(buttonContainer);

		var confirmButton = new Button(this.Generate) {
			name = name_createButton,
			text = "Create",
		};
		buttonContainer.Add(confirmButton);

		var cancelButton = new Button(this.Close) {
			name = name_cancelButton,
			text = "Cancel",
		};
		buttonContainer.Add(cancelButton);

		root.Bind(new SerializedObject(this));
	}

	private async void Generate()
	{
		if (!AssemblyFactory.Validate(options, out var errors)) {
			EditorApplication.Beep();
			EditorUtility.DisplayDialog(menuName, errors, "OK");
		} else {

			using var progress = new EditorProgressBar(menuName, "Creating Assembly...");
			await AssemblyFactory.Generate(options, progress);
			Close();
			AssetDatabase.Refresh();
		}
	}
}
