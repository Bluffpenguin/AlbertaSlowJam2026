using UnityEngine;
using UnityEngine.Tilemaps;

public class Obscurer : MonoBehaviour
{
    [SerializeField] Tilemap obstacleTilemap;
    [SerializeField] int range = 2;
    [SerializeField] List<Vector3Int> nearbyPositions;
    [SerializeField] Transform player;
    [SerializeField] float alphaTransparency;

    struct TileColor
    {
        public TileData tileData;
        public Color whole;
        public Color transparent;
    }
    Dictionary<Vector3Int, TileColor> tiles = new Dictionary<Vector3Int, TileColor>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Player.Controller.transform;
    }


	private void FixedUpdate()
	{
        if (obstacleTilemap && player != null)
        {
			CheckNearbyTilesAlt();
		}
		
	}


	void CheckNearbyTilesAlt()
	{
		Vector3Int playerPos = obstacleTilemap.WorldToCell(player.position);
		List<Vector3Int> newNearbyPositions = new();

        Color transparentColor = new Color(1.0f, 1.0f, 1.0f, alphaTransparency);

		for (int x = -range; x <= range; x++)
		{
			for (int y = -range; y <= range; y++)
			{
				Vector3Int pos = new Vector3Int(playerPos.x + x, playerPos.y + y);
                
				if (obstacleTilemap.HasTile(pos))
				{
					obstacleTilemap.SetTileFlags(pos, TileFlags.None);
					obstacleTilemap.SetColor(pos, transparentColor);
					newNearbyPositions.Add(pos);
				}

			}
		}

		foreach (Vector3Int pos in nearbyPositions)
        {
            if (!newNearbyPositions.Contains(pos))
            {

                // Make whole again
                obstacleTilemap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 1.0f));

		    }
        }

        foreach (Vector3Int pos in newNearbyPositions)
        {
			if (!nearbyPositions.Contains(pos))
			{

				// Make transparent
				obstacleTilemap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, alphaTransparency));

			}
		}

        nearbyPositions = newNearbyPositions;
    }
}
