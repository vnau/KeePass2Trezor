using KeePassLib.Serialization;
using System.Collections.Generic;

namespace KeePass2Trezor
{
    /// <summary>
    /// Cache containing IDs of Trezor keys for the existing connections.
    /// </summary>
    class TrezorKeysCache
    {
        public static readonly string TrezorPropertyKey = "trezor";
        private readonly Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
        private TrezorKeysCache() { }

        private static readonly TrezorKeysCache instance = new TrezorKeysCache();
        public static TrezorKeysCache Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Add Trezor key ID to cache for specific connection.
        /// </summary>
        /// <param name="connection">Connection info</param>
        /// <param name="keyId">Trezor key ID</param>
        public void Add(IOConnectionInfo connection, byte[] keyId)
        {
            lock (cache)
            {
                cache[connection.Path] = keyId;
            }
        }

        /// <summary>
        /// Get Trezor key ID for specified connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>Trezor key ID</returns>
        public byte[] Get(IOConnectionInfo connection)
        {
            lock (cache)
            {
                return cache.ContainsKey(connection.Path) ? cache[connection.Path] : null;
            }
        }

        /// <summary>
        /// Remove Trezor key ID for specified connection.
        /// </summary>
        /// <param name="connection"></param>
        public void Remove(IOConnectionInfo connection)
        {
            lock (cache)
            {
                if (cache.ContainsKey(connection.Path))
                    cache.Remove(connection.Path);
            }
        }
    }
}
