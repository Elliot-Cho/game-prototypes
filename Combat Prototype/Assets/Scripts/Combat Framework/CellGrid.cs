using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CellGrid : MonoBehaviour {

    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;

    // The grid delegates some of its behaviours to cellGridState object.
    private CellGridState _cellGridState;
    public CellGridState CellGridState
    {
        get
        {
            return _cellGridState;
        }
        set
        {
            if (_cellGridState != null)
                _cellGridState.OnStateExit();
            _cellGridState = value;
            _cellGridState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public Player CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }

    public int CurrentPlayerNumber { get; private set; }

    public Transform PlayersParent;
    public Transform CornerGrid;    // For movement & pathfinding of large units - units of even GridSize use this

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }
    public List<Obstacle> Obstacles { get; private set; }

    // Units list ordered by dexterity
    public List<Unit> InitTracker { get; private set; }

    // Use this for initialization
    void Start () {

        // Create a list of all players (that control the units)
        Players = new List<Player>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        NumberOfPlayers = Players.Count;

        // Create a list of all cells on the current scene
        // Individual cells are children of the parent Grid
        Cells = new List<Cell>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();

            if (cell != null)
                Cells.Add(cell);
            else
                Debug.LogError("Invalid object in cells parent game object");
        }

        // Entity generator
        var entityGenerator = GetComponent<IUnitGenerator>();
        var Entities = new List<Entity>();

        // Setup Entities
        if (entityGenerator != null)
        {
            Entities = entityGenerator.SpawnEntities(Cells);

            Units = new List<Unit>();
            Obstacles = new List<Obstacle>();
            foreach (Entity entity in Entities)
            {
                if (entity is Unit)
                    Units.Add(entity as Unit);
                else if (entity is Obstacle)
                    Obstacles.Add(entity as Obstacle);
            }

            // Set CurrentPlayerNumber to the owner of the unit with the highest dexterity score
            InitTracker = Units.OrderByDescending(o => o.Dexterity).ToList();
            CurrentPlayerNumber = InitTracker[0].PlayerNumber;

            // Subscribe functions to entity events
            foreach (var unit in Units)
            {
                unit.EntityClicked += OnEntityClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
            foreach (var obs in Obstacles)
            {
                obs.EntityClicked += OnEntityClicked;
                obs.ObsDestroyed += OnObsDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");

        // Subscribe click and highlighting events to Cell clicks
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }

        CellGridState = new CellGridStateBase(this);

        StartGame();
    }

    // Base cell interaction events
    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        CellGridState.OnCellSelected(sender as Cell);
    }
    private void OnCellClicked(object sender, EventArgs e)
    {
        CellGridState.OnCellClicked(sender as Cell);
    }

    // Entity events
    private void OnEntityClicked(object sender, EventArgs e)
    {
        CellGridState.OnEntityClicked(sender as Entity);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if (GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());

            Debug.Log("Game Over");
        }
    }
    private void OnObsDestroyed(object sender, EventArgs e)
    {
        Obstacles.Remove(sender as Obstacle);
    }

    // Method is called once, at the beginning of the game.
    public void StartGame()
    {
        if (GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());

        InitTracker[0].OnTurnStart();
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        // Select unit with a short delay
        StartCoroutine(StartGameSelect());
    }
    // Delay cellgrid state change to allow cell sprites to load first
    private IEnumerator StartGameSelect ()
    {
        yield return new WaitForSeconds(0.5f);
        if (CurrentPlayerNumber == 0)
        {
            CellGridState = new CellGridStateUnitSelected(this, InitTracker[0]);
        }
    }

    /// <summary>
    /// Method makes turn transitions. It is called by player at the end of his turn.
    /// </summary>
    public void EndTurn()
    {
        if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
        {
            return;
        }
        CellGridState = new CellGridStateBase(this);

        // End turn for current unit and advance the init tracker
        InitTracker[0].OnTurnEnd();
        AdvanceTracker();
        // Set current player to next player in the initiative
        CurrentPlayerNumber = InitTracker[0].PlayerNumber;

        if (TurnEnded != null)
            TurnEnded.Invoke(this, new EventArgs());

        // Start next unit's turn
        InitTracker[0].OnTurnStart();
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        // Select unit
        if (CurrentPlayerNumber == 0)
        {
            CellGridState = new CellGridStateUnitSelected(this, InitTracker[0]);
        }
    }

    // Advances the InitTracker by moving the first unit to the end after its turn ends
    public void AdvanceTracker ()
    {
        InitTracker.Add(InitTracker[0]);
        InitTracker.RemoveAt(0);
    }
}
