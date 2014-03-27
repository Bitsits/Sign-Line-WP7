using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace BitSits_Framework
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        GameContent gameContent;

        int prevScore;
        float score;
        int starCount;

        // Meta-level game state.
        private const int MaxLevelIndex = 15;    //Number of Levels
        private int levelIndex = 0;
        private Level level;
        bool load;

        Camera2D camera = new Camera2D();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.DoubleTap;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            this.gameContent = ScreenManager.GameContent;
            LoadNextLevel();
        }


        #endregion

        #region Update and Handle Input


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (load)
            {
                if (level.IsLevelUp)
                {
                    load = false;
                    LevelUpScreen l = new LevelUpScreen(false, level.Score >= level.starScore);
                    l.Accepted += MessageBoxAccepted;
                    ScreenManager.AddScreen(l, null);
                }
                else if (level.ReloadLevel)
                {
                    load = false;
                    MessageBoxScreen m = new MessageBoxScreen(gameContent.blank, true);
                    m.Accepted += MessageBoxAccepted;
                    m.Cancelled += MessageBoxAccepted;
                    ScreenManager.AddScreen(m, null);
                }
            }

            if (IsActive) level.Update(gameTime);
        }


        void MessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            if (level.IsLevelUp)
            {
                prevScore += level.Score; score = prevScore;
                starCount += level.Score >= level.starScore ? 1 : 0;
                LoadNextLevel();
            }
            else if (level.ReloadLevel) ReloadCurrentLevel();
        }

        void GameOverAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }


        private void LoadNextLevel()
        {
            if (levelIndex == MaxLevelIndex)
            {
                //LoadingScreen.Load(ScreenManager, false, null, new QuickMenuScreen());
                //LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
                LevelUpScreen l = new LevelUpScreen(true, false);
                l.totalScore = prevScore;
                l.totalStar = starCount;

                l.Accepted += GameOverAccepted;
                ScreenManager.AddScreen(l, null);
                return;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null) level.Dispose();

            // Load the level.
            level = new Level(ScreenManager, levelIndex); ++levelIndex;
            load = true;

            //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                level.HandleInput(input, playerIndex);
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.White, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            level.Draw(gameTime, spriteBatch);

            DrawScore(gameTime, spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(1 - TransitionAlpha);
        }

        private void DrawScore(GameTime gameTime, SpriteBatch spriteBatch)
        {
            score = Math.Min(score + (float)gameTime.ElapsedGameTime.TotalSeconds * 50, prevScore + level.Score);

            spriteBatch.DrawString(gameContent.gameFont, "Level: " + levelIndex + "/" + MaxLevelIndex, new Vector2(10), 
                Color.Black, 0, Vector2.Zero, 40f / gameContent.gameFontSize, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.gameFont, "Score: " + level.Score, new Vector2(320, 10),
                Color.Black, 0, Vector2.Zero, 40f / gameContent.gameFontSize, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.gameFont, "Min Score: " + level.minScore, new Vector2(620, 5),
                Color.Black, 0, Vector2.Zero, 20f / gameContent.gameFontSize, SpriteEffects.None, 1);

            spriteBatch.Draw(gameContent.star, new Vector2(660, 40), null, Color.White,
                0, Vector2.Zero, 0.2f, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameContent.gameFont, ": " + level.starScore, new Vector2(710, 40),
                Color.Black, 0, Vector2.Zero, 20f / gameContent.gameFontSize, SpriteEffects.None, 1);
        }


        #endregion
    }
}
