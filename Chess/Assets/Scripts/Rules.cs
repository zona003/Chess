using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;
using System;
using ChessClientLib;

public class Rules : MonoBehaviour
{
    const string HOST = "https://localhost:44325/api/Games";
    string user;

    DragAndDrop dad;
    Chess.Chess chess;
    ChessClient chessClient;


    public Rules()
    {
        user = "1004";//SystemInfo.deviceUniqueIdentifier;
        chessClient = new ChessClient(HOST, user);
        dad = new DragAndDrop();
        chess = new Chess.Chess(chessClient.GetCurrentGame().FEN);
    }

    void Start()
    {
        

        ShowFigures();
        MarkValidFigures();

        InvokeRepeating("Refresh", 2, 2);
    }

    void Refresh()
    {
        chess = new Chess.Chess(chessClient.GetCurrentGame().FEN);
        ShowFigures();
        MarkValidFigures();
    }

    void Update()
    {
        if (dad.Action())
        {
            string from = GetSquare(dad.pickPosition);
            string to = GetSquare(dad.dropPosition);
            string figure = chess.GetFigureAt(GetSquareX(dad.pickPosition),GetSquareY(dad.pickPosition)).ToString();
            string move = figure + from + to;
            Debug.Log(move);
            
            chess = new Chess.Chess(chessClient.SendMove(move).FEN);
            Debug.Log(chessClient.SendMove(move).FEN);
            ShowFigures();
            MarkValidFigures();
        }
    }

    int GetSquareX(Vector2 position)
    {
        return Convert.ToInt32(position.x / 2);
    }
    int GetSquareY(Vector2 position)
    {
        return Convert.ToInt32(position.y / 2);
    }

    string GetSquare(Vector2 position)
    {
        int x = Convert.ToInt32(position.x / 2);
        int y = Convert.ToInt32(position.y / 2);
        

        return ((char)('a' + x)).ToString() + (y + 1).ToString();
    }

    public void ShowFigures()
    {
        int nr = 0;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                string figure = chess.GetFigureAt(x, y).ToString();
                if (figure == ".") continue;
                PlaceFigure("box" + nr,figure, y, x);
                nr++;
            }
        for(;nr<32; nr++)
        {
            PlaceFigure("box" + nr, "q", 9, 9);
        }
    }

    void MarkValidFigures()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                MarkSquare(x, y, false);
            }
        foreach(string moves in chess.GetAllMoves())
        {
            int x, y;
            GetCoord(moves.Substring(1, 2), out x, out y);
            MarkSquare(y, x, true);
        }
    }

    public void GetCoord(string name, out int x, out int y)
    {
        x = 9;
        y = 9;
        if (name.Length == 2 &&
            name[0] >= 'a' && name[0] <= 'h' &&
            name[1] >= '1' && name[1] <= '8')
        {
            x = name[0] - 'a';
            y = name[1] - '1';
        }
    }

    private void PlaceFigure(string box, string figure, int x, int y)
    {
        //Debug.Log($"{x}  {y}");
        GameObject goBox = GameObject.Find(box);
        GameObject goFigure = GameObject.Find(figure);
        GameObject goSquare = GameObject.Find("" + x + y);

        var spriteFigure = goFigure.GetComponent<SpriteRenderer>();
        var spriteBox = goBox.GetComponent<SpriteRenderer>();
        spriteBox.sprite = spriteFigure.sprite;

        goBox.transform.position = goSquare.transform.position;
    }

    void MarkSquare(int x, int y, bool isMarked)
    {
        GameObject goSquare = GameObject.Find("" + x + y);
        GameObject goCell;
        string color = (x + y) % 2 == 0 ? "Black" : "White";
        if (isMarked)
            goCell = GameObject.Find(color + "SquareMarked");
        else
            goCell = GameObject.Find(color + "Square");

        var spriteSquare = goSquare.GetComponent<SpriteRenderer>();
        var spriteCell = goCell.GetComponent<SpriteRenderer>();
        spriteSquare.sprite = spriteCell.sprite;
        
    }
}

public class DragAndDrop
{
    enum State
    {
        none,
        drag,
    }

    public Vector2 pickPosition { get; private set; }
    public Vector2 dropPosition { get; private set; }


    State state;
    GameObject item;
    Vector2 offset;

    public DragAndDrop()
    {
        state = State.none;
        item = null;
    }

    public bool Action()
    {
        switch (state)
        {
            case State.none:
                if (IsMouseButtonPressed())
                    PickUp();
                break;
            case State.drag:
                if (IsMouseButtonPressed())
                      Drag();
                else
                {
                      Drop();
                      return true;
                }
                break;
        }
        return false;
    }

    bool IsMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    void PickUp()
    {
        
        Vector2 clickPosition = GetClickPosition();
        Transform clickedItem = GetItemAt(clickPosition);
        if (clickedItem == null) return;
        pickPosition = clickedItem.position;
        item = clickedItem.gameObject;
        state = State.drag;
        offset = pickPosition - clickPosition;
        Debug.Log("Picked up " + item.name);
    }

    Vector2 GetClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Transform GetItemAt(Vector2 posistion)
    {
        RaycastHit2D[] figures = Physics2D.RaycastAll(posistion, posistion, 0.5f);
        if (figures.Length == 0)
            return null;
        return figures[0].transform;
    }

    void Drag()
    {
        item.transform.position = GetClickPosition() + offset;
    }

    void Drop()
    {
        dropPosition = item.transform.position;
        state = State.none;
        item = null;
        Debug.Log($"Drop in {dropPosition.x} {dropPosition.y}");
    }
}