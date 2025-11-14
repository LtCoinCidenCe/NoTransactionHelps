namespace NTH.DTO.User;

public enum UserRoleDTO
{
    User = 0,
    Translator = 0b0001,
    Scriptor = 0b0010,
    Manager = 0b0100,
    SystemAdministrator = 0x80,

    // This works as intended but hopefully we have the "whole set"
    Compound = User | Translator | Scriptor | Manager,

    SuperAdministrator = 0x7f_ff_ff_ff,
}

