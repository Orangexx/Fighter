using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    bool FlipX { get; }

    void HitboxContact(ContactData contactData);
}

