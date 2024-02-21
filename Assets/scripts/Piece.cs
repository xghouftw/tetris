using System;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Board board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public int rotationIndex {get; private set;} //orientation of piece

    public float stepDelay = 1f; //time for piece to fall down
    public float lockDelay = 0.5f; //time for piece to lock and stay

    //keeping track of last time stepped/locked
    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data) {
        this.board = board;
        this.position = position;
        this.data = data;

        rotationIndex = 0;

        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        if (cells == null) {
            cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++) {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public void Update() {
        board.Clear(this); //to make sure isValidPosition from board doesn't collide with this current piece
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }
        else if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1); //counterclockwise
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            Rotate(1); //clockwise
        }

        if (Time.time >= stepTime) {
            Step(); //time to auto move one down
        }
        board.Set(this); //put the piece back
    }

    private void Step() { //auto one move down every stepDelay
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);
        if (lockTime >= lockDelay) { //if piece hasn't moved in a while, lock in place
            Lock();
        }
    }

    private void Lock() { //ends actions for this piece, starts new piece
        board.Set(this);
        if (board.ClearLines()) { //if there was at least one line cleared, make game harder
            float difficultyRamp = 1f*board.linesMax/(1f*board.settings.level);
            float difficultyFrac = Math.Min(1f, 1f*board.lineClearEvent/difficultyRamp); //fraction of ramped up difficulty
            //Adjust audio and stepDelay to reflect how fast the game moves until reaching peak difficulty
            board.audioControl.setTempo(0.75f+(2-0.75f)*difficultyFrac); //increases from 80% of original tempo to twice
            stepDelay = 1f - (1-0.1f)*difficultyFrac; //decreases from 1 to 0.1 seconds between each step
        }
        board.SpawnPiece();
    }

    private bool Move(Vector2Int translation) {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        bool valid = board.IsValidPosition(this, newPosition);
        if (valid) {
            position = newPosition;
            lockTime = 0f; //piece has moved, so lock timer resets
        }
        return valid;
    }

    private void HardDrop() {
        while(Move(Vector2Int.down)) continue; //go down as far as possible
        Lock();
    }

    private void Rotate(int direction) {
        int curRotationIndex = rotationIndex;
        rotationIndex = (rotationIndex + direction) % 4; //quadrant of rotation
        ApplyRotationMatrix(direction); 
        if (!TestWallKicks(rotationIndex, direction)) { //complications if rotating runs into walls/other pieces
            rotationIndex = curRotationIndex;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction) {
        for (int i = 0; i < cells.Length; i++) {
            Vector3 cell = cells[i];
            int x, y;
            switch (data.tetromino) {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(cell.x * Data.RotationMatrix[0] * direction + cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cell.x * Data.RotationMatrix[2] * direction + cell.y * Data.RotationMatrix[3] * direction);
                    break;

                default:
                    x = Mathf.RoundToInt(cell.x * Data.RotationMatrix[0] * direction + cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cell.x * Data.RotationMatrix[2] * direction + cell.y * Data.RotationMatrix[3] * direction);
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection) {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < data.wallKicks.GetLength(1); i++) {

            Vector2Int translation = data.wallKicks[wallKickIndex, i];
            if (Move(translation)) return true;
           
        }
        return false;
    }
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)  {
        int wallKickIndex = rotationIndex * 2;
        if (rotationDirection < 0) wallKickIndex--;
        wallKickIndex = (wallKickIndex + 8) % 8;
        return wallKickIndex;
    }
}
