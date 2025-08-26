using Dreamteck.Splines;
using System.Linq;
using System.Numerics;

public class T_Coin_Manager : WizardMiniGameManager
{
    public static T_Coin_Manager Instance { get; private set; }

    public SplineFollower cam;
    protected override void Awake()
    {
        base.Awake();
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        UpdateHUD();
    }

    private void Update()
    {
        if (isGameStarted && cam.follow) return;
        else if (isGameStarted && (!cam.follow))
        {
            cam.follow = true;
            //spike.follow = true;
        }
    }

    protected override void TriggerAfterCutscene()
    {
        base.TriggerAfterCutscene();
    }

    public override void SpawnRewardAvatar()
    {
        base.SpawnRewardAvatar();
    }

    public override void ShowGameOverPanel()
    {
        base.ShowGameOverPanel();
    }

    public override bool CheckGameOver()
    {
        return (playersCompleteGame.Count == PlayerManager.instance.players.Count) || playerScores.All(p => p.Value <= 0);
    }
}