import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

// The Impressum must stay reachable without a session (§5 TMG) - it's the one
// page a logged-out visitor needs to be able to see besides /login itself.
const PUBLIC_PATHS = ["/login", "/impressum"];
const BACKEND_URL = process.env.BACKEND_URL ?? "http://localhost:5128";

// Every page requires a session, matching the backend's gdc_session cookie.
// /api/* is excluded so the GitHub OAuth redirect/callback and every
// rewritten backend call keep working - that's exactly how you get the
// cookie in the first place.
export async function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (PUBLIC_PATHS.includes(pathname)) {
    return NextResponse.next();
  }

  if (!request.cookies.has("gdc_session")) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // A cookie merely being present isn't enough: it can outlive the session it
  // points to (expired server-side, or undecryptable after the backend's
  // signing key rotated), in which case the backend treats the request as
  // anonymous even though the browser still holds the cookie. Confirm the
  // session is actually still valid before letting the request through.
  const meRes = await fetch(`${BACKEND_URL}/api/auth/me`, {
    headers: { cookie: request.headers.get("cookie") ?? "" },
    cache: "no-store",
  });

  if (!meRes.ok) {
    const response = NextResponse.redirect(new URL("/login", request.url));
    response.cookies.delete("gdc_session");
    return response;
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"],
};
