using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState
{
    Observing,
    InLineForward,
    WalkingBack,
    LevelComplete,
    LevelFailed
}

public class GameManager : MonoBehaviour
{
    [Header("Configs")]
    public List<FeatureDefinition> allFeatures = new();
    public List<LevelConfig> levels = new();
    public int startLevelIndex = 0;

    [Header("UI")]
    public TMP_Text capacityText;
    public GameObject builderPanel;
    public TMP_Text stateText; // optional

    [Header("Wiring")]
    public NpcSpawner npcSpawner;

    // Runtime
    public bool IsRunning { get; private set; }
    public LevelConfig CurrentLevel => levels[_levelIndex];
    public GameState State => _state;

    private int _levelIndex;
    private int _capacity;
    private PartyRule _rule;
    private MaskData _playerMask = new();
    private GameState _state = GameState.Observing;
    private float _timer;

    private void Start()
    {
        _levelIndex = Mathf.Clamp(startLevelIndex, 0, levels.Count - 1);

        _playerMask.EnsureAllFeatures(allFeatures);
        StartLevel(_levelIndex);

        IsRunning = true;
    }

    private void Update()
    {
        if (!IsRunning) return;

        if (_capacity >= CurrentLevel.maxCapacity && _state != GameState.LevelComplete)
        {
            FailLevel();
        }

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                OnTimerDone();
            }
        }

        UpdateUI();
    }

    private void StartLevel(int idx)
    {
        _levelIndex = idx;
        _capacity = 0;

        _rule = PartyRuleGenerator.Generate(allFeatures, CurrentLevel.relevantFeatureCount);

        _state = GameState.Observing;
        _timer = 0f;

        // Reset player mask to something (optional). Keep current selections for now.
        // _playerMask.EnsureAllFeatures(allFeatures);

        IsRunning = true;
    }

    public void OnNpcReachedDoor(NpcController npc)
    {
        // Level 1: always accepted NPCs (for now)
        if (npc.isAccepted)
        {
            _capacity++;
        }
    }

    public void AttemptEntry()
    {
        if (_state != GameState.Observing) return;
        if (_capacity >= CurrentLevel.maxCapacity) { FailLevel(); return; }

        LockBuilder(true);
        _state = GameState.InLineForward;
        _timer = CurrentLevel.forwardWalkTime;
    }

    private void OnTimerDone()
    {
        if (_state == GameState.InLineForward)
        {
            ResolveAtDoor();
        }
        else if (_state == GameState.WalkingBack)
        {
            // back to observing / editing
            _state = GameState.Observing;
            LockBuilder(false);
        }
    }

    private void ResolveAtDoor()
    {
        if (_capacity >= CurrentLevel.maxCapacity)
        {
            FailLevel();
            return;
        }

        bool ok = DoesMaskMatchRule(_playerMask, _rule);
        if (ok)
        {
            _capacity++; // player enters
            CompleteLevel();
        }
        else
        {
            // rejection: walk back
            _state = GameState.WalkingBack;
            _timer = CurrentLevel.walkBackTime;
        }
    }

    private bool DoesMaskMatchRule(MaskData mask, PartyRule rule)
    {
        foreach (var f in rule.relevantFeatures)
        {
            int need = rule.requiredVariantIndex[f];
            int have = mask.GetVariantIndex(f);
            if (have != need) return false;
        }
        return true;
    }

    private void CompleteLevel()
    {
        _state = GameState.LevelComplete;

        int next = _levelIndex + 1;
        if (next >= levels.Count)
        {
            // loop or stop; for MVP loop
            next = 0;
        }

        StartLevel(next);
    }

    private void FailLevel()
    {
        _state = GameState.LevelFailed;
        // restart same level
        StartLevel(_levelIndex);
    }

    public MaskData GetPlayerMask() => _playerMask;

    public MaskData CreateNpcMask(bool accepted)
    {
        var m = new MaskData();
        m.EnsureAllFeatures(allFeatures);

        // irrelevant random first
        foreach (var c in m.choices)
        {
            int count = c.feature.variants.Count;
            c.variantIndex = Random.Range(0, count);
        }

        if (accepted)
        {
            // force relevant to required
            foreach (var f in _rule.relevantFeatures)
            {
                m.SetVariantIndex(f, _rule.requiredVariantIndex[f]);
            }
        }

        return m;
    }

    public string BuildMaskDebugString(MaskData m, bool showAll = false)
    {
        if (showAll)
        {
            string s = "";
            foreach (var c in m.choices)
                s += $"{c.feature.featureId}:{c.variantIndex + 1}\n";
            return s.TrimEnd();
        }

        // default: only relevant
        string r = "";
        foreach (var f in _rule.relevantFeatures)
            r += $"{f.featureId}:{m.GetVariantIndex(f) + 1}\n";
        return r.TrimEnd();
    }

    private void LockBuilder(bool locked)
    {
        // simplest MVP: just disable the whole builder panel interactivity
        // (still visible if you want, or hide it)
        // We'll keep it visible but you can also hide.
        var cg = builderPanel.GetComponent<CanvasGroup>();
        if (!cg) cg = builderPanel.AddComponent<CanvasGroup>();

        cg.interactable = !locked;
        cg.blocksRaycasts = !locked;
        cg.alpha = locked ? 0.7f : 1f;
    }

    private void UpdateUI()
    {
        if (capacityText != null)
            capacityText.text = $"Capacity: {_capacity}/{CurrentLevel.maxCapacity}";

        if (stateText != null)
            stateText.text = $"State: {_state}";
    }

    public void ShowBuilder(bool show)
    {
        if (builderPanel != null) builderPanel.SetActive(show);
    }
}
