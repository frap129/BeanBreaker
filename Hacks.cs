﻿using UnityEngine;

public class TheBushsBakedBeansGoldenRetriever : MonoBehaviour
{
    public WeaponManager weaponManager;

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

    public void Start()
    {
        this.window = new Rect(10f, 10f, 100f, 100f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            this.Visible = !this.Visible;
        }

        // Get weapon manager
        foreach (WeaponManager wm in FindObjectsOfType<WeaponManager>())
        {
            if (wm.isLocalPlayer) weaponManager = wm;
        }
        if (weaponManager == null) return;

        Weapon cw = weaponManager.weapons[weaponManager.currentWeapon];

        if (InfiniteAmmo)
        {
            cw.currentclip = cw.clipSize;
            cw.Networkcurrentclip = cw.clipSize;
        }

        if (FullAuto)
        {
            cw.fullAuto = true;
            cw.recovering = false;
            cw.recoveryTime = 0f;
        }

        if (NoRecoil)
        {
            cw.verticalKick = 0f;
            cw.sideKick = 0f;
            cw.additionalSideKick = 0f;
        }
    }

    public void OnGUI()
    {
        if (!this.Visible) return;
        this.window = GUILayout.Window(0, this.window, new GUI.WindowFunction(this.Draw), "BeanBreaker", new GUILayoutOption[0]);
        if (this.OtherVisible) this.otherWindow = GUILayout.Window(1, this.otherWindow, new GUI.WindowFunction(this.DrawOtherOptions), "Other Options", new GUILayoutOption[0]);
    }

    public void Draw(int id)
    {
        InfiniteAmmo = GUILayout.Toggle(InfiniteAmmo, "Infinite Ammo", new GUILayoutOption[0]);
        FullAuto = GUILayout.Toggle(FullAuto, "Full Auto", new GUILayoutOption[0]);
        NoRecoil = GUILayout.Toggle(NoRecoil, "No Recoil", new GUILayoutOption[0]);
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
