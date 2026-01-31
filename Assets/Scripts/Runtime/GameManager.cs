using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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


    public Transform wayBackPosition;
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

    [Header("Player Movement (World)")] public Transform playerTransform;
    public Transform playerStartPoint;
    public Transform doorQueuePoint;

    private Vector3 _moveFrom;
    private Vector3 _moveTo;
    private float _moveDuration;
    private Vector3 plaerScaleForward;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameplayUIPanel;


    private void Start()
    {
        IsRunning = false; // wait for Play button
        plaerScaleForward = playerTransform.localScale;
    }

    public void StartGame()
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

        // --- HARD RESET OF SCENE STATE ---
        ShowBuilder(false); // close builder
        LockBuilder(false); // unlock (in case we were mid-line)

        _state = GameState.Observing;
        _timer = 0f;
        _moveDuration = 0f;

        if (playerTransform != null && playerStartPoint != null)
        {
            var p = playerStartPoint.position;
            p.z = 0f;
            playerTransform.position = p;
        }

        if (npcSpawner != null)
        {
            npcSpawner.ClearAllNpcs(); // clears all existing NPC instances
            npcSpawner.ResetSpawner(); // resets spawn accumulator
        }
        // --- END RESET ---

        _capacity = 0;
        _rule = PartyRuleGenerator.Generate(allFeatures, CurrentLevel.relevantFeatureCount);

        IsRunning = true;
    }


    public void OnNpcReachedDoor(NpcController npc)
    {
        if (npc == null) return;
        if (!npc.TryMarkDoorProcessed()) return;

        if (npc.isAccepted)
        {
            _capacity++;
            Destroy(npc.gameObject);
        }
        else
        {
            npc.RejectAndExitLeft(); // will walk left and self-destroy past despawnX
            npc.transform.position = wayBackPosition.position;
        }
    }

    public void AttemptEntry()
    {
        if (_state != GameState.Observing) return;
        if (_capacity >= CurrentLevel.maxCapacity)
        {
            FailLevel();
            return;
        }

        LockBuilder(true);
        _state = GameState.InLineForward;

        playerTransform.localScale = plaerScaleForward;

        _timer = CurrentLevel.forwardWalkTime;
        _moveDuration = CurrentLevel.forwardWalkTime;

        _moveFrom = playerTransform != null ? playerTransform.position : Vector3.zero;
        _moveTo = doorQueuePoint != null ? doorQueuePoint.position : _moveFrom;

        _moveFrom.z = 0f;
        _moveTo.z = 0f;
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
            if (playerTransform != null && playerStartPoint != null)
            {
                var p = playerStartPoint.position;
                p.z = 0f;
                playerTransform.position = p;
            }

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
            _moveDuration = CurrentLevel.walkBackTime;
            playerTransform.localScale = new Vector3(-plaerScaleForward.x, playerTransform.localScale.y, playerTransform.localScale.z);

            _moveFrom = playerTransform != null ? playerTransform.position : Vector3.zero;
            _moveTo = playerStartPoint != null ? playerStartPoint.position : _moveFrom;

            _moveFrom.z = 0f;
            _moveTo.z = 0f;
        }
    }

    private bool DoesMaskMatchRule(MaskData mask, PartyRule rule)
    {
        // return true;
        int countCorrectFeatures = 0;
        foreach (var f in rule.relevantFeatures)
        {
            int need = rule.requiredVariantIndex[f];
            int have = mask.GetVariantIndex(f);
            if (have == need) countCorrectFeatures++;
        }

        return countCorrectFeatures >= CurrentLevel.numberOfCorrectFeaturesToPass;
    }

    private void CompleteLevel()
    {
        _state = GameState.LevelComplete;

        int next = _levelIndex + 1;

        if (next >= levels.Count)
        {
            IsRunning = false;
            FindObjectOfType<GameCycle>()?.WinGame();
            return;
        }

        StartLevel(next);
    }


    private void FailLevel()
    {
        IsRunning = false;
        FindObjectOfType<GameCycle>()?.LoseGame();
    }

    public MaskData GetPlayerMask() => _playerMask;

    public MaskData CreateNpcMask(bool accepted)
    {
        var m = new MaskData();
        m.EnsureAllFeatures(allFeatures);

        // randomize everything first
        foreach (var c in m.choices)
        {
            int count = c.feature.variants.Count;
            c.variantIndex = Random.Range(0, count);
        }

        if (accepted)
        {
            // force relevant to required
            foreach (var f in _rule.relevantFeatures)
                m.SetVariantIndex(f, _rule.requiredVariantIndex[f]);
        }
        else
        {
            // force at least ONE relevant feature to mismatch (guaranteed wrong)
            if (_rule.relevantFeatures.Count > 0)
            {
                var f = _rule.relevantFeatures[Random.Range(0, _rule.relevantFeatures.Count)];
                int required = _rule.requiredVariantIndex[f];
                int count = f.variants.Count;

                if (count > 1)
                {
                    int different = required;
                    while (different == required)
                        different = Random.Range(0, count);

                    m.SetVariantIndex(f, different);
                }
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

        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        if (playerTransform == null) return;

        if (_state == GameState.InLineForward && _moveDuration > 0f)
        {
            float t = 1f - Mathf.Clamp01(_timer / _moveDuration);
            playerTransform.position = Vector3.Lerp(_moveFrom, _moveTo, t);
        }
        else if (_state == GameState.WalkingBack && _moveDuration > 0f)
        {
            float t = 1f - Mathf.Clamp01(_timer / _moveDuration);
            playerTransform.position = Vector3.Lerp(_moveFrom, _moveTo, t);
        }
    }


    public void ShowBuilder(bool show)
    {
        if (builderPanel != null) builderPanel.SetActive(show);
    }
}