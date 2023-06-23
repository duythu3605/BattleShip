using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    char[] guessGrid;
    List<int> potentialHits;
    List<int> currentHits;
    private int guess;

    public GameObject enemyMissilePrefab;

    public GameManager _gameManager;

    private void Start()
    {
        potentialHits = new List<int>();
        currentHits = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray();
    }
    public List<int[]> PlaceEnemyShips()
    {
        List<int[]> enemyShips = new List<int[]>
        {
            new int[]{-1,-1,-1,-1,-1},
            new int[]{-1,-1,-1,-1},
            new int[]{-1,-1,-1},
            new int[]{-1,-1,-1},
            new int[]{-1,-1}
        };
        int[] gridNumbers = Enumerable.Range(1, 100).ToArray();
        bool taken = true;
        foreach(int[] gridNumArray in enemyShips)
        {
            taken = true;
            while(taken == true)
            {
                taken = false;
                int shipMode = UnityEngine.Random.Range(0, 99);
                int rotateBool = UnityEngine.Random.Range(0, 2);
                int minusAmount = rotateBool == 0 ? 10 : 1;
                for(int i = 0; i < gridNumArray.Length; i++)
                {
                    //Check that ship  end will not go off board and check if grid is taken
                    if((shipMode - (minusAmount*i )) < 0 || gridNumbers[shipMode - i * minusAmount] < 0)
                    {
                        taken = true;
                        break;
                    }
                    //Ship is horizontal, check ship doesnt go off the sides 0 to 10 , 11 to 20
                    else if( minusAmount == 1 && shipMode / 10 != ((shipMode -i * minusAmount) -1) / 10)
                    {
                        taken = true;
                        break;
                    }
                }
                // if grid is not taken, loop through grid number assign them to the array in the list
                if (taken == false)
                {
                    for(int j = 0; j <gridNumArray.Length; j++)
                    {
                        gridNumArray[j] = gridNumbers[shipMode - j * minusAmount];
                        gridNumbers[shipMode - j * minusAmount] = -1;
                    }
                }
            }
        }

        foreach(var x in enemyShips)
        {
            Debug.Log("x: " + x[0]);
        }
        return enemyShips;
    }

    public void NPCTurn()
    {        
        List<int> hitIndex = new List<int>();
        for(int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') hitIndex.Add(i);
        }
        if (hitIndex.Count > 1)
        {
            int diff = hitIndex[1] = hitIndex[0];
            int posNeg = Random.Range(0, 2) * 2 -1;
            int nextIndex = hitIndex[0] + diff;

            while(guessGrid[nextIndex] != 'o')
            {
                if(guessGrid[nextIndex] == 'm' || nextIndex > 100 || nextIndex < 0)
                {
                    diff *= -1;
                }
                nextIndex += diff;
            }
            guess = nextIndex;
        }
        else if(hitIndex.Count == 1)
        {
            List<int> closeGrids = new List<int>();
            closeGrids.Add(1);closeGrids.Add(-1);closeGrids.Add(10);closeGrids.Add(-10);

            int index = Random.Range(0, closeGrids.Count);
            int possibleGuess = hitIndex[0] + closeGrids[index];
            bool onGrid = possibleGuess > -1 && possibleGuess < 100;
            
            while((!onGrid || guessGrid[possibleGuess] != 'o') && closeGrids.Count > 0)
            {
                closeGrids.RemoveAt(index);
                index = Random.Range(0, closeGrids.Count);
                possibleGuess = hitIndex[0] + closeGrids[index];
                onGrid = possibleGuess > -1 && possibleGuess < 100;
            }
            guess = possibleGuess; 
        }
        else
        {
            int nextIndex = Random.Range(0, 100);
            
            while (guessGrid[nextIndex] != 'o') nextIndex = Random.Range(0, 100);
            nextIndex = GuessAgainCheck(nextIndex);
            Debug.Log("-------");
            nextIndex = GuessAgainCheck(nextIndex);
            Debug.Log("Again: -------");
            guess = nextIndex;
        }
        GameObject gridObj = GameObject.Find("Grid (" + (guess + 1) + ")");
        guessGrid[guess] = 'm';
        Vector3 vec = gridObj.transform.position;
        vec.y += 20;
        GameObject missible = Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation);
        missible.GetComponent<EnemyMissile>().SetTarget(guess);
        missible.GetComponent<EnemyMissile>()._targetGridLocation = gridObj.transform.position;
    }

    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h';
        Invoke("EndTurn", 1.0f);
    }

    public void SunkPlayer()
    {
        for(int i =0; i< guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') guessGrid[i] = 'x';
        }
    }

    public void EndTurn()
    {
        _gameManager.GetComponent<GameManager>().EndEnemyTurn();
    }
    public void PauseAndEnd(int miss)
    {
        if(currentHits.Count > 0 &&  currentHits[0] > miss)
        {
            foreach(int potential in potentialHits)
            {
                if(currentHits[0] > miss)
                {
                    if (potential < miss) potentialHits.Remove(potential);
                }
                else
                {
                    if (potential > miss) potentialHits.Remove(potential);
                }
            }
        }
        Invoke("EndTurn", 1.0f);
    }


    private int GuessAgainCheck(int currentIndex)
    {
        string str = "nx: " + currentHits;
        int nextIndex = currentIndex;
        bool edgeCase = nextIndex < 10 || nextIndex > 89 || nextIndex % 10 == 0 || nextIndex % 10 == 9;
        bool nearGuess = false;
        if (nextIndex + 1 < 100) nearGuess = guessGrid[nextIndex + 1] != 'o';
        if (!nearGuess && nextIndex - 1 > 0) nearGuess = guessGrid[nextIndex - 1] != 'o';
        if (!nearGuess && nextIndex + 10 < 99) nearGuess = guessGrid[nextIndex + 10] != 'o';
        if (!nearGuess && nextIndex - 10 > 0) nearGuess = guessGrid[nextIndex - 10] != 'o';
        if (edgeCase || nearGuess) nextIndex = Random.Range(0, 100);
        while (guessGrid[nextIndex] != 'o') nextIndex = Random.Range(0, 100);
        Debug.Log("GuessAgainCheck------- : " + str + " newGuess: " + nextIndex + " e: " + edgeCase + " g: " + nearGuess);
        return nextIndex;
    }
}
