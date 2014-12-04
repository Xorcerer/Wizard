using System;

namespace Xorcerer.Wizard.Utilities.Authorization
{
    public interface IAuthorizer
    {
        /// <summary>
        /// Generates the salt for id/password hashing.
        /// ref: https://zh.wikipedia.org/wiki/%E7%9B%90_(%E5%AF%86%E7%A0%81%E5%AD%A6)
        /// </summary>
        /// <returns>The salt.</returns>
        string GenerateSalt();

        /// <summary>
        /// Creates the authorization hash.
        /// WARNING: This method is not recommended being used in server side.
        /// The server should only generate the salt, send to client, then get the whole autorization hash back.
        /// So the server NEVER save neither the raw password nor the hash of raw password.
        /// </summary>
        /// <returns>The auth hash.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="salt">Salt.</param>
        /// <param name="password">Password.</param>
        string CreateAuthHash(string salt, string id, string password);

        /// <summary>
        /// Check the authToken and authHash are matched.
        /// The authToken should be a string generated from the combining of id, authHash, transientSalt.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="transientSalt">Transient salt, NOT THE SAME salt in <c>CreateAuthHash</c>, just used once.</param>
        /// <param name="authToken">Auth token.</param>
        /// <param name="authHash">Auth hash.</param>
        bool Validate(string id, string transientSalt, string authToken, string authHash);
    }
}

