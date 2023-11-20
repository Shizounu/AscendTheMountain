using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat;
using Commands;
public class BoardRenderer : MonoBehaviour
{
    [Header("References")]
    public TileVisualsManager[,] tiles;
    public Queue<IVisualCommand> visualCommands;

    [Header("Tile Scale")]
    public Vector2 tileScale;
    public Vector2 positionOffset = new Vector2(-4.5f, -2.5f);

    [Header("Prefabs")]
    public TileVisualsManager tilePrefab;
    private void Awake()
    {
        
    }

    private void Start() {
        InitializeBoard(GameManager.Instance.currentBoard.tiles);
        GameManager.Instance.onCommand += ProcessCommand;
    }

    public void ProcessCommand(ICommand command) {
        IVisualCommand visualCommand = Helpers.GetInterface<IVisualCommand>(command);
        if (visualCommand == null)
            return;
        visualCommands.Enqueue(visualCommand); 
    }

    
    public void InitializeBoard(Tile[,] boardArray) {
        tiles = new TileVisualsManager[boardArray.GetLength(0), boardArray.GetLength(1)];
        Vector3 offset = new Vector3(transform.position.x + positionOffset.x, transform.position.y + positionOffset.y);

        for (int x = 0; x < tiles.GetLength(0); x++) {
            for (int y = 0; y < tiles.GetLength(1); y++) {
                Vector3 position = new Vector3(x * tileScale.x, y * tileScale.y);

                GameObject temp = Instantiate(tilePrefab, position + offset, Quaternion.identity, this.transform).gameObject;
                temp.GetComponent<TileClick>().position = new Vector2Int(x, y); //initializes the info for TileClick as putting it in another place would have made code significantly harder to read
            }
        }
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Vector3 offset = new Vector3(transform.position.x + positionOffset.x, transform.position.y + positionOffset.y);

            for (int x = 0; x < GameManager.Instance.currentBoard.tiles.GetLength(0); x++) {
                for (int y = 0; y < GameManager.Instance.currentBoard.tiles.GetLength(1); y++) {
                    Vector3 position = new Vector3(x * tileScale.x, y * tileScale.y);

                    Gizmos.color = GameManager.Instance.currentBoard.tiles[x, y].unit == null ? Color.white : Color.red;
                    Gizmos.DrawWireCube(position + offset, new Vector3(1, 1, 0));
                }
            }
        }
    }

}
