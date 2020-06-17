This solution demonstrates a client-side, non-hosted Blazor Web Assembly app that
uses OIDC authorization_code workflow to securely call two different APIs.  This
architecture requires Identity Server to be a separate project, rather than
integrated into a host API project.

Note that the Identity Server project combines Identity Server with ASP.NET
Identity for managing users.  The Identity Server project also makes use
of a custom Profile Service class in EDennis.AspNet.Base.

The EDennis.AspNetBase library project contains a subset of classes that
will be included in a Github repo of the same name (to be released in
upcoming weeks).  The OidcLoggingMiddleware is a developer tool for 
observing the OIDC-related communications (especially all of the redirects)
with Identity Server -- which aids in understanding the protocol.

To run the solution, first start EDennis.AspNetIdentityServer with the Seed profile
(to ensure that the database is created and populated).  Once the logging output
appears for Identity Server, then the database is ready.  Then start Api1, Api2,
and BlazorApp3.