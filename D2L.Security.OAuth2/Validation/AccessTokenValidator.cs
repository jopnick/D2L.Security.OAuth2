﻿using System;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using D2L.Security.OAuth2.Validation.Jwks;
using D2L.Security.OAuth2.Validation.Token;
using D2L.Security.OAuth2.Validation.Token.JwtValidation;

namespace D2L.Security.OAuth2.Validation {
	internal sealed class AccessTokenValidator : IAccessTokenValidator {

		private readonly ISecurityTokenProvider m_securityTokenProvider;
		private readonly JwtSecurityTokenHandler m_tokenHandler = new JwtSecurityTokenHandler();

		public AccessTokenValidator(
			ISecurityTokenProvider securityTokenProvider
		) {
			m_securityTokenProvider = securityTokenProvider;
		}

		async Task<ValidationResponse> IAccessTokenValidator.ValidateAsync(
			Uri jwksEndPoint,
			string token
		) {

			var unvalidatedToken = (JwtSecurityToken)m_tokenHandler.ReadToken(
				token
			);

			if( !unvalidatedToken.Header.ContainsKey( "kid" ) ) {
				throw new Exception( "KeyId not found in token" );
			}

			// TODO should this be ToString?
			var keyId = (string)unvalidatedToken.Header["kid"];

			SecurityToken signingToken = await m_securityTokenProvider.GetSecurityTokenAsync(
				jwksEndPoint: jwksEndPoint,
				keyId: keyId
			).ConfigureAwait( false );
			
			// TODO ... do we validate audience, issuer, or anything else?
			var validationParameters = new TokenValidationParameters() {
				RequireSignedTokens = true,
				IssuerSigningKeys = signingToken.SecurityKeys
			};
			
			IValidatedToken validatedToken = null;
			var status = ValidationStatus.Success;

			try {

				SecurityToken securityToken;
				m_tokenHandler.ValidateToken(
					token,
					validationParameters,
					out securityToken
				);
				validatedToken = new ValidatedJwt( (JwtSecurityToken)securityToken );

			} catch( SecurityTokenExpiredException ) {
				status = ValidationStatus.Expired;
			}

			return new ValidationResponse(
				status,
				validatedToken
			);

		}

	}
}
