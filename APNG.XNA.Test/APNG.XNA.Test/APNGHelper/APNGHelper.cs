using System;
using System.Collections.Generic;

using APNG;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace APNGTest.APNGHelper
{
    public class APNGHelper
    {
        #region Constants and Fields

        private readonly List<APNGFrame> frameList = new List<APNGFrame>();
        private readonly Game game;
        private readonly int numPlays;
        private readonly List<Texture2D> renderedTextureList = new List<Texture2D>();
        private readonly SpriteBatch sb;
        private int alreadyPlays;
        private TimeSpan alreadyWaitTime = TimeSpan.Zero;
        private APNGFrame baseFrame;
        private int currentPlayedIndex;

        #endregion Constants and Fields

        #region Constructors and Destructors

        public APNGHelper(Game game, string pngFile)
        {
            this.game = game;

            var image = new APNG.APNG(pngFile);
            this.numPlays = (int)image.acTLChunk.NumPlays;
            this.baseFrame = new APNGFrame(game, image.DefaultImage);

            foreach (Frame frame in image.Frames)
            {
                this.frameList.Add(new APNGFrame(game, frame));
            }

            this.sb = new SpriteBatch(this.game.GraphicsDevice);

            this.RenderEachFrame();
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public Texture2D CurrentFrame { get; private set; }

        #endregion Public Properties

        #region Public Methods and Operators

        public void Update(GameTime gameTime)
        {
            if (this.CurrentFrame == null)
            {
                this.CurrentFrame = this.baseFrame.FrameTexture;
            }

            if (this.numPlays != 0 && this.alreadyPlays >= this.numPlays)
            {
                this.CurrentFrame = this.renderedTextureList[0];
            }

            if (this.alreadyWaitTime > this.frameList[this.currentPlayedIndex].DelayTime)
            {
                this.currentPlayedIndex = this.currentPlayedIndex < this.renderedTextureList.Count - 1
                                              ? this.currentPlayedIndex + 1
                                              : 0;

                this.CurrentFrame = this.renderedTextureList[this.currentPlayedIndex];

                this.alreadyWaitTime = TimeSpan.Zero;

                this.alreadyPlays++;
            }
            else
            {
                this.alreadyWaitTime += gameTime.ElapsedGameTime;
            }
        }

        #endregion Public Methods and Operators

        #region Methods

        private void RenderEachFrame()
        {
            for (int crtIndex = 0; crtIndex < this.frameList.Count; crtIndex++)
            {
                var currentTexture = new RenderTarget2D(
                    this.game.GraphicsDevice, this.baseFrame.Width, this.baseFrame.Height);

                this.game.GraphicsDevice.SetRenderTarget(currentTexture);
                this.game.GraphicsDevice.Clear(Color.Transparent);

                // if this is the first frame, just draw.
                if (crtIndex == 0)
                {
                    goto LABEL_DRAW_NEW_FRAME;
                }

                // Restore previous texture
                this.sb.Begin();
                this.sb.Draw(this.renderedTextureList[crtIndex - 1], Vector2.Zero, Color.White);
                this.sb.End();

                APNGFrame crtFrame = this.frameList[crtIndex - 1];

                switch (crtFrame.DisposeOp)
                {
                    // Do nothing.
                    case DisposeOps.APNGDisposeOpNone:
                        break;

                    // Set current Rectangle to transparent.
                    case DisposeOps.APNGDisposeOpBackground:
                    LABEL_APNG_DISPOSE_OP_BACKGROUND:
                        var t2 = new Texture2D(this.game.GraphicsDevice, 1, 1);
                        this.sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        this.sb.Draw(
                            t2,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        this.sb.End();
                        break;

                    // Rollback to previous frame.
                    case DisposeOps.APNGDisposeOpPrevious:
                        // If the first `fcTL` chunk uses a `dispose_op` of APNG_DISPOSE_OP_PREVIOUS
                        // it should be treated as APNG_DISPOSE_OP_BACKGROUND.
                        if (crtIndex - 1 == 0)
                        {
                            goto LABEL_APNG_DISPOSE_OP_BACKGROUND;
                        }

                        APNGFrame prevFrame = this.frameList[crtIndex - 2];

                        this.sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        this.sb.Draw(
                            prevFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        this.sb.End();
                        break;
                }

            LABEL_DRAW_NEW_FRAME:
                // Now let's look at the new frame.
                if (crtIndex == 0)
                {
                    crtFrame = this.frameList[0];
                }
                else
                {
                    crtFrame = crtIndex < this.frameList.Count
                                   ? this.frameList[crtIndex]
                                   : this.frameList[0];
                }

                switch (crtFrame.BlendOp)
                {
                    // Do not apply alpha
                    case BlendOps.APNGBlendOpSource:
                        this.sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        this.sb.Draw(
                            crtFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        this.sb.End();
                        break;

                    // Apply alpha
                    case BlendOps.APNGBlendOpOver:
                        this.sb.Begin();
                        this.sb.Draw(
                            crtFrame.FrameTexture,
                            new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                            Color.White);
                        this.sb.End();
                        break;
                }

                this.renderedTextureList.Add(currentTexture);
            }

            // Okay it's all over now
            this.game.GraphicsDevice.SetRenderTarget(null);
        }

        #endregion Methods
    }
}