import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const PUBLIC_PATHS = ["/login"];

// Every page requires a session, matching the backend's gdc_session cookie.
// /api/* is excluded so the GitHub OAuth redirect/callback and every
// rewritten backend call keep working - that's exactly how you get the
// cookie in the first place.
export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (PUBLIC_PATHS.includes(pathname)) {
    return NextResponse.next();
  }

  if (!request.cookies.has("gdc_session")) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"],
};
