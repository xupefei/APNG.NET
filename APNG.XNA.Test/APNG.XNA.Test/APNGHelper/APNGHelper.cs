using System;
using System.Collections.Generic;

using APNG;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace APNGTest.APNGHelper
{
    public class APNGHelper
    {
        private readonly Game game;
        private APNGFrame baseFrame;
        private List<APNGFrame> frameList = new List<APNGFrame>();
        private int numPlays;
        private int alreadyPlays;
        private TimeSpan alreadyWaitTime = TimeSpan.Zero;
        private int currentFrameIndex = 0;
        private RenderTarget2D currentTexture;
        private SpriteBatch sb;

        public Texture2D CurrentFrame { get; private set; }

        public APNGHelper(Game game, string pngFile)
        {
            this.game = game;

            var image = new APNG.APNG(pngFile);
            this.numPlays = (int)image.acTLChunk.NumPlays;
            this.baseFrame = new APNGFrame(game, image.DefaultImage);

            foreach (var frame in image.Frames)
                this.frameList.Add(new APNGFrame(game, frame));

            currentTexture = new RenderTarget2D(
                game.GraphicsDevice,
                this.baseFrame.Width,
                this.baseFrame.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            sb = new SpriteBatch(this.game.GraphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            game.GraphicsDevice.SetRenderTarget(currentTexture);
            game.GraphicsDevice.Clear(Color.Transparent);

            // Restore previous texture
            if (this.CurrentFrame != null)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                sb.Draw(this.CurrentFrame, Vector2.Zero, Color.White);
                sb.End();
            }

            var crtFrame = frameList[currentFrameIndex];

            if (alreadyWaitTime < crtFrame.DelayTime)
            {
                alreadyWaitTime += gameTime.ElapsedGameTime;
                goto LABEL_DRAW_NEW_FRAME;
            }
            // alreadyWaitTime >= crtFrame.DelayTime
            //
            // Reset timer
            alreadyWaitTime = TimeSpan.Zero;

            switch (crtFrame.DisposeOp)
            {
                // Do nothing.
                case DisposeOps.APNGDisposeOpNone:
                    break;

                // Set current Rectangle to transparent.
                case DisposeOps.APNGDisposeOpBackground:
                LABEL_APNG_DISPOSE_OP_BACKGROUND:
                    var t2 = new Texture2D(game.GraphicsDevice, 1, 1);
                    sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                    sb.Draw(
                        t2,
                        new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                        Color.White);
                    sb.End();
                    break;

                // Rollback to previous frame.
                case DisposeOps.APNGDisposeOpPrevious:
                    // If the first `fcTL` chunk uses a `dispose_op` of APNG_DISPOSE_OP_PREVIOUS
                    // it should be treated as APNG_DISPOSE_OP_BACKGROUND.
                    if (currentFrameIndex == 0)
                        goto LABEL_APNG_DISPOSE_OP_BACKGROUND;

                    var prevFrame = frameList[currentFrameIndex - 1];

                    sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                    sb.Draw(
                        prevFrame.FrameTexture,
                        new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                        new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                        Color.White);
                    sb.End();
                    break;
            }

            NextFrame();

        LABEL_DRAW_NEW_FRAME:
            // Now let's look at the new frame.
            crtFrame = frameList[currentFrameIndex];
            switch (crtFrame.BlendOp)
            {
                // Do not apply alpha
                case BlendOps.APNGBlendOpSource:
                    sb.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                    sb.Draw(
                        crtFrame.FrameTexture,
                        new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                        Color.White);
                    sb.End();
                    break;

                // Apply alpha
                case BlendOps.APNGBlendOpOver:
                    sb.Begin();
                    sb.Draw(
                        crtFrame.FrameTexture,
                        new Rectangle(crtFrame.X, crtFrame.Y, crtFrame.Width, crtFrame.Height),
                        Color.White);
                    sb.End();
                    break;
            }

            // Okay it's all over now
            game.GraphicsDevice.SetRenderTarget(null);

            // Backup current texture for next use.
            var cc = new Color[currentTexture.Width * currentTexture.Height];
            currentTexture.GetData(cc);
            if (CurrentFrame == null)
                CurrentFrame = new Texture2D(game.GraphicsDevice, currentTexture.Width, currentTexture.Height);
            this.CurrentFrame.SetData(cc);

            game.ResetElapsedTime();
        }

        private void NextFrame()
        {
            if (numPlays != 0 && numPlays <= alreadyPlays)
                currentFrameIndex = 0;

            if (++currentFrameIndex > frameList.Count - 1)
            {
                alreadyPlays++;

                currentFrameIndex = 0;
            }
        }
    }
}