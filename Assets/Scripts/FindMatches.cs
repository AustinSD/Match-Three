using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }

        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }
        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }
        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }

        return currentDots;
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if(currentDot != null)
                {
                    if(i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if(leftDot != null && rightDot != null)
                        {
                            if(leftDot.tag == board.allDots[i, j].tag && rightDot.tag == board.allDots[i, j].tag)
                            {
                                currentMatches.Union(IsRowBomb(currentDot.GetComponent<Dot>(), leftDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));
                                currentMatches.Union(IsColumnBomb(currentDot.GetComponent<Dot>(), leftDot.GetComponent<Dot>(), rightDot.GetComponent<Dot>()));

                                AddToListAndMatch(currentDot);
                                AddToListAndMatch(leftDot);
                                AddToListAndMatch(rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == board.allDots[i, j].tag && downDot.tag == board.allDots[i, j].tag)
                            {

                                currentMatches.Union(IsRowBomb(currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>(), upDot.GetComponent<Dot>()));
                                currentMatches.Union(IsColumnBomb(currentDot.GetComponent<Dot>(), downDot.GetComponent<Dot>(), upDot.GetComponent<Dot>()));

                                AddToListAndMatch(currentDot);
                                AddToListAndMatch(upDot);
                                AddToListAndMatch(downDot);
                            }
                        }
                    }
                }
                if(currentDot != null)
                {
                    if (currentDot.GetComponent<Dot>().isMatched == true && currentDot.GetComponent<Dot>().isColumnBomb)
                    {
                        currentMatches.Union(GetColumnPieces(i));
                    }
                    if (currentDot.GetComponent<Dot>().isMatched == true && currentDot.GetComponent<Dot>().isRowBomb)
                    {
                        currentMatches.Union(GetRowPieces(j));
                    }
                }
            }
        }
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i < board.height; i++)
        {
            if(board.allDots[column, i] != null)
            {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    public void MatchColorPieces(string color)
    {
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j <board.height; j++)
            {
                if(board.allDots[i, j] != null)
                {
                    if(board.allDots[i, j].tag == color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    public void CheckBombs()
    {
        // Did the player move a piece
        if(board.currentDot != null){
            // Is the piece moved matched
            if (board.currentDot.isMatched)
            {
                // Make current unmatched to turn it into a bomb
                board.currentDot.isMatched = false;
                // Decide type of bomb
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            } 
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }

                }
            }
        }
    }

    public void CheckRainbowBomb()
    {
        // Did the player move a piece
        if (board.currentDot != null)
        {
            // Is the piece moved matched
            if (board.currentDot.isMatched)
            {
                // Make current unmatched to turn it into a bomb
                board.currentDot.isMatched = false;
                // Add Rainbow Bomb
                Debug.Log("Make Rainbow Bomb.");
                board.currentDot.MakeColorBomb();

            }
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                    // Make current unmatched to turn it into a bomb
                    board.currentDot.isMatched = false;
                    // Add Rainbow Bomb
                    Debug.Log("Make Rainbow Bomb.");
                    otherDot.MakeColorBomb();

                }
            }
        }
    }
}
