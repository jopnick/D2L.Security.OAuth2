﻿using System;

namespace D2L.Security.BrowserAuthTokens.Tests.Utilities {
	internal static class TestUris {

		private static readonly Uri AUTH_SERVER_SITE = new Uri( "https://auth.proddev.d2l:44331" );

		internal static readonly Uri AUTH_TOKEN_PROVISIONING_URI = new Uri( AUTH_SERVER_SITE, "core/connect/token/" );
		internal static readonly Uri TOKEN_VERIFICATION_AUTHORITY_URI = new Uri( AUTH_SERVER_SITE, "core/" );
	}
}
