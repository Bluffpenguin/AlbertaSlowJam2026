using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerItemDrop : MonoBehaviour
{

    [SerializeField] Tilemap _tileMap;
    public static PlayerItemDrop Instance;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		Instance = this;
		this.transform.SetParent(null);
		DontDestroyOnLoad(this.gameObject);

	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public bool DropItem(Vector3 pos, ItemStack item)
	{
		Vector3Int tilePos = _tileMap.WorldToCell(pos);
		if (item.Count > 0 && _tileMap.GetInstantiatedObject(tilePos) == null)
		{
			if (item.Data.ItemTile != null)
			{
				_tileMap.SetTile(tilePos, item.Data.ItemTile);
				return true;
			}
			else return false;
			
		}
		return false;
	}
}
