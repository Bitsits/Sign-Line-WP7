using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class CreditScreen : MenuScreen
    {
        public override void LoadContent()
        {
            titleTexture = ScreenManager.GameContent.credits;
            titlePosition = Vector2.Zero;

            // Create our menu entries.
            MenuEntry backMenuEntry = new MenuEntry(this, "Back", new Vector2(550, 500));

            // Hook up menu event handlers.
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(backMenuEntry);
        }
    }
}
