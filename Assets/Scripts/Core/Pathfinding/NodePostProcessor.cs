using UnityEngine;

public class NodePostProcessor : MonoBehaviour, IRoomPostProcessor
{
    [SerializeField] private TilePainter _nodePainter;
    [SerializeField] private PathfindingManager _manager;

	public void ProcessRoom(RoomInfo room)
	{
        _nodePainter.PaintTiles(room.Tiles);

	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
