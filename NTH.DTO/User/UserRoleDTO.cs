namespace NTH.DTO.User;

public enum UserRoleDTO
{
    User = 0,
    Wanderer = 0b0001,
    Translator = 0b0010,
    Scriptor = 0b0100,
    Manager = 0b1000,
    SystemAdministrator = 0x80,

    // This works as intended but hopefully we have the "whole set"
    Compound = User | Translator | Scriptor | Manager,

    SuperAdministrator = 0x7f_ff_ff_ff,
}
