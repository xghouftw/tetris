using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour {

    public Tile tile;
    public Board board;
    public Piece trackingPiece; //ghost piece affilated with real piece

    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Tilemap tilemap { get; private set; }

    private void Awake() {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate() {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear() { //delete existing ghost
        for (int i = 0; i < cells.Length; i++) {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy() { //copy cells of the real piece
        for (int i = 0; i < cells.Length; i++) {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop() { //pull the current position down as far as possible without colliding with existing 
        Vector3Int position = trackingPiece.position;
        board.Clear(trackingPiece); //to prevent colliding with itself, remove and then set it back at the end
        for (int row = position.y; row >= -board.boardSize.y / 2 - 1; row--) {
            position.y = row;
            if (board.IsValidPosition(trackingPiece, position)) this.position = position;
            else break;
        }
        board.Set(trackingPiece);
    }

    private void Set() { //set ghost piece at lowest position possible
        for (int i = 0; i < cells.Length; i++) {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
}