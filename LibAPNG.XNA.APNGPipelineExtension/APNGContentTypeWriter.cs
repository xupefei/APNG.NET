using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace LibAPNG.XNA.APNGPipelineExtension
{
    [ContentTypeWriter]
    public class APNGContentTypeWriter : ContentTypeWriter<APNGFrameList>
    {
        protected override void Write(ContentWriter output, APNGFrameList value)
        {
            output.Write(value.LoopCount);
            output.Write(value.Frames.Length);
            foreach (Frame frame in value.Frames)
            {
                output.Write(frame.DelayTime.Ticks);
                output.WriteObject(frame.Content);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "LibAPNG.XNA.APNGContentTypeReader,LibAPNG.XNA";
        }
    }
}