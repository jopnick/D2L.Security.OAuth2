﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using D2L.Security.AuthTokenValidation;
using D2L.Security.RequestAuthentication.Core.Default;
using Moq;
using NUnit.Framework;

namespace D2L.Security.RequestAuthentication.Tests.Unit.Core.Default {
	
	[TestFixture]
	internal sealed class IValidatedTokenExtensionsTests {

		private void MockClaim( Mock<IValidatedToken> validatedTokenMock, string claimName, string claimValue ) {
			Claim claim = new Claim( claimName, claimValue );
			Claim[] claims = new Claim[] { claim };
			validatedTokenMock.SetupGet( x => x.Claims ).Returns( claims );
		}

		[Test]
		public void GetXsrfToken_Success() {
			string expected = "somexsrf";
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			MockClaim( validatedTokenMock, Constants.Claims.XSRF, expected );
			Assert.AreEqual( expected, validatedTokenMock.Object.GetXsrfToken() );
		}

		[Test]
		public void GetXsrfToken_None_ReturnsNull() {
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			Assert.IsNull( validatedTokenMock.Object.GetXsrfToken() );
		}

		[Test]
		public void GetTenantId_Success() {
			string expected = "sometenantid";
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			MockClaim( validatedTokenMock, Constants.Claims.TENANT_ID, expected );
			Assert.AreEqual( expected, validatedTokenMock.Object.GetTenantId() );
		}

		[Test]
		public void GetTenantId_None_ReturnsNull() {
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			Assert.IsNull( validatedTokenMock.Object.GetTenantId() );
		}

		[Test]
		public void GetScopes_One_Success() {
			string expected = "somescope";
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			MockClaim( validatedTokenMock, Constants.Claims.SCOPE, expected );
			IEnumerable<string> scopes = validatedTokenMock.Object.GetScopes();
			Assert.AreEqual( 1, scopes.Count() );
			Assert.AreEqual( expected, scopes.First() );
		}

		[Test]
		public void GetScopes_Many_Success() {
			string scope1 = "somescope1";
			string scope2 = "somescope2";
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			MockClaim( validatedTokenMock, Constants.Claims.SCOPE, scope1 + " " + scope2 );
			IEnumerable<string> scopes = validatedTokenMock.Object.GetScopes();
			Assert.AreEqual( 2, scopes.Count() );
			Assert.IsTrue( scopes.Contains( scope1 ) );
			Assert.IsTrue( scopes.Contains( scope2 ) );
		}

		[Test]
		public void GetScopes_None_ReturnsEmpty() {
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			IEnumerable<string> scopes = validatedTokenMock.Object.GetScopes();
			Assert.IsFalse( scopes.Any() );
		}

		[Test]
		public void GetUserId_Success() {
			string expected = "userid-1337";
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			MockClaim( validatedTokenMock, Constants.Claims.USER_ID, expected.ToString() );
			Assert.AreEqual( expected, validatedTokenMock.Object.GetUserId() );
		}
		
		[Test]
		public void GetUserId_None_ReturnsNull() {
			Mock<IValidatedToken> validatedTokenMock = new Mock<IValidatedToken>();
			Assert.IsNull( validatedTokenMock.Object.GetUserId() );
		}
	}
}