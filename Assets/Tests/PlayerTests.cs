using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Player;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    [Test]
    public void PlayerTakeDamage()
    {
        var player = new GameObject().AddComponent<FirstPersonController>();
        player.SetHealth(100);
        player.TakeDamage(25);
        Assert.AreEqual(75, player.GetHealth(), "player health is : " + player.GetHealth());

    }
}
