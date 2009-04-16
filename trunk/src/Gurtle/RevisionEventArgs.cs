namespace Gurtle
{
    #region Imports

    using System;

    #endregion

    [Serializable]
    internal sealed class RevisionEventArgs : EventArgs
    {
        public int Revision { get; private set; }

        public RevisionEventArgs(int revision)
        {
            Revision = revision;
        }

        public override string ToString()
        {
            return Revision.ToString();
        }
    }
}