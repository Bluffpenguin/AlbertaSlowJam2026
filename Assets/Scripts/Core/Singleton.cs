using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
	public static Singleton Global { get; private set; }

	private void Awake()
	{
		if (Global == null)
		{
			InitiateGlobal();
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	private void InitiateGlobal()
	{
		Global = this;
		DontDestroyOnLoad(this.gameObject);
	}
}
