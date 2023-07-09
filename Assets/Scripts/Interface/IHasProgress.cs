using System;

namespace Interface
{
    public interface IHasProgress 
    {
        public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
        public class OnProgressChangedEventArgs : EventArgs
        {
            public float ProgressNormalized;
        }
    }
}
