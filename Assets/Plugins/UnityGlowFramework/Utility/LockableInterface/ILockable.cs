using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface a class can implement to offer a IsLocked boolean. Keeps track of all components holding a lock on a class.
/// </summary>
public interface ILockable 
{
    void Lock(bool locking, Component lockRaiser);
    void ReleaseAll();
}
