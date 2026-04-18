namespace Sackrany.PrefabManager.Hash
{
    public static class FNV1a
    {
        public static int Compute(string str)
        {
            unchecked
            {
                int hash = (int)2166136261;
                foreach (var c in str)
                    hash = (hash ^ c) * 16777619;
                return hash;
            }
        }
    }
}