using UnityEngine;
using UnityEngine.Networking;

public class TheBushsBakedBeansGoldenRetriever : MonoBehaviour
{
    // Toggle menu visibility
    public bool Visible = true;
    public bool WeaponVisible = false;
    public bool OtherVisible = false;

    // Rect to hold menus
    private Rect window;
    private Rect weaponWindow;
    private Rect otherWindow;

    // Settings
    public static bool NoRecoil = false;
    public static bool FullAuto = false;
    public static bool InfiniteAmmo = false;
    public static bool AutoHeal = false;
    public static bool FastSprint = false;
    public static bool RocketBoots = false;
    public static bool MapESP = false;
    public static float SprintMultiplier = 1f;
    public static string StartWeapon = "0";
    public static string CustomAttachment = "0";
    public static string CustomLevel = "";

    public void Start()
    {
        this.window = new Rect(10f, 10f, 100f, 100f);
    }

    private void Update()
    {
        // Handle keys
        KeyBindings();

        // Handle hacks
        WeaponHacks();
        HealthHacks();
        MovementHacks();
        MapHacks();
    }

    private void KeyBindings()
    {
        if (Input.GetKeyDown(KeyCode.Insert)) this.Visible = !this.Visible;
        if (Input.GetKeyDown(KeyCode.C)) InfiniteAmmo = !InfiniteAmmo;
        if (Input.GetKeyDown(KeyCode.F)) FullAuto = !FullAuto;
        if (Input.GetKeyDown(KeyCode.V)) NoRecoil = !NoRecoil;
        if (Input.GetKeyDown(KeyCode.G)) AutoHeal = !AutoHeal;
        if (Input.GetKeyDown(KeyCode.H)) FastSprint = !FastSprint;
        if (Input.GetKeyDown(KeyCode.B)) RocketBoots = !RocketBoots;
    }

    private void WeaponHacks()
    {
        // Get weapon manager
        WeaponManager weaponManager = null;
        foreach (WeaponManager wm in FindObjectsOfType<WeaponManager>())
        {
            if (wm.isLocalPlayer) weaponManager = wm;
        }
        if (weaponManager == null) return;

        // Get current weapon
        Weapon cw = weaponManager.weapons[weaponManager.currentWeapon];

        if (InfiniteAmmo)
        {
            cw.currentclip = cw.clipSize;
            cw.Networkcurrentclip = cw.clipSize;
            cw.totalAmmo = cw.maxSpareAmmo;
            cw.NetworktotalAmmo = cw.maxSpareAmmo;
        }


        cw.fullAuto = FullAuto;
        if (FullAuto) cw.recovering = false;

        if (NoRecoil)
        {
            cw.verticalKick = 0f;
            cw.sideKick = 0f;
            cw.additionalSideKick = 0f;
        }

        if (StartWeapon.Length != 0) weaponManager.startingWeapon = int.Parse(StartWeapon);
        else weaponManager.startingWeapon = 0;

        if (CustomAttachment.Length != 0)
        {
            cw.currentAttachment = int.Parse(CustomAttachment);
            cw.NetworkcurrentAttachment = int.Parse(CustomAttachment);
        }
        else
        {
            cw.currentAttachment = 0;
            cw.NetworkcurrentAttachment = 0;
        }
    }

    private void HealthHacks()
    {
        // Get health class
        Health health = null;
        foreach (Health h in FindObjectsOfType<Health>())
        {
            if (h.isLocalPlayer) health = h;
        }
        if (health == null) return;

        // Get NetworkIdentity
        NetworkIdentity id = null;
        foreach (NetworkIdentity ni in FindObjectsOfType<NetworkIdentity>())
        {
            if (ni.isLocalPlayer) id = ni;
        }
        if (id == null) return;

        // Get Extras object
        Extras extras = null;
        foreach (Extras ex in FindObjectsOfType<Extras>())
        {
            if (ex.isLocalPlayer) extras = ex;
        }
        if (extras == null) return;


        if (AutoHeal && health.currentHealth <= 90 && !health.healing)
        {
            extras.CallCmdHeal(id.netId, 100 - health.currentHealth);
        }
    }

    private void MovementHacks()
    {
        Movement move = null;
        foreach (Movement mv in FindObjectsOfType<Movement>())
        {
            if (mv.isLocalPlayer) move = mv;
        }
        if (move == null) return;

        if (FastSprint)
            move.sprintSpeed = 12f * SprintMultiplier;
        else
            move.sprintSpeed = 12f;

        move.rocketJumpEnabled = RocketBoots;
    }

    private void MapHacks()
    {
        SetUpLocalPlayer slp = null;
        foreach (SetUpLocalPlayer s in FindObjectsOfType<SetUpLocalPlayer>())
        {
            if (s.isLocalPlayer) slp = s;
        }
        if (slp == null) return;

        if (MapESP)
        {
            foreach (NetworkIdentity ni in FindObjectsOfType<NetworkIdentity>())
            {
                if (!ni.isLocalPlayer)
                {
                    GameObject player = ClientScene.FindLocalObject(ni.netId);
                    player.GetComponent<SetUpLocalPlayer>().playerColor = Color.red;
                    slp.SetSinglePointerUI(player);
                }
            }
        }
    }

    public void OnGUI()
    {
        if (!this.Visible) return;
        this.window = GUILayout.Window(0, this.window, new GUI.WindowFunction(this.Draw), "BeanBreaker", new GUILayoutOption[0]);
        if (this.WeaponVisible) this.weaponWindow = GUILayout.Window(1, this.weaponWindow, new GUI.WindowFunction(this.DrawWeaponOptions), "Weapon Options", new GUILayoutOption[0]);
        if (this.OtherVisible) this.otherWindow = GUILayout.Window(2, this.otherWindow, new GUI.WindowFunction(this.DrawOtherOptions), "Other Options", new GUILayoutOption[0]);
    }

    public void Draw(int id)
    {
        InfiniteAmmo = GUILayout.Toggle(InfiniteAmmo, "Infinite Ammo (C)", new GUILayoutOption[0]);
        FullAuto = GUILayout.Toggle(FullAuto, "Full Auto (F)", new GUILayoutOption[0]);
        NoRecoil = GUILayout.Toggle(NoRecoil, "No Recoil (V)", new GUILayoutOption[0]);
        AutoHeal = GUILayout.Toggle(AutoHeal, "Auto Heal (G)", new GUILayoutOption[0]);
        string sprintTitle = "Sprint Multiplier (H): " + SprintMultiplier.ToString() + "x";
        FastSprint = GUILayout.Toggle(FastSprint, sprintTitle, new GUILayoutOption[0]);
        SprintMultiplier = Mathf.Round(GUILayout.HorizontalSlider(SprintMultiplier, 1f, 3f, new GUILayoutOption[0]) * 100f) / 100f;
        RocketBoots = GUILayout.Toggle(RocketBoots, "Rocket Jump (B)", new GUILayoutOption[0]);
        MapESP = GUILayout.Toggle(MapESP, "Map ESP", new GUILayoutOption[0]);
        if (GUILayout.Button("Weapon Options", new GUILayoutOption[0]))
        {
            this.weaponWindow.x = this.window.width + 20f;
            this.weaponWindow.width = 200f;
            this.WeaponVisible = !this.WeaponVisible;
        }
        if (GUILayout.Button("Other Options", new GUILayoutOption[0]))
        {
            this.otherWindow.x = this.window.width + 20f;
            this.OtherVisible = !this.OtherVisible;
        }
        GUI.DragWindow();
    }

    public void DrawWeaponOptions(int id)
    {
        GUILayout.Label("Set Starting Weapon (use number):", new GUILayoutOption[0]);
        StartWeapon = GUILayout.TextField(StartWeapon, new GUILayoutOption[0]);
        string WeaponList = "Glock: 0, Ranger: 1, Pump: 2\nAK-47: 3, B50: 4, Uzi: 5\nRocket: 6, AS-VAL: 7\nGrenade: 8, TM-100: 9\nRevolver: 10, M16: 11\nLazer: 12, R70: 13, KAT: 14";
        GUILayout.Label(WeaponList, new GUILayoutOption[0]);
        GUILayout.Label("Set current attachment:", new GUILayoutOption[0]);
        CustomAttachment = GUILayout.TextField(CustomAttachment, new GUILayoutOption[0]);
        string AttachmentList = "None: 0, Holographic: 1\nAcog: 2, Sniper: 3\nSilencer: 4";
        GUILayout.Label(AttachmentList, new GUILayoutOption[0]);
        GUI.DragWindow();
    }

    public void DrawOtherOptions(int id)
    {
        SteamPlayerInfo player = FindObjectOfType<SteamPlayerInfo>();
        if (GUILayout.Button("Unlock Every Item", new GUILayoutOption[0]))
        {
            player.UnlockEveryItem();
        }
        if (GUILayout.Button("Unlock Every Icon/Sprite", new GUILayoutOption[0]))
        {
            for (int i = 0; i < player.playerSprites.Length; i++) player.UnlockSprite(i);
        }
        if (GUILayout.Button("Unlock Every Title", new GUILayoutOption[0]))
        {
            for (int i = 0; i < player.titleManager.titles.Length; i++) player.titleManager.UnlockTitle(i);
        }
        if (GUILayout.Button("Unlock Every Acheivement", new GUILayoutOption[0]))
        {
            for (int i = 0; i < player.AchievementsID.Count; i++) player.UnlockSteamAchievement(player.AchievementsID[i]);
        }
        GUILayout.Label("Enter Player Level:", new GUILayoutOption[0]);
        if (CustomLevel.Length == 0) CustomLevel = player.GetPlayerLevel().ToString();
        CustomLevel = GUILayout.TextField(CustomLevel, new GUILayoutOption[0]);
        if (GUILayout.Button("Set Level", new GUILayoutOption[0]))
        {
            if (CustomLevel.Length > 0)
                player.SetPlayerLevel(int.Parse(CustomLevel));
        }
        GUI.DragWindow();
    }
}
