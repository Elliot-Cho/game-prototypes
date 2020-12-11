using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private GameState _gameState;
    public GameState state {
      get {
        return _gameState;
      }
      set {
        if (_gameState != null) {
          this.UnsubscribeStateEvents();
          _gameState.OnStateExit();
        }
        _gameState = value;
        this.SubscribeStateEvents();
        _gameState.OnStateEnter();
      }
    }

    public List<Unit> initiativeOrderedUnits;

    private void Awake() {
      // Allow only a single instance of the game manager to exist (Singleton)
      if (!Instance) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
      this.state = new EnterBattleGameState(Unit.UnitList);
      // this.XMLOpsTest();
    }

    // Update is called once per frame
    void Update () {
      this.state.Execute();
    }

    private void SubscribeStateEvents() {
      Cell.CellList.ForEach(cell => {
        cell.CellClicked += this.state.OnCellClicked;
        cell.CellHighlighted += this.state.OnCellMouseOver;
        cell.CellDehighlighted += this.state.OnCellMouseExit;
      });

      Entity.EntityList.ForEach(entity => {
        entity.EntityClicked += this.state.OnEntityClicked;
        entity.EntityClicked += this.state.OnEntityMouseExit;
        entity.EntityClicked += this.state.OnEntityMouseOver;
      });

      CameraManager.Instance.CameraEdgeScroll += this.state.OnCameraEdgeScroll;
      CameraManager.Instance.CameraZoom += this.state.OnCameraZoom;
    }

    private void UnsubscribeStateEvents() {
      Cell.CellList.ForEach(cell => {
        cell.CellClicked -= this.state.OnCellClicked;
        cell.CellHighlighted -= this.state.OnCellMouseOver;
        cell.CellDehighlighted -= this.state.OnCellMouseExit;
      });

      Entity.EntityList.ForEach(entity => {
        entity.EntityClicked -= this.state.OnEntityClicked;
        entity.EntityClicked -= this.state.OnEntityMouseExit;
        entity.EntityClicked -= this.state.OnEntityMouseOver;
      });

      CameraManager.Instance.CameraEdgeScroll -= this.state.OnCameraEdgeScroll;
      CameraManager.Instance.CameraZoom -= this.state.OnCameraZoom;
    }

    private void XMLOpsTest() {
      List<SerializedObject> derp = DataDeserializer.DeserializeFile("Assets/XML/Abilities/TestAbility.xml");

      Debug.Log(derp.Count);
      SerializedAbility serializedAbility = (SerializedAbility) derp[0];
      Debug.Log(serializedAbility.abilityEffects[0]);
    }

    // private void TestCharacterDeserialization() {
    //   var sChar = XMLOps.Deserialize<SerializedCharacter>("Assets/XML/TestChar.xml");

    //   var parent = GameObject.FindGameObjectWithTag("EntityParent").transform;
    //   GameObject characterObject = new GameObject();
    //   characterObject.AddComponent<Character>();
    //   characterObject.transform.parent = parent;

    //   var character = characterObject.GetComponent<Character>();

    //   character.id = sChar.id;
    //   character.entityName = sChar.name;
    //   character.name = sChar.name;

    //   character.gridSize = new Vector2(sChar.properties.gridSizex, sChar.properties.gridSizey);
    //   character.maxHitPoints.baseValue = sChar.properties.baseHitPoints;
    //   character.stats.maxActionPoints.baseValue = sChar.properties.baseActions;
    //   character.stats.meleeDamage.baseValue = sChar.properties.baseMeleeDamage;
    //   character.stats.rangedDamage.baseValue = sChar.properties.baseRangedDamage;
    //   character.stats.magicDamage.baseValue = sChar.properties.baseMagicDamage;
    //   character.stats.defense.baseValue = sChar.properties.baseDefense;
    //   character.stats.initiative.baseValue = sChar.properties.baseInitiative;
    //   character.stats.accuracy.baseValue = sChar.properties.baseAccuracy;
    //   character.stats.evasion.baseValue = sChar.properties.baseEvasion;
    //   character.stats.resist.baseValue = sChar.properties.baseResist;
    //   character.stats.skillPower.baseValue = sChar.properties.baseSkillPower;
    //   character.stats.skillCharge.baseValue = sChar.properties.baseSkillCharge;
    //   character.stats.critChance.baseValue = sChar.properties.baseCritChance;

    //   character.attributes.strength.baseValue = sChar.baseAttributes.strength;
    //   character.attributes.dexterity.baseValue = sChar.baseAttributes.dexterity;
    //   character.attributes.speed.baseValue = sChar.baseAttributes.speed;
    //   character.attributes.constitution.baseValue = sChar.baseAttributes.constitution;
    //   character.attributes.aptitude.baseValue = sChar.baseAttributes.aptitude;

    //   character.talents.exploration.baseValue = sChar.baseTalents.exploration;
    //   character.talents.crafting.baseValue = sChar.baseTalents.crafting;
    //   character.talents.smithing.baseValue = sChar.baseTalents.smithing;
    //   character.talents.thaumaturgy.baseValue = sChar.baseTalents.thaumaturgy;

    //   character.talentStats.travelSpeed.baseValue = sChar.talentStats.travelSpeed;
    //   character.talentStats.regionClearSpeed.baseValue = sChar.talentStats.regionClearSpeed;
    //   character.talentStats.craftingQuality.baseValue = sChar.talentStats.craftingQuality;
    //   character.talentStats.craftingSpeed.baseValue = sChar.talentStats.craftingSpeed;
    //   character.talentStats.buildingSpeed.baseValue = sChar.talentStats.buildingSpeed;
    //   character.talentStats.smithingQuality.baseValue = sChar.talentStats.smithingQuality;
    //   character.talentStats.smithingSpeed.baseValue = sChar.talentStats.smithingSpeed;
    //   character.talentStats.thaumQuality.baseValue = sChar.talentStats.thaumQuality;
    //   character.talentStats.thaumSpeed.baseValue = sChar.talentStats.thaumSpeed;
    // }
  }
}
