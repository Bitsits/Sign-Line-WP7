using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace BitSits_Framework
{
    class LevelUpScreen : GameScreen
    {
        Camera2D camera = new Camera2D();

        bool lastLevel, isStar;
        public int totalScore, totalStar;
        
        public event EventHandler<PlayerIndexEventArgs> Accepted;

        public LevelUpScreen(bool lastLevel, bool isStar)
        {
            IsPopup = true;

            this.isStar = isStar;
            this.lastLevel = lastLevel;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void HandleInput(InputState input)
        {
            if (ScreenState != ScreenState.Active) return;

            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex)
                || input.IsMouseLeftButtonClick())
            {
                // Raise the accepted event, then exit the screen.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                //ExitScreen();
            }

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // Raise the accepted event, then exit the screen.
                    if (Accepted != null)
                        Accepted(this, new PlayerIndexEventArgs(playerIndex));

                    ExitScreen();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Vector2 pos = Camera2D.BaseScreenSize / 2;

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

            if (!lastLevel)
            {
                spriteBatch.Draw(ScreenManager.GameContent.levelUp, Vector2.Zero, color);
                if (isStar)
                    spriteBatch.Draw(ScreenManager.GameContent.star, new Vector2(550, 120), color);
            }
            else
            {
                spriteBatch.Draw(ScreenManager.GameContent.gameOver, Vector2.Zero, color);

                spriteBatch.DrawString(ScreenManager.GameContent.gameFont,   totalScore.ToString()  ,
                    new Vector2(430, 370 - 20), color, 0, Vector2.Zero, 40f / ScreenManager.GameContent.gameFontSize,
                    SpriteEffects.None, 1);
                spriteBatch.DrawString(ScreenManager.GameContent.gameFont, totalStar.ToString(),
                    new Vector2(430, 430 - 20), color, 0, Vector2.Zero, 40f / ScreenManager.GameContent.gameFontSize,
                    SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }
    }
}
