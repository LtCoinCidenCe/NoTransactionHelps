import { createContext } from "react";

const template = { jwt: "", expireTime: new Date(1990, 2, 3), userIdentifier: "" };
// For React refresh to work correctly, your file should only export React components.
const JWTContext = createContext(template);
export default JWTContext;
