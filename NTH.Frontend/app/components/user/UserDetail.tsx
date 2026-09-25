import React, { useContext } from "react";
import JWTContext from "../provider/JWTContext";
import ErrorContext from "../provider/ErrorContext";

const UserDetail: React.FC<{ userID: number }> = ({ userID }) => {
  const { jwt, expireTime, userIdentifier } = useContext(JWTContext);
  const errorContext = useContext(ErrorContext);
  return <div>{userID}</div>;
}

export default UserDetail;
