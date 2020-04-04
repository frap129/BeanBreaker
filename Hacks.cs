using UnityEngine;

public class TheBushsBakedBeansGoldenRetriever : MonoBehaviour
{
    // Toggle menu visibility
    public bool Visible = true;
    public bool OtherVisible = false;

    // Rect to hold menus
    private Rect window;
    private Rect otherWindow;

    // Settings
    public static bool NoRecoil = false;
    public static bool FullAuto = false;
    public static bool InfiniteAmmo = false;
    public static bool InfiniteHealth = false;
    public static bool FastSprint = false;
    public static bool RocketBoots = false;

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
    }

    private void KeyBindings()
    {
        if (Input.GetKeyDown(KeyCode.Insert)) this.Visible = !this.Visible;
        if (Input.GetKeyDown(KeyCode.C)) InfiniteAmmo = !InfiniteAmmo;
        if (Input.GetKeyDown(KeyCode.F)) FullAuto = !FullAuto;
        if (Input.GetKeyDown(KeyCode.V)) NoRecoil = !NoRecoil;
        if (Input.GetKeyDown(KeyCode.G)) InfiniteHealth = !InfiniteHealth;
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
        }


        cw.fullAuto = FullAuto;
        if (FullAuto) cw.recovering = false;

        if (NoRecoil)
        {
            cw.verticalKick = 0f;
            cw.sideKick = 0f;
            cw.additionalSideKick = 0f;
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

        if (InfiniteHealth)
        {
            health.NetworkcurrentHealth = 125;
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
            move.sprintSpeed = 30f;
        else
            move.sprintSpeed = 12f;

        move.rocketJumpEnabled = RocketBoots;
    }

    public void OnGUI()
    {
        if (!this.Visible) return;
        this.window = GUILayout.Window(0, this.window, new GUI.WindowFunction(this.Draw), "BeanBreaker", new GUILayoutOption[0]);
        if (this.OtherVisible) this.otherWindow = GUILayout.Window(1, this.otherWindow, new GUI.WindowFunction(this.DrawOtherOptions), "Other Options", new GUILayoutOption[0]);
    }

    public void Draw(int id)
    {
        InfiniteAmmo = GUILayout.Toggle(InfiniteAmmo, "Infinite Ammo (C)", new GUILayoutOption[0]);
        FullAuto = GUILayout.Toggle(FullAuto, "Full Auto (F)", new GUILayoutOption[0]);
        NoRecoil = GUILayout.Toggle(NoRecoil, "No Recoil (V)", new GUILayoutOption[0]);
        InfiniteHealth = GUILayout.Toggle(InfiniteHealth, "Infinite Health (G)", new GUILayoutOption[0]);
        FastSprint = GUILayout.Toggle(FastSprint, "Sprint fast (H)", new GUILayoutOption[0]);
        RocketBoots = GUILayout.Toggle(RocketBoots, "Rocket Jump (B)", new GUILayoutOption[0]);
        if (GUILayout.Button("Other Options", new GUILayoutOption[0]))
        {
            this.otherWindow.x = this.window.width + 20f;
            this.OtherVisible = !this.OtherVisible;
        }
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
            for(int i = 0; i < player.playerSprites.Length; i++) player.UnlockSprite(i);
        }
        if (GUILayout.Button("Unlock Every Title", new GUILayoutOption[0]))
        {
            for (int i = 0; i < player.titleManager.titles.Length; i++) player.titleManager.UnlockTitle(i);
        }
        if (GUILayout.Button("Unlock Every Acheivement", new GUILayoutOption[0]))
        {
            for (int i = 0; i < player.AchievementsID.Count; i++) player.UnlockSteamAchievement(player.AchievementsID[i]);
        }
        GUI.DragWindow();

    }
}
