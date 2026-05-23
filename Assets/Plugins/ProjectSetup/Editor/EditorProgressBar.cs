using System;
using UnityEditor;

internal class EditorProgressBar : IProgress<float>, IDisposable
{
	public readonly string title;
	public string info;

	public EditorProgressBar() : this(string.Empty, string.Empty) { }
	public EditorProgressBar(string title) : this(title, string.Empty) { }
	public EditorProgressBar(string title, string info)
	{
		this.title = title;
		this.info = info;
	}

	public void Report(float value)
	{
		EditorUtility.DisplayProgressBar(title, info, value);
	}

	public void Dispose()
	{
		EditorUtility.ClearProgressBar();
	}
}
