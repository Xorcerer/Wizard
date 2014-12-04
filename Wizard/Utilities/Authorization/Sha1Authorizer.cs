using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace Xorcerer.Wizard.Utilities.Authorization
{
    class Sha1Authorizer : IAuthorizer
    {
        public const int LengthOfRawSaltInBytes = 8;
        public const string IdSaltSplitter = "|";

        /// <summary>
        /// The random bytes generator for salt.
        /// All methods are thread safe according to msdn.
        /// ref: http://msdn.microsoft.com/en-us/library/system.security.cryptography.rngcryptoserviceprovider.aspx
        /// </summary>
        static RNGCryptoServiceProvider s_cryptoServiceProvider = new RNGCryptoServiceProvider();

        #region IAuthorizer implementation

        public string GenerateSalt()
        {
            byte[] saltInBytes = new byte[LengthOfRawSaltInBytes];
            s_cryptoServiceProvider.GetBytes(saltInBytes);

            return Convert.ToBase64String(saltInBytes);
        }

        public string CreateAuthHash(string salt, string id, string password)
        {
            return Encoding.UTF8.GetString(GetHashBytes(salt, id, password));
        }

        public bool Validate(string id, string transientSalt, string authToken, string authHash)
        {
            byte[] rawHash = Convert.FromBase64String(authToken);

            var hash = GetHashBytes(id, transientSalt, authHash);

            // A faster way to compare two byte array.
            // http://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
            return hash.Length == rawHash.Length && hash.SequenceEqual(rawHash);
        }

        #endregion

        /// <summary>
        /// Gets the hash in bytes from the combining of id, salt, rawData.
        /// </summary>
        /// <returns>The hash in bytes.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="salt">Salt.</param>
        /// <param name="rawData">Raw data.</param>
        public static byte[] GetHashBytes(string salt, string id, string rawData)
        {
            // Not thread safe.
            SHA1 sha = new SHA1CryptoServiceProvider();
            // Hash source string: "salt|id|salt"
            byte[] data = Encoding.UTF8.GetBytes(string.Join(IdSaltSplitter, salt, id, rawData));
            byte[] hash = sha.ComputeHash(data);
            return hash;
        }

    }
}
