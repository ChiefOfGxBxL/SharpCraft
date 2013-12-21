using System;

namespace TinkerWorX.SharpCraft.Game.Jass
{
    [JassType("Hlightning;")]
    public partial struct JassLightning
    {
        public readonly IntPtr Handle;
        
        public JassLightning(IntPtr handle)
        {
            this.Handle = handle;
        }
    }
}
