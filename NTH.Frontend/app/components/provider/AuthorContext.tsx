import { createContext } from "react";
import type { AuthorBasic } from "~/types";

// For React refresh to work correctly, your file should only export React components.
const AuthorContext = createContext<{ authors: AuthorBasic[], setAuthors: React.Dispatch<React.SetStateAction<AuthorBasic[]>> }>
    ({ authors: [], setAuthors: () => { } });

export default AuthorContext;
