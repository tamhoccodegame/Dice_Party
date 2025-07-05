using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;

    public const byte JUMPBUTTON = 1;

    public NetworkButtons buttons;
}
