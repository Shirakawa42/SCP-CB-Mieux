using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HandTarget : MonoBehaviour
{
    public abstract void Click(PlayerHandScript other);
}
