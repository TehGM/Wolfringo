using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Represents wolf error codes.</summary>
    public enum WolfErrorCode
    {
        NoSuchUser = 0,
        // why tf use same event code for 2 different things is beyond me...
        LoginIncorrectOrCannotSendToGroup = 1,
        TosViolations = 2,
        HigherLevelRequired = 4,
        GroupNameTaken = 8,
        // oh look, another error code with double meaning...
        AlreadyContactOrGroupNameForbidden = 9,
        GroupNameTooShort = 15,
        GroupFull = 100,
        MaxGroupsReached = 101,
        GroupNotExisting = 105,
        Banned = 107,
        AlreadyInGroup = 110,
        RestrictedToNewUsers = 112,
        GroupLocked = 115,
        TooManyAccounts = 116,
        GameJoinOnly = 117
    }

    public static class WolfErrorCodeExtensions
    {
        /// <summary>Gets error code description based on sent command.</summary>
        /// <param name="code">Error code.</param>
        /// <param name="sentCommand">Sent command.</param>
        /// <returns>Error code description.</returns>
        public static string GetDescription(this WolfErrorCode code, string sentCommand = null)
        {
            if (!Enum.IsDefined(code.GetType(), code))
                return $"Unknown error code {(int)code}";

            switch (code)
            {
                case WolfErrorCode.NoSuchUser:
                    return "User does not exist";
                case WolfErrorCode.LoginIncorrectOrCannotSendToGroup:
                    {
                        if (sentCommand != null && !string.Equals(sentCommand, MessageCommands.SecurityLogin, StringComparison.OrdinalIgnoreCase))
                            return "Incorrect login credentials";
                        return "Silenced, banned, or not in group";
                    }
                case WolfErrorCode.TosViolations:
                    return "Terms of Service violations";
                case WolfErrorCode.HigherLevelRequired:
                    return "Higher level required";
                case WolfErrorCode.GroupNameTaken:
                    return "Group name is already taken";
                case WolfErrorCode.AlreadyContactOrGroupNameForbidden:
                    {
                        if (sentCommand != null && !string.Equals(sentCommand, MessageCommands.SubscriberContactAdd, StringComparison.OrdinalIgnoreCase))
                            return "Group name is not allowed";
                        return "Contact already added";
                    }
                case WolfErrorCode.GroupNameTooShort:
                    return "Group name is too short";
                case WolfErrorCode.GroupFull:
                    return "Group members list is full";
                case WolfErrorCode.MaxGroupsReached:
                    return "Already in max amount of groups";
                case WolfErrorCode.GroupNotExisting:
                    return "Group does not exist";
                case WolfErrorCode.Banned:
                    return "Banned";
                case WolfErrorCode.AlreadyInGroup:
                    return "Already in the group";
                case WolfErrorCode.RestrictedToNewUsers:
                    return "Restricted to new users";
                case WolfErrorCode.GroupLocked:
                    return "Group is locked";
                case WolfErrorCode.TooManyAccounts:
                    return "Too many accounts";
                case WolfErrorCode.GameJoinOnly:
                    return "Game join only";
                default:
                    return code.ToString();
            }
        }
    }
}
