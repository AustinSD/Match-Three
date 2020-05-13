using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempTargetPosition;
    public int previousColumn;
    public int previousRow;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherDot;
    public bool isMatched;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;

        column = (int)transform.position.x;
        row = (int)transform.position.y;
        previousColumn = column;
        previousRow = row;
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            } else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }

        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempTargetPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempTargetPosition, .6f);
            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempTargetPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempTargetPosition;
            //board.allDots[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempTargetPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempTargetPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempTargetPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempTargetPosition;
            //board.allDots[column, row] = this.gameObject;
        }

    }

    private void OnMouseDown()
    {
        // Convert pixel coords "Input.mousePosition" to World Points
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
        
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
        } 
    }

    void MovePieces()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right Swipe
            Debug.Log("Right Swipe.");
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left Swipe
            Debug.Log("Left Swipe.");
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up Swipe
            Debug.Log("Up Swipe.");
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        } else if ((swipeAngle > -135 && swipeAngle <= -45) && row > 0)
        {
            // Down Swipe
            Debug.Log("Down Swipe.");
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1 != null && rightDot1 != null && leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                Debug.Log("Match found!!");
                isMatched = true;
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null && upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
            {
                Debug.Log("Match found!!");
                upDot1.GetComponent<Dot>().isMatched = true;
                downDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }
}

/*void CalculateAngle()
{
    swipeAngle = Mathf.Atan2(finalTouchpos.y - firstTouchpos.y, finalTouchpos.x - firstTouchpos.x);
    swipeAngle *= Mathf.Rad2Deg;
    if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width)//Right Swipe
    {
        MovePieces(1, 0);//Switch with piece:(x+1,y+0)
    }
    else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.heigth)//Up Swipe
    {
        MovePieces(0, 1);//See previous
    }
    else if (swipeAngle < -45 && swipeAngle >= -135 && row > 1)//Down Swipe
    {
        MovePieces(0, -1);
    }
    else if (swipeAngle > 135 || swipeAngle <= 135 && column > 1)//Left Swipe
    {
        MovePieces(-1, 0);
    }
}

void MovePieces(int columnMove, int rowMove)
{
    otherDot = board.allDots[column + columnMove, row + rowMove];//Find the piece in that direction
    otherDot.GetComponent<Dot>().column -= columnMove;
    otherDot.GetComponent<Dot>().row -= rowMove;
    column += columnMove;
    row += rowMove;
}*/
