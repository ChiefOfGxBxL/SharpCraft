using System;

namespace TinkerWorX.SharpCraft.Game.Jass
{
    [JassType("Hfilterfunc;")]
    [Serializable]
    public partial struct JassFilterFunction
    {
        public readonly IntPtr Handle;
        
        public JassFilterFunction(IntPtr handle)
        {
            this.Handle = handle;
        }
    }
}