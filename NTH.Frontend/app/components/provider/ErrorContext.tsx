import { createContext } from "react";
// For React refresh to work correctly, your file should only export React components.
const ErrorContext = createContext((message: string) => { });
export default ErrorContext;
