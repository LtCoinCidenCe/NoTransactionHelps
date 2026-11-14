import { JwtClaimsSchema } from "~/types";

export const parseJWT = (jwtString: string) => {
  const parts = jwtString.split(".");
  if (parts.length !== 3)
    return null;
  try {
    const payloadbase64 = parts[1];
    const jsonStr = atob(payloadbase64) // ignore the legacy thing
    const payload = JSON.parse(jsonStr); // leave it to catch error
    const { exp, iss, aud } = JwtClaimsSchema.parse(payload); // leave it to catch error

    // jwt exp is second, but JavaScript Date is millisecond
    let expireTime = new Date(exp * 1000);
    return { expireTime, userIdentifier: aud };
  } catch (error) {
    return null;
  }
}
