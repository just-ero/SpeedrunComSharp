﻿using System;
using System.Collections.Generic;

namespace SpeedrunComSharp;

public class Player
{
    public bool IsUser => string.IsNullOrEmpty(GuestName);
    public string UserID { get; private set; }
    public string GuestName { get; private set; }

    #region Links

    internal Lazy<User> user;
    private Lazy<Guest> guest;

    public User User => user.Value;
    public Guest Guest => guest.Value;
    public string Name => IsUser ? User.Name : GuestName;

    #endregion

    private Player() { }

    public static Player Parse(SpeedrunComClient client, dynamic playerElement)
    {
        var player = new Player();

        var properties = playerElement.Properties as IDictionary<string, dynamic>;

        if (properties.ContainsKey("uri"))
        {
            if ((playerElement.rel as string) == "user")
            {
                player.UserID = playerElement.id as string;
                player.user = new Lazy<User>(() => client.Users.GetUser(player.UserID));
                player.guest = new Lazy<Guest>(() => null);
            }
            else
            {
                player.GuestName = playerElement.name as string;
                player.guest = new Lazy<Guest>(() => client.Guests.GetGuest(player.GuestName));
                player.user = new Lazy<User>(() => null);
            }
        }
        else
        {
            if ((playerElement.rel as string) == "user")
            {
                var user = User.Parse(client, playerElement) as User;
                player.user = new Lazy<User>(() => user);
                player.UserID = user.ID;
                player.guest = new Lazy<Guest>(() => null);
            }
            else
            {
                var guest = Guest.Parse(client, playerElement) as Guest;
                player.guest = new Lazy<Guest>(() => guest);
                player.GuestName = guest.Name;
                player.user = new Lazy<User>(() => null);
            }
        }

        return player;
    }

    public override int GetHashCode()
    {
        return (UserID ?? string.Empty).GetHashCode()
            ^ (GuestName ?? string.Empty).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is not Player player)
        {
            return false;
        }

        return UserID == player.UserID
            && GuestName == player.GuestName;
    }

    public override string ToString()
    {
        return Name;
    }
}
