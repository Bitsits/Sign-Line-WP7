#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace BitSits_Framework
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        public override void LoadContent()
        {
            titleTexture = ScreenManager.GameContent.mainMenuTitle;
            titlePosition = Vector2.Zero;

            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry(this, "Play", new Vector2(550, 360));
            MenuEntry creditsMenuEntry = new MenuEntry(this, "Credits", new Vector2(550, 430));
            MenuEntry optionsMenuEntry = new MenuEntry(this, "Options", new Vector2(550, 430));
            MenuEntry exitMenuEntry = new MenuEntry(this, "Exit", new Vector2(550, 500));

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            creditsMenuEntry.Selected += CreditsMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(creditsMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
                               new GameplayScreen(), new PauseMenuScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreditScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // Do nothing
        }


        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
