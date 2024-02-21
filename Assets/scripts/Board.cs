using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour {
    public Tilemap tilemap {get; private set;}
    public Piece activePiece {get; private set;}
    public TetrominoData[] tetrominoes; //7 regular shapes
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Tile blank; //for adding random starts
    
    public AudioControl audioControl;
    public Menu settings;

    public int linesMax{get; private set;} = 200; //determines peak difficulty 
    public int lineClearEvent {get; private set;} //how many times has some number of lines been cleared
    public int score{get; private set;} //follows simple old Nintendo scoring system

    //defines board boundary
    public RectInt Bounds {
        get {
            Vector2Int position = new Vector2Int(-boardSize.x/2, -boardSize.y/2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake() {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start() {
        lineClearEvent = 0;
        score = 0;
        if (settings.randomStart) loadRandom();
        SpawnPiece();
    }

    public void SpawnPiece() {
        int random = UnityEngine.Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];
        activePiece.Initialize(this, spawnPosition, data);
        if (IsValidPosition(activePiece, spawnPosition)) Set(activePiece);
        else GameOver();
    }

    private void GameOver() {
        tilemap.ClearAllTiles();
        lineClearEvent = 0;
        score = 0;
        audioControl.setTempo(0.5f);
        activePiece.stepDelay = 1f;
        if (settings.randomStart) loadRandom();
    }

    //places all tiles of a piece on display
    public void Set(Piece piece) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] +  piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    //Check for any lines that are full after any piece has locked. If so, clear them
    public bool ClearLines() {
        
        int fullCount = 0;
        for (int row = Bounds.yMin; row < Bounds.yMax; row++) { //check each row if it is full
            bool fullLine = true;
            for (int col = Bounds.xMin; col < Bounds.xMax; col++) {
                Vector3Int position = new Vector3Int(col, row, 0);
                if (!tilemap.HasTile(position)) fullLine = false;
            }
            if (fullLine) {
                fullCount++;
                
                for (int col = Bounds.xMin; col < Bounds.xMax; col++) { //clear this row if full
                    Vector3Int position = new Vector3Int(col, row, 0);
                    tilemap.SetTile(position, null); 
                }
                for (int rshift = row; rshift < Bounds.yMax; rshift++) { //shift all rows above down
                    for (int c = Bounds.xMin; c < Bounds.xMax; c++) {
                        Vector3Int position = new Vector3Int(c, rshift + 1, 0);
                        TileBase above = tilemap.GetTile(position);

                        position = new Vector3Int(c, rshift, 0);
                        tilemap.SetTile(position, above);
                    }
                }
                row--; //check this row again for if the line above was full and now moved down here 
            }
        }

        if (fullCount > 0) lineClearEvent++;

        //Update score with original Nintendo system because it's simpler
        if (fullCount == 1) score += 40*(settings.level+1);
        else if (fullCount == 2) score += 100*(settings.level+1);
        else if (fullCount == 3) score += 300*(settings.level+1);
        else if (fullCount == 4) score += 1200*(settings.level+1);
    
        return fullCount > 0; //if any line has been cleared
    }

    //removes all tiles of this piece from display
    public void Clear(Piece piece) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] +  piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    //checks if this piece will be in the way of other already existing pieces if placed at this position
    public bool IsValidPosition(Piece piece, Vector3Int position) {
        RectInt bounds = Bounds;
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (tilemap.HasTile(tilePosition)) return false;
            if (!bounds.Contains((Vector2Int)tilePosition)) return false; //or out of bounds
        }
        return true;
    }

    //For any level, loads that amount of randomly selected white squares from the bottom half of the board
    //Runs only if "randomStart" setting is enabled, making the game a little more interesting
    public void loadRandom() {
        List<Tuple<int,int>> possibleCoords = new List<Tuple<int, int>>();
        for (int i = -5; i <= 5; i++) {
            for (int j = -10; j <= 0; j++) {
                possibleCoords.Add(new Tuple<int, int>(i, j));
            }
        }

        for (int l = 0; l < settings.level; l++) {
            int randomIndex = UnityEngine.Random.Range(0, possibleCoords.Count);
            Tuple<int,int> randomCoord = possibleCoords[randomIndex]; //tuple of X,Y coordinate
            possibleCoords.RemoveAt(randomIndex);
            tilemap.SetTile(new Vector3Int(randomCoord.Item1,randomCoord.Item2,0), blank);
        }
    }
}
