import { useContext } from "react";
import type { AuthorContactItem } from "~/types";
import ErrorContext from "../provider/ErrorContext";
import AuthorContext from "../provider/AuthorContext";

const CurrentAuthorContact = (contacts: AuthorContactItem[]) => {
  const voidContact: AuthorContactItem = { id: 0, byUserAudit: 0, userID: 0, changeDate: new Date(0) };
  return contacts.reduce(
    (prev, current) => current.id > prev.id ? current : prev,
    voidContact);
};

const AuthorGrid: React.FC = () => {
  const errorContext = useContext(ErrorContext);
  const { authors, setAuthors } = useContext(AuthorContext);
  console.log(authors);
  console.log(authors.map(a => CurrentAuthorContact(a.contact)));
  return <p onClick={() => errorContext(`not found ${new Date()}`)} >something unusual</p>;
};

export default AuthorGrid;
