import { createContext } from "react";
import { User0, type UserBasic } from "~/types";

// For React refresh to work correctly, your file should only export React components.
const UserContext = createContext<{ users: UserBasic[], usersMap: Map<number, UserBasic>, currentUser: UserBasic, setUsers: React.Dispatch<React.SetStateAction<UserBasic[]>>, updateCurrentUser: (newUser: UserBasic) => void }>
    ({ users: [], usersMap: new Map(), currentUser: User0, setUsers: () => { }, updateCurrentUser: () => { } });

export default UserContext;
