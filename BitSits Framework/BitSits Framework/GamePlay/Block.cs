using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    enum MDGoal
    {
        EndPovertyAndHunger,
        UniversalEducation,
        GenderEquality,
        ChildHealth,
        MaternalHealth,
        CombatAIDS,
        EnvironmentalSustainability,
        GlobalParnership,
    }

    class Block
    {
        public readonly MDGoal Goal;

        GameContent gameContent;
        Vector2 position;

        public readonly Rectangle Bounds;

        public bool IsColor = false;
        public int Score;
        int number;

        public Block(GameContent gameContent, MDGoal goal, int number, Vector2 position)
        {
            this.gameContent = gameContent;
            this.Goal = goal;
            this.position = position;
            this.number = number;
            Bounds = new Rectangle((int)position.X, (int)position.Y, Tile.Width, Tile.Width);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.goalBlocks[(int)Goal][number],
                position, new Rectangle(IsColor ? Tile.Width : 0, 0, Tile.Width, Tile.Width),
                Color.White);

            if (Score > 0)
                spriteBatch.DrawString(gameContent.gameFont, Score.ToString(),
                    position + new Vector2(Tile.Width * .85f, Tile.Width * .8f), Color.Black, 0,
                    Vector2.Zero, 10f / gameContent.gameFontSize, SpriteEffects.None, 1);
        }
    }
}
