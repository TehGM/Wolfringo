using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Represents wolf error codes.</summary>
    public enum WolfErrorCode
    {
        /// <summary>User does not exist.</summary>
        NoSuchUser = 0,
        /// <summary>Login was incorrect, or cannot send message to the group.</summary>
        // why tf use same event code for 2 different things is beyond me...
        LoginIncorrectOrCannotSendToGroup = 1,
        /// <summary>Terms of Service violations.</summary>
        TosViolations = 2,
        /// <summary>Higher reputation level is required.</summary>
        HigherLevelRequired = 4,
        /// <summary>Group name is already in use.</summary>
        GroupNameTaken = 8,
        /// <summary>User is already a contact, or group name is blacklisted.</summary>
        // oh look, another error code with double meaning...
        AlreadyContactOrGroupNameForbidden = 9,
        /// <summary>Group name is too short.</summary>
        GroupNameTooShort = 15,
        /// <summary>Group is full.</summary>
        GroupFull = 100,
        /// <summary>Already in max count of groups.</summary>
        MaxGroupsReached = 101,
        /// <summary>Group does not exist.</summary>
        GroupNotExisting = 105,
        /// <summary>User banned in the group.</summary>
        Banned = 107,
        /// <summary>User already a member of the group.</summary>
        AlreadyInGroup = 110,
        /// <summary>Only new users can perform this action.</summary>
        RestrictedToNewUsers = 112,
        /// <summary>Group is locked.</summary>
        GroupLocked = 115,
        /// <summary>Too many accounts.</summary>
        TooManyAccounts = 116,
        /// <summary>Game join only.</summary>
        GameJoinOnly = 117
    }

    /// <summary>Extension methods for <see cref="WolfErrorCode"/>.</summary>
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
                        if (sentCommand != null && string.Equals(sentCommand, MessageEventNames.SecurityLogin, StringComparison.OrdinalIgnoreCase))
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
                        if (sentCommand != null && !string.Equals(sentCommand, MessageEventNames.SubscriberContactAdd, StringComparison.OrdinalIgnoreCase))
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
