using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;

    public List<Grid> _allGrids;

    [Header("GamePlayScene")]
    public Button _nextBtn;
    public Button _rotaBtn;
    public Button _replayBtn;
    public TMP_Text TopText;
    public TMP_Text _playerShipText;
    public TMP_Text _enemyShipText;
    //
    private bool setupComplete = false;
    private bool playerTurn = true;

    private int shipIndex = 0;
    private Ship _ship;

    //Player
    private int playerShipsCount = 5;
    //List
    private List<int[]> enemyShips;
    private List<GameObject> _playerFires = new List<GameObject>();
    private List<GameObject> _enemyFires = new List<GameObject>();
    
    //Enemy
    [Header("Enemy")]
    public Enemy _enemy;   
    private int enemyShipsCount = 5;
    

    [Header("Objects")]
    public GameObject _missile;
    public GameObject _enemyMissile;
    public GameObject _placeShip;
    public GameObject _firePrefab;
    // Start is called before the first frame update
    void Start()
    {
        _ship = ships[shipIndex].GetComponent<Ship>();
        _nextBtn.onClick.AddListener(() => NextShipClicked());
        _rotaBtn.onClick.AddListener(() => NextRotateClicked());
        _replayBtn.onClick.AddListener(() =>ReplayClicked());
        enemyShips = _enemy.PlaceEnemyShips();
    }


    private void NextShipClicked()
    {
        if(!_ship.OnGameBoard())
        {
            _ship.FlashColor(Color.red);
        }
        else
        {
            if(shipIndex <= ships.Length - 2)
            {
                shipIndex++;
                _ship = ships[shipIndex].GetComponent<Ship>();
                _ship.FlashColor(Color.green);
            }
            else
            {
                _nextBtn.gameObject.SetActive(false);
                _rotaBtn.gameObject.SetActive(false);
                _placeShip.SetActive(false);
                TopText.text = "Guess an enemy grid";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++)
                {
                    ships[i].SetActive(false);
                }
            }            
        }
        
    }
    private void NextRotateClicked()
    {
        _ship.RotateShip();
    }


    public void GridClicked(GameObject grid)
    {
        if(setupComplete && playerTurn)
        {
            // Drop a missile = boom
            Vector3 gridPos = grid.transform.position;
            gridPos.y += 15;
            playerTurn = false;
            Instantiate(_missile, gridPos, _missile.transform.rotation);
        }
        else if (!setupComplete)
        {
            PlaceShip(grid);
            _ship.SetClickedGrid(grid);
        }
    }

    private void PlaceShip(GameObject grid)
    {
        _ship = ships[shipIndex].GetComponent<Ship>();
        _ship.ClearGridList();
        Vector3 newVec = _ship.GetOffSetVec(grid.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    public void CheckHit(GameObject grid)
    {
        
        int gridNum = Int32.Parse(Regex.Match(grid.name, @"\d+").Value);
        int hitCount = 0;

        foreach (int[] gridNumArray in enemyShips)
        {
            if (gridNumArray.Contains(gridNum))
            {
                for (int i = 0; i < gridNumArray.Length; i++)
                {
                    if (gridNumArray[i] == gridNum)
                    {
                        gridNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (gridNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == gridNumArray.Length)
                {
                    enemyShipsCount--;
                    TopText.text = " Sunk!!!";
                    //enemy fire
                    _enemyFires.Add(Instantiate(_firePrefab, grid.transform.position, Quaternion.identity));
                    grid.GetComponent<Grid>().SetGridColor(1, new Color32(68, 0, 0, 255));
                    grid.GetComponent<Grid>().SwitchColors(1);
                }
                else
                {
                    TopText.text = " HIT!!!";
                    grid.GetComponent<Grid>().SetGridColor(1, new Color32(255, 0, 0, 255));
                    grid.GetComponent<Grid>().SwitchColors(1);
                }
                break;
            }           
        }
        if(hitCount == 0)
        {
            grid.GetComponent<Grid>().SetGridColor(1, new Color32(36, 57, 76, 255));
            grid.GetComponent<Grid>().SwitchColors(1);
            TopText.text = " Missed, There is notthing!!!";
        }
        Invoke("EndPlayerTurn", 2.0f);
    }

    public void EnemyHitPlayer(Vector3 grid, int gridNum,GameObject hitObj)
    {
        _enemy.MissileHit(gridNum);
        grid.y += 0.2f;
       _playerFires.Add(Instantiate(_firePrefab, grid, Quaternion.identity));
        if (hitObj.GetComponent<Ship>().HitCheckSank())
        {
            playerShipsCount--;
            _playerShipText.text = playerShipsCount.ToString();
            _enemy.SunkPlayer();
        }
        Invoke("EndEnemyTurn", 2.0f);
    }

    public void EndPlayerTurn()
    {
        for(int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(true);
        }
        foreach (GameObject fire in _playerFires) fire.SetActive(true);
        foreach (GameObject fire in _enemyFires) fire.SetActive(false);
        _enemyShipText.text = enemyShipsCount.ToString();
        TopText.text = "Enemy's Turn!!!!";
        _enemy.NPCTurn();
        ColorAllGrids(0);
        if (playerShipsCount < 1) GameOver("The Eneny Won!!!!");
    }
    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i].SetActive(false);
        }
        foreach (GameObject fire in _playerFires) fire.SetActive(false);
        foreach (GameObject fire in _enemyFires) fire.SetActive(true);
        _playerShipText.text = playerShipsCount.ToString();
        TopText.text = "Your's Turn , select a grid!!!!";
        playerTurn = true;
        ColorAllGrids(1);
        if (enemyShipsCount < 1) GameOver("You Win!!!!");
    }

    private void ColorAllGrids(int colorIndex)
    {
        foreach(Grid grid in _allGrids)
        {
            grid.SwitchColors(colorIndex);
        }
    }

    private void GameOver(string winner)
    {
        TopText.text = "Game Over, " + winner;
        _replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }
    private void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
