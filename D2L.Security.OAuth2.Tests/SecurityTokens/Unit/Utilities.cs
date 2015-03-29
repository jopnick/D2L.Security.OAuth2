﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;

using D2L.Security.OAuth2.SecurityTokens;

using NUnit.Framework;

namespace D2L.Security.OAuth2.Tests.SecurityTokens.Unit {
	internal static class Utilities {
		public static readonly TimeSpan ROTATION_BUFFER = TimeSpan.FromMinutes( 10 );
		public static readonly TimeSpan TOKEN_LIFETIME = TimeSpan.FromHours( 1 );

		public static void AssertNumberOfTokensStored(
			ISecurityTokenManager manager,
			long num
		) {
			Assert.AreEqual(
				num,
				manager.GetAllTokens().Count()
			);
		}

		public static D2LSecurityToken CreateExpiredToken() {
			return CreateTokenWithTimeRemaining(
				-TimeSpan.FromHours( 10 )
			);
		}

		public static D2LSecurityToken CreateExpiringToken() {
			return CreateTokenWithTimeRemaining(
				ROTATION_BUFFER - TimeSpan.FromSeconds( 30 )
				);
		}

		public static D2LSecurityToken CreateActiveToken() {
			return CreateTokenWithTimeRemaining(
				TOKEN_LIFETIME - TimeSpan.FromSeconds( 1 )
			);
		}

		public static D2LSecurityToken CreateTokenWithTimeRemaining( TimeSpan remaining ) {
			var validTo = DateTime.UtcNow + remaining;
			var validFrom = validTo - TOKEN_LIFETIME;
			var csp = new RSACryptoServiceProvider( 2048 ) {
				PersistKeyInCsp = false
			};
			var key = new RsaSecurityKey( csp );

			return new D2LSecurityToken(
				Guid.NewGuid(),
				validFrom,
				validTo,
				key
			);
		}

		public static D2LSecurityToken CreateTokenWithoutPrivateKey() {
			var validTo = DateTime.UtcNow + TimeSpan.FromHours( 1 );
			var validFrom = DateTime.UtcNow - TimeSpan.FromHours( 1 );
			var csp = new RSACryptoServiceProvider( 2048 ) {
				PersistKeyInCsp = false
			};
			var ps = csp.ExportParameters( includePrivateParameters: false );
			csp.ImportParameters( ps );
			var key = new RsaSecurityKey( csp );

			return new D2LSecurityToken(
				Guid.NewGuid(),
				validFrom,
				validTo,
				key
			);
		}

		public static void AssertTokenActive( D2LSecurityToken token ) {
			Assert.False( token.IsExpired() );
			Assert.False( token.IsExpiringSoon( ROTATION_BUFFER ) );
		}

		public static void AssertTokensHavePrivateKeys( IEnumerable<D2LSecurityToken> tokens ) {
			foreach( var token in tokens ) {
				Assert.IsTrue( token.HasPrivateKey() );
			}
		}

		public static void AssertTokensDoNotHavePrivateKeys( IEnumerable<D2LSecurityToken> tokens ) {
			foreach( var token in tokens ) {
				Assert.IsFalse( token.HasPrivateKey() );
			}
		}
	}
}
